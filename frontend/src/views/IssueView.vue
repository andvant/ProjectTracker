<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useIssuesStore } from '@/stores/issues'
import { useProjectsStore } from '@/stores/projects'
import { useUsersStore } from '@/stores/users'
import { useAuth } from '@/auth/useAuth'
import {
  IssuePriority,
  IssueStatus,
  IssueType,
  type IssueDto,
  UpdateIssueRequest,
} from '@/types/issues'
import { ApiError, type ValidationErrors } from '@/types/api'
import { Role } from '@/types/roles'
import { applyErrorsFromApi, createDefaultErrors, getEnumLabel, getEnumOptions } from '@/utils'
import EntityTitle from '@/components/UI/EntityTitle.vue'
import Property from '@/components/UI/Property.vue'
import InputProperty from '@/components/UI/InputProperty.vue'
import InputErrors from '@/components/UI/InputErrors.vue'
import ControlButton from '@/components/UI/ControlButton.vue'

const route = useRoute()
const router = useRouter()

const issuesStore = useIssuesStore()
const projectsStore = useProjectsStore()
const usersStore = useUsersStore()

const { hasRole, userId } = useAuth()

const projectId = computed(() => projectsStore.getProjectIdByKey(route.params.projectKey as string))
const issueId = computed(() => issuesStore.getIssueIdByKey(route.params.issueKey as string))

const memberUsers = computed(() => projectsStore.cachedProject!.members)

const nonWatcherUsers = computed(() =>
  memberUsers.value.filter((u) => !issue.value?.watchers.find((w) => w.id === u.id)),
)

const canEditIssue = computed(
  () =>
    hasRole(Role.Admin) ||
    userId.value === projectsStore.cachedProject?.owner.id ||
    userId.value === issue.value?.reporter.id,
)

const issue = ref<IssueDto>()

const selectedWatcherId = ref<string | null>()

const req = new UpdateIssueRequest()
const isEditing = ref(false)
const isAddingWatcher = ref(false)
const isSubmitting = ref(false)

type Errors = ValidationErrors<UpdateIssueRequest>

const errors = ref<Errors>(createDefaultErrors(req))

const validate = () => {
  errors.value = createDefaultErrors(req)

  let isValid = true

  if (!req.title.trim()) {
    errors.value.title = 'Title is required'
    isValid = false
  }

  if (req.estimationMinutes && req.estimationMinutes < 0) {
    errors.value.estimationMinutes = 'Estimation minutes cannot be negative'
    isValid = false
  }

  return isValid
}

const onUpdateIssue = async () => {
  if (!validate()) return

  try {
    isSubmitting.value = true

    issue.value = await issuesStore.updateIssue(projectId.value!, issueId.value!, req)

    isEditing.value = false
  } catch (e) {
    if (e instanceof ApiError && e.problem) {
      applyErrorsFromApi(errors.value, e.problem)
    } else {
      errors.value.general = 'Unexpected error occurred'
    }
  } finally {
    isSubmitting.value = false
  }
}

const onEditing = () => {
  errors.value = createDefaultErrors(req)
  req.title = issue.value!.title
  req.description = issue.value!.description
  req.assigneeId = issue.value!.assignee?.id
  req.status = issue.value!.status
  req.priority = issue.value!.priority
  req.dueDate = issue.value!.dueDate
  req.estimationMinutes = issue.value!.estimationMinutes

  isEditing.value = true
}

const onDeleteIssue = async () => {
  router.push({ name: 'Project', params: { projectKey: route.params.projectKey } })

  await issuesStore.deleteIssue(projectId.value!, issueId.value!)
}

const onRemoveWatcher = async (watcherId: string) => {
  await issuesStore.removeWatcher(projectId.value!, issue.value!, watcherId)
}

const onAddingWatcher = async () => {
  await usersStore.fetchUsers()

  selectedWatcherId.value = null
  isAddingWatcher.value = true
}

const onAddWatcher = async () => {
  if (!selectedWatcherId.value) return

  isSubmitting.value = true
  try {
    await issuesStore.addWatcher(projectId.value!, issue.value!, selectedWatcherId.value)
    isAddingWatcher.value = false
  } finally {
    isSubmitting.value = false
  }
}

const onFilesSelected = async (event: Event) => {
  const input = event.target as HTMLInputElement
  if (!input.files) return

  isSubmitting.value = true
  try {
    for (const file of input.files) {
      const formData = new FormData()
      formData.append('file', file)

      issue.value = await issuesStore.uploadAttachment(projectId.value!, issueId.value!, formData)
    }
  } finally {
    isSubmitting.value = false
  }

  input.value = ''
}

