<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useIssuesStore } from '@/stores/issues'
import type { CreateIssueRequest, IssueTypeEnum, IssuePriorityEnum } from '@/types'
import { IssuePriority, IssueType } from '@/types'
import { ApiError, type ValidationErrors } from '@/types/api'
import { useProjectsStore } from '@/stores/projects'
import { applyErrorsFromApi, getEnumOptions } from '@/utils'

const route = useRoute()
const router = useRouter()

const issuesStore = useIssuesStore()
const projectsStore = useProjectsStore()

const projectId = computed(() => projectsStore.getProjectIdByKey(route.params.projectKey as string))

const memberUsers = computed(() => projectsStore.getCachedProject()!.members)

const epicIssues = computed(() => issuesStore.issues.filter(i => i.type === IssueType.Epic.value))

const onSelectIssue = (issueKey: string) => {
  router.push({ name: 'Issue', params: { issueKey } })
}

const title = ref('')
const description = ref<string | undefined>()
const assigneeId = ref<string | undefined>()
const type = ref<IssueTypeEnum | undefined>()
const priority = ref<IssuePriorityEnum | undefined>()
const parentIssueId = ref<string | undefined>()
const dueDate = ref<Date | undefined>()
const estimationMinutes = ref<number | undefined>()

const isCreating = ref(false)
const isSubmitting = ref(false)

type Errors = ValidationErrors<CreateIssueRequest>

const createDefaultErrors = () => ({
  title: '',
  description: '',
  assigneeId: '',
  type: '',
  priority: '',
  parentIssueId: '',
  dueDate: '',
  estimationMinutes: '',
  general: '',
})

const errors = ref<Errors>(createDefaultErrors())

const validate = () => {
  errors.value = createDefaultErrors()

  let isValid = true

  if (!title.value.trim()) {
    errors.value.title = 'Title is required'
    isValid = false
  }

  if (estimationMinutes.value && estimationMinutes.value < 0) {
    errors.value.estimationMinutes = 'Estimation minutes cannot be negative'
    isValid = false
  }

  return isValid
}

const onSubmit = async () => {
  if (!validate()) return

  try {
    isSubmitting.value = true

    await issuesStore.createIssue(projectId.value!, {
      title: title.value,
      description: description.value,
      assigneeId: assigneeId.value,
      type: type.value,
      priority: priority.value,
      parentIssueId: parentIssueId.value,
      dueDate: dueDate.value,
      estimationMinutes: estimationMinutes.value,
    })

    isCreating.value = false
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

const onCreating = () => {
  title.value = ''
  description.value = undefined
  assigneeId.value = undefined
  type.value = undefined
  priority.value = undefined
  parentIssueId.value = undefined
  dueDate.value = undefined
  estimationMinutes.value = undefined
  isCreating.value = true
}

watch(projectId, () => {
    isCreating.value = false
  }
)
</script>
<template>
  <div>
    <button v-if="!isCreating" @click="onCreating">New</button>
    <div v-else>
      <div class="form-group">
        <label>Title</label>
        <input v-model="title" />
        <span v-if="errors.title" class="error">{{ errors.title }}</span>
      </div>

      <div class="form-group">
        <label>Description</label>
        <textarea v-model="description"></textarea>
        <span v-if="errors.description" class="error">{{ errors.description }}</span>
      </div>

      <div class="form-group">
        <label>Type</label>
        <select v-model="type">
          <option :value="undefined">{{ "<Not selected>" }}</option>
          <option
            v-for="option in getEnumOptions(IssueType)"
            :key="option.value"
            :value="option.value"
          >
            {{ option.label }}
          </option>
        </select>
        <span v-if="errors.type" class="error">{{ errors.type }}</span>
      </div>

      <div class="form-group">
        <label>Priority</label>
        <select v-model="priority">
          <option :value="undefined">{{ "<Not selected>" }}</option>
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

      <div class="form-group">
        <label>Assignee</label>
        <select v-model="assigneeId">
          <option :value="undefined">{{ "<Not selected>" }}</option>
          <option v-for="user in memberUsers" :key="user.id" :value="user.id">
            {{ user.name }}
          </option>
        </select>
        <span v-if="errors.assigneeId" class="error">{{ errors.assigneeId }}</span>
      </div>

      <div class="form-group">
        <label>Parent issue</label>
        <select v-model="parentIssueId">
          <option :value="undefined">{{ "<Not selected>" }}</option>
          <option v-for="issue in epicIssues" :key="issue.id" :value="issue.id">
            {{ issue.title }}
          </option>
        </select>
        <span v-if="errors.parentIssueId" class="error">{{ errors.parentIssueId }}</span>
      </div>

      <div class="form-group">
        <label>Estimation minutes</label>
        <input v-model="estimationMinutes" type="number" />
        <span v-if="errors.estimationMinutes" class="error">{{ errors.estimationMinutes }}</span>
      </div>

      <div class="form-group">
        <label>Due date</label>
        <input v-model="dueDate" type="date" />
        <span v-if="errors.dueDate" class="error">{{ errors.dueDate }}</span>
      </div>

      <button @click="onSubmit">Create</button>
      <button @click="isCreating = false">Cancel</button>
    </div>
  </div>

  <div class="issues">
    <ul>
      <li v-for="issue in issuesStore.issues" :key="issue.id" @click="onSelectIssue(issue.key)">
        {{ issue.title }}
      </li>
    </ul>
  </div>
</template>
<style scoped>
.form-group {
  margin-bottom: 1rem;
  display: flex;
  flex-direction: column;
}

.error {
  color: red;
  font-size: 0.8rem;
}

.issues {
  flex: 1;
  background-color: white;
  padding: 1rem;
  overflow-y: auto;
}

.issues ul {
  list-style: none;
  margin: 0;
  padding: 0;
}

.issues li {
  padding: 0.5rem 0;
  border-bottom: 1px solid #eee;
}
</style>
