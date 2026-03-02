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
  <div v-if="issue" class="issue">
    <div>
      <h2 v-if="!isEditing">{{ issue.title }}</h2>
      <div v-else class="form-group">
        <input v-model="req.title" />
        <span v-if="errors.title" class="error">{{ errors.title }}</span>
      </div>
    </div>

    <p>{{ issue.key }}</p>

    <p>Id: {{ issue.id }}</p>

    <p>
      Project:
      <RouterLink
        :to="{ name: 'Project', params: { projectKey: projectsStore.cachedProject?.key } }"
      >
        {{ projectsStore.cachedProject?.name }}
      </RouterLink>
    </p>

    <div>
      <label>Description: </label>
      <span v-if="!isEditing">{{ issue.description }}</span>
      <div v-else class="form-group">
        <textarea v-model="req.description"></textarea>
        <span v-if="errors.description" class="error">{{ errors.description }}</span>
      </div>
    </div>

    <div>
      Reporter:
      <RouterLink :to="{ name: 'User', params: { userId: issue.reporter.id } }">
        {{ issue.reporter.name }}
      </RouterLink>
    </div>

    <div>
      <label>Assignee: </label>
      <span v-if="!isEditing">
        <span v-if="!issue.assignee">Unassigned</span>
        <RouterLink v-else :to="{ name: 'User', params: { userId: issue.assignee.id } }">
          {{ issue.assignee.name }}
        </RouterLink>
      </span>
      <div v-else class="form-group">
        <select v-model="req.assigneeId">
          <option v-for="user in memberUsers" :key="user.id" :value="user.id">
            {{ user.name }}
          </option>
        </select>
        <span v-if="errors.assigneeId" class="error">{{ errors.assigneeId }}</span>
      </div>
    </div>

    <p>Type: {{ getEnumLabel(IssueType, issue.type) }}</p>

    <div>
      <label>Status: </label>
      <span v-if="!isEditing">{{ getEnumLabel(IssueStatus, issue.status) }}</span>
      <div v-else class="form-group">
        <select v-model="req.status">
          <option
            v-for="option in getEnumOptions(IssueStatus)"
            :key="option.value"
            :value="option.value"
          >
            {{ option.label }}
          </option>
        </select>
        <span v-if="errors.status" class="error">{{ errors.status }}</span>
      </div>
    </div>

    <div>
      <label>Priority: </label>
      <span v-if="!isEditing">{{ getEnumLabel(IssuePriority, issue.priority) }}</span>
      <div v-else class="form-group">
        <select v-model="req.priority">
          <option
            v-for="option in getEnumOptions(IssuePriority)"
            :key="option.value"
            :value="option.value"
          >
            {{ option.label }}
          </option>
        </select>
        <span v-if="errors.priority" class="error">{{ errors.priority }}</span>
      </div>
    </div>

    <div>
      <label>Due date: </label>
      <span v-if="!isEditing">{{ issue.dueDate }}</span>
      <div v-else class="form-group">
        <input v-model="req.dueDate" type="date" />
        <span v-if="errors.dueDate" class="error">{{ errors.dueDate }}</span>
      </div>
    </div>

    <div>
      <label>Estimation minutes: </label>
      <span v-if="!isEditing">{{ issue.estimationMinutes }}</span>
      <div v-else class="form-group">
        <input v-model="req.estimationMinutes" type="number" />
        <span v-if="errors.estimationMinutes" class="error">{{ errors.estimationMinutes }}</span>
      </div>
    </div>

    <p>Created at: {{ issue.createdAt }}</p>
    <p>Updated at: {{ issue.updatedAt }}</p>

    <div v-if="issue.parentIssue">
      Parent issue:
      <RouterLink :to="{ name: 'Issue', params: { issueKey: issue.parentIssue.key } }">
        {{ issue.parentIssue.key }} {{ issue.parentIssue.title }}
      </RouterLink>
    </div>

    <div v-if="issue.childIssues.length">
      Child Issues:
      <ul>
        <li v-for="childIssue of issue.childIssues" :key="childIssue.id">
          <RouterLink :to="{ name: 'Issue', params: { issueKey: childIssue.key } }">
            {{ childIssue.key }} {{ childIssue.title }}
          </RouterLink>
        </li>
      </ul>
    </div>

    <label>Watchers:</label>
    <ul>
      <li v-for="watcher in issue.watchers" :key="watcher.id">
        <RouterLink :to="{ name: 'User', params: { userId: watcher.id } }">
          {{ watcher.name }}
        </RouterLink>
        <button v-if="canEditIssue" @click="onRemoveWatcher(watcher.id)">X</button>
      </li>
    </ul>

    <button v-if="canEditIssue" @click="onDeleteIssue">Delete</button>
    <button v-if="!isEditing && canEditIssue" @click="onEditing">Edit</button>
    <button v-if="isEditing" @click="onUpdateIssue" :disabled="isSubmitting">Save</button>
    <button v-if="isEditing" @click="isEditing = false">Cancel</button>

    <div>
      <button v-if="!isAddingWatcher && canEditIssue" @click="onAddingWatcher">Add watcher</button>

      <div v-if="isAddingWatcher">
        <select v-model="selectedWatcherId">
          <option disabled :value="null">Select a user</option>
          <option v-for="user in nonWatcherUsers" :key="user.id" :value="user.id">
            {{ user.name }}
          </option>
        </select>
        <button @click="onAddWatcher" :disabled="!selectedWatcherId || isSubmitting">Add</button>
        <button @click="isAddingWatcher = false">Cancel</button>
      </div>
    </div>

    <div>
      <label>Attachments:</label>
      <ul>
        <li v-for="attachment in issue.attachments" :key="attachment.id">
          {{ attachment.name }}
        </li>
      </ul>
    </div>

    <div v-if="canEditIssue">
      <p>Upload attachments</p>
      <input type="file" multiple @change="(e) => onFilesSelected(e)" :disabled="isSubmitting" />
    </div>
  </div>
</template>
<style scoped>
.issue {
  padding: 1rem;
}

.form-group {
  margin-bottom: 1rem;
  display: flex;
  flex-direction: column;
}

.error {
  color: red;
  font-size: 0.8rem;
}
</style>