watch(
  [projectId, issueId],
  async ([projectId, issueId]) => {
    if (!projectId) return

    isEditing.value = false
    isAddingWatcher.value = false

    if (!projectsStore.cachedProject) {
      await projectsStore.getProject(projectId)
    }

    if (!issueId) {
      await issuesStore.fetchIssues(projectId)
    } else {
      issue.value = await issuesStore.getIssue(projectId, issueId)
    }
  },
  { immediate: true },
)
</script>
<template>
  <div v-if="issue" class="wrapper">
    <EntityTitle v-if="!isEditing" :title="`${issue.key} ${issue.title}`" />

    <InputProperty v-if="isEditing" label="Title" :error="errors.title">
      <input v-model="req.title" />
    </InputProperty>

    <Property label="Project">
      <RouterLink
        :to="{ name: 'Project', params: { projectKey: projectsStore.cachedProject?.key } }"
      >
        {{ projectsStore.cachedProject?.name }}
      </RouterLink>
    </Property>

    <Property v-if="!isEditing" label="Description">{{ issue.description }}</Property>

    <InputProperty v-if="isEditing" label="Description" :error="errors.description">
      <textarea v-model="req.description"></textarea>
    </InputProperty>

    <Property label="Reporter">
      <RouterLink :to="{ name: 'User', params: { userId: issue.reporter.id } }">
        {{ issue.reporter.name }}
      </RouterLink>
    </Property>

    <Property v-if="!isEditing" label="Assignee">
      <span v-if="!issue.assignee">Unassigned</span>
      <RouterLink v-else :to="{ name: 'User', params: { userId: issue.assignee.id } }">
        {{ issue.assignee.name }}
      </RouterLink>
    </Property>

    <InputProperty v-if="isEditing" label="Assignee" :error="errors.assigneeId">
      <select v-model="req.assigneeId">
        <option v-for="user in memberUsers" :key="user.id" :value="user.id">
          {{ user.name }}
        </option>
      </select>
    </InputProperty>

    <Property label="Type">{{ getEnumLabel(IssueType, issue.type) }}</Property>

    <Property v-if="!isEditing" label="Status">
      {{ getEnumLabel(IssueStatus, issue.status) }}
    </Property>

    <InputProperty v-if="isEditing" label="Status" :error="errors.status">
      <select v-model="req.status">
        <option
          v-for="option in getEnumOptions(IssueStatus)"
          :key="option.value"
          :value="option.value"
        >
          {{ option.label }}
        </option>
      </select>
    </InputProperty>

    <Property v-if="!isEditing" label="Priority">
      {{ getEnumLabel(IssuePriority, issue.priority) }}
    </Property>

    <InputProperty v-if="isEditing" label="Priority" :error="errors.priority">
      <select v-model="req.priority">
        <option
          v-for="option in getEnumOptions(IssuePriority)"
          :key="option.value"
          :value="option.value"
        >
          {{ option.label }}
        </option>
      </select>
    </InputProperty>

    <Property v-if="!isEditing" label="Due date">{{ issue.dueDate }}</Property>

    <InputProperty v-if="isEditing" label="Due date" :error="errors.dueDate">
      <input v-model="req.dueDate" type="date" />
    </InputProperty>

    <Property v-if="!isEditing" label="Estimation minutes">
      {{ issue.estimationMinutes }}
    </Property>

    <InputProperty v-if="isEditing" label="Estimation minutes" :error="errors.estimationMinutes">
      <input v-model="req.estimationMinutes" type="number" />
    </InputProperty>

    <InputErrors v-if="isEditing" :error="errors.general" />

    <ControlButton v-if="!isEditing && canEditIssue" @click="onEditing" label="Edit" />
    <ControlButton v-if="isEditing" @click="onUpdateIssue" :disabled="isSubmitting" label="Save" />
    <ControlButton v-if="canEditIssue" @click="onDeleteIssue" label="Delete" />
    <ControlButton v-if="isEditing" @click="isEditing = false" label="Cancel" />

    <Property label="Created at">{{ issue.createdAt }}</Property>
    <Property label="Updated at">{{ issue.updatedAt }}</Property>

    <Property v-if="issue.parentIssue" label="Parent issue">
      <RouterLink :to="{ name: 'Issue', params: { issueKey: issue.parentIssue.key } }">
        {{ issue.parentIssue.key }} {{ issue.parentIssue.title }}
      </RouterLink>
    </Property>

    <Property v-if="issue.childIssues.length" label="Child issues">
      <ul>
        <li v-for="childIssue of issue.childIssues" :key="childIssue.id">
          <RouterLink :to="{ name: 'Issue', params: { issueKey: childIssue.key } }">
            {{ childIssue.key }} {{ childIssue.title }}
          </RouterLink>
        </li>
      </ul>
    </Property>

    <Property label="Watchers">
      <ul>
        <li v-for="watcher in issue.watchers" :key="watcher.id">
          <RouterLink :to="{ name: 'User', params: { userId: watcher.id } }">
            {{ watcher.name }}
          </RouterLink>
          <ControlButton v-if="canEditIssue" @click="onRemoveWatcher(watcher.id)" label="X" />
        </li>
      </ul>
    </Property>

    <div>
      <ControlButton
        v-if="!isAddingWatcher && canEditIssue"
        @click="onAddingWatcher"
        label="Add watcher"
      />

      <div v-if="isAddingWatcher">
        <select v-model="selectedWatcherId">
          <option disabled :value="null">Select a user</option>
          <option v-for="user in nonWatcherUsers" :key="user.id" :value="user.id">
            {{ user.name }}
          </option>
        </select>
        <ControlButton
          @click="onAddWatcher"
          :disabled="!selectedWatcherId || isSubmitting"
          label="Add"
        />
        <ControlButton @click="isAddingWatcher = false" label="Cancel" />
      </div>
    </div>

    <Property label="Attachments">
      <ul>
        <li v-for="attachment in issue.attachments" :key="attachment.id">
          {{ attachment.name }}
        </li>
      </ul>
    </Property>

    <div v-if="canEditIssue">
      <input type="file" multiple @change="(e) => onFilesSelected(e)" :disabled="isSubmitting" />
    </div>
  </div>
</template>
<style scoped>
.wrapper {
  padding: 1rem;
}
</style>
