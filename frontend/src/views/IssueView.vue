<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useIssuesStore } from '@/stores/issuesStore'
import { useProjectsStore } from '@/stores/projectsStore'
import { useUsersStore } from '@/stores/usersStore'
import issuesApi from '@/api/issuesApi'
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
import {
  applyErrorsFromApi,
  createDefaultErrors,
  formatDate,
  getEnumLabel,
  getEnumOptions,
  minutesToTimespan,
  timespanToMinutes,
  getHoursFromTotalMinutes,
  getMinutesFromTotalMinutes,
  removeNonDigits,
} from '@/utils'
import Comments from '@/components/Comments.vue'
import ConfirmModal from '@/components/ConfirmModal.vue'
import ViewTitle from '@/components/UI/ViewTitle.vue'
import LabelProperty from '@/components/UI/LabelProperty.vue'
import LabelInput from '@/components/UI/LabelInput.vue'
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
const showConfirmModal = ref(false)

const req = reactive(new UpdateIssueRequest())
const estimationHours = ref('')
const estimationMinutes = ref('')
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

  return isValid
}

const onUpdateIssue = async () => {
  if (!validate()) return

  try {
    isSubmitting.value = true

    req.dueDate ||= undefined
    req.estimationMinutes = timespanToMinutes(estimationHours.value, estimationMinutes.value)
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
  estimationHours.value = getHoursFromTotalMinutes(issue.value!.estimationMinutes)
  estimationMinutes.value = getMinutesFromTotalMinutes(issue.value!.estimationMinutes)

  isEditing.value = true
}

const onDeleteIssue = async () => {
  router.push({ name: 'Project', params: { projectKey: route.params.projectKey } })

  await issuesStore.deleteIssue(projectId.value!, issueId.value!)
  showConfirmModal.value = false
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

const fileInputRef = ref<HTMLInputElement>()

const openFileDialog = () => fileInputRef.value!.click()

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

const onRemoveAttachment = async (attachmentId: string) => {
  await issuesStore.removeAttachment(projectId.value!, issue.value!, attachmentId)
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
  <div v-if="issue" class="issue-wrapper">
    <div class="issue-column">
      <ViewTitle v-if="!isEditing" :title="issue.title" :subtitle="issue.key" />

      <LabelInput v-if="isEditing" label="Title" :error="errors.title">
        <input v-model="req.title" class="text-input" />
      </LabelInput>

      <LabelProperty v-if="!isEditing" label="Description">{{ issue.description }}</LabelProperty>

      <LabelInput v-if="isEditing" label="Description" :error="errors.description">
        <textarea v-model="req.description" class="text-input"></textarea>
      </LabelInput>

      <InputErrors v-if="isEditing" :error="errors.general" />

      <div>
        <ControlButton v-if="!isEditing && canEditIssue" @click="onEditing" label="Edit" />
        <ControlButton
          v-if="isEditing"
          @click="onUpdateIssue"
          :disabled="isSubmitting"
          label="Save"
          type="primary"
        />
        <ControlButton
          v-if="isEditing"
          @click="isEditing = false"
          :disabled="isSubmitting"
          label="Cancel"
        />
        <ControlButton
          v-if="canEditIssue"
          @click="showConfirmModal = true"
          :disabled="isSubmitting"
          label="Delete issue"
          type="danger"
        />
      </div>

      <LabelProperty v-if="issue.parentIssue" label="Parent issue">
        <RouterLink :to="{ name: 'Issue', params: { issueKey: issue.parentIssue.key } }">
          {{ issue.parentIssue.key }} {{ issue.parentIssue.title }}
        </RouterLink>
      </LabelProperty>

      <LabelProperty v-if="issue.childIssues.length" label="Child issues">
        <ul class="list">
          <li v-for="childIssue of issue.childIssues" :key="childIssue.id">
            <RouterLink :to="{ name: 'Issue', params: { issueKey: childIssue.key } }">
              {{ childIssue.key }} {{ childIssue.title }}
            </RouterLink>
          </li>
        </ul>
      </LabelProperty>

      <LabelProperty label="Attachments">
        <ul v-if="issue.attachments.length" class="list">
          <li v-for="attachment in issue.attachments" :key="attachment.id">
            <a :href="issuesApi.getDownloadAttachmentLink(projectId!, issueId!, attachment.id)">
              {{ attachment.name }}
            </a>
            <ControlButton
              v-if="canEditIssue"
              @click="onRemoveAttachment(attachment.id)"
              type="remove"
            />
          </li>
        </ul>
      </LabelProperty>

      <div v-if="canEditIssue">
        <input
          ref="fileInputRef"
          type="file"
          multiple
          @change="(e) => onFilesSelected(e)"
          style="display: none"
        />
        <ControlButton @click="openFileDialog" :disabled="isSubmitting" label="Add attachments" />
      </div>

      <Comments
        v-model="issue"
        :projectId="projectId!"
        :issueId="issueId!"
        :memberUsers="memberUsers"
      />
    </div>

    <div class="issue-column">
      <LabelProperty label="Project">
        <RouterLink
          :to="{ name: 'Project', params: { projectKey: projectsStore.cachedProject?.key } }"
        >
          {{ projectsStore.cachedProject?.name }}
        </RouterLink>
      </LabelProperty>

      <LabelProperty label="Reporter">
        <RouterLink :to="{ name: 'User', params: { userId: issue.reporter.id } }">
          {{ issue.reporter.name }}
        </RouterLink>
      </LabelProperty>

      <LabelProperty v-if="!isEditing" label="Assignee">
        <span v-if="!issue.assignee" class="unassigned">Unassigned</span>
        <RouterLink v-else :to="{ name: 'User', params: { userId: issue.assignee.id } }">
          {{ issue.assignee.name }}
        </RouterLink>
      </LabelProperty>

      <LabelInput v-if="isEditing" label="Assignee" :error="errors.assigneeId">
        <select v-model="req.assigneeId">
          <option :value="undefined">Unassigned</option>
          <option v-for="user in memberUsers" :key="user.id" :value="user.id">
            {{ user.name }}
          </option>
        </select>
      </LabelInput>

      <LabelProperty label="Type">{{ getEnumLabel(IssueType, issue.type) }}</LabelProperty>

      <LabelProperty v-if="!isEditing" label="Status">
        {{ getEnumLabel(IssueStatus, issue.status) }}
      </LabelProperty>

      <LabelInput v-if="isEditing" label="Status" :error="errors.status">
        <select v-model="req.status">
          <option
            v-for="option in getEnumOptions(IssueStatus)"
            :key="option.value"
            :value="option.value"
          >
            {{ option.label }}
          </option>
        </select>
      </LabelInput>

      <LabelProperty v-if="!isEditing" label="Priority">
        {{ getEnumLabel(IssuePriority, issue.priority) }}
      </LabelProperty>

      <LabelInput v-if="isEditing" label="Priority" :error="errors.priority">
        <select v-model="req.priority">
          <option
            v-for="option in getEnumOptions(IssuePriority)"
            :key="option.value"
            :value="option.value"
          >
            {{ option.label }}
          </option>
        </select>
      </LabelInput>

      <LabelProperty v-if="!isEditing" label="Due date">
        {{ formatDate(issue.dueDate, false) }}
      </LabelProperty>

      <LabelInput v-if="isEditing" label="Due date" :error="errors.dueDate">
        <input v-model="req.dueDate" type="date" />
      </LabelInput>

      <LabelProperty v-if="!isEditing" label="Estimation">
        {{ minutesToTimespan(issue.estimationMinutes) }}
      </LabelProperty>

      <LabelInput v-if="isEditing" label="Estimation" :error="errors.estimationMinutes">
        <div class="estimation">
          <input
            v-model="estimationHours"
            type="text"
            @input="estimationHours = removeNonDigits(estimationHours)"
            maxlength="3"
          />
          <span>:</span>
          <input
            v-model="estimationMinutes"
            type="text"
            @input="estimationMinutes = removeNonDigits(estimationMinutes)"
            maxlength="2"
          />
        </div>
      </LabelInput>

      <LabelProperty label="Created">{{ formatDate(issue.createdAt) }}</LabelProperty>
      <LabelProperty label="Updated">{{ formatDate(issue.updatedAt) }}</LabelProperty>

      <LabelProperty label="Watchers">
        <ul v-if="issue.watchers.length" class="list">
          <li v-for="watcher in issue.watchers" :key="watcher.id">
            <RouterLink :to="{ name: 'User', params: { userId: watcher.id } }">
              {{ watcher.name }}
            </RouterLink>
            <ControlButton v-if="canEditIssue" @click="onRemoveWatcher(watcher.id)" type="remove" />
          </li>
        </ul>
      </LabelProperty>

      <ControlButton
        v-if="!isAddingWatcher && canEditIssue"
        @click="onAddingWatcher"
        label="Add watcher"
      />

      <div v-if="isAddingWatcher" class="user-select">
        <select v-model="selectedWatcherId">
          <option disabled :value="null">Select a user</option>
          <option v-for="user in nonWatcherUsers" :key="user.id" :value="user.id">
            {{ user.name }}
          </option>
        </select>
        <div>
          <ControlButton
            @click="onAddWatcher"
            :disabled="!selectedWatcherId || isSubmitting"
            label="Add"
            type="primary"
          />
          <ControlButton @click="isAddingWatcher = false" label="Cancel" />
        </div>
      </div>
    </div>

    <ConfirmModal
      :show="showConfirmModal"
      :title="`Delete issue ${issue!.key}`"
      @cancel="showConfirmModal = false"
      @confirm="onDeleteIssue"
    />
  </div>
</template>
<style scoped>
.issue-wrapper {
  padding: 2rem;
  display: grid;
  grid-template-columns: 3fr 1fr;
  align-items: start;
  width: 70vw;
}

.issue-column {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.user-select {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.unassigned {
  color: var(--color-text-muted);
}

.text-input {
  width: 500px;
}

.estimation {
  display: flex;
  gap: 0.3rem;
  align-items: center;
}

.estimation input {
  width: 1.6rem;
}
</style>
