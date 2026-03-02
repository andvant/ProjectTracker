<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useIssuesStore } from '@/stores/issues'
import { useProjectsStore } from '@/stores/projects'
import { CreateIssueRequest, IssuePriority, IssueType, IssueStatus } from '@/types/issues'
import { ApiError, type ValidationErrors } from '@/types/api'
import { applyErrorsFromApi, createDefaultErrors, getEnumLabel, getEnumOptions } from '@/utils'

const route = useRoute()

const issuesStore = useIssuesStore()
const projectsStore = useProjectsStore()

const projectId = computed(() => projectsStore.getProjectIdByKey(route.params.projectKey as string))

const memberUsers = computed(() => projectsStore.cachedProject!.members)

const epicIssues = computed(() => issuesStore.issues.filter((i) => i.type === IssueType.Epic.value))

const req = new CreateIssueRequest()
const isCreating = ref(false)
const isSubmitting = ref(false)

type Errors = ValidationErrors<CreateIssueRequest>

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

const onSubmit = async () => {
  if (!validate()) return

  try {
    isSubmitting.value = true

    await issuesStore.createIssue(projectId.value!, req)

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
  errors.value = createDefaultErrors(req)
  Object.assign(req, new CreateIssueRequest())
  isCreating.value = true
}

watch(
  projectId,
  async (projectId) => {
    if (!projectId) return

    isCreating.value = false

    await issuesStore.fetchIssues(projectId)
  },
  { immediate: true },
)
</script>
<template>
  <div v-if="projectId">
    <button v-if="!isCreating" @click="onCreating">New</button>
    <div v-else>
      <div class="form-group">
        <label>Title</label>
        <input v-model="req.title" />
        <span v-if="errors.title" class="error">{{ errors.title }}</span>
      </div>

      <div class="form-group">
        <label>Description</label>
        <textarea v-model="req.description"></textarea>
        <span v-if="errors.description" class="error">{{ errors.description }}</span>
      </div>

      <div class="form-group">
        <label>Type</label>
        <select v-model="req.type">
          <option :value="undefined">{{ '&lt;Not selected&gt;' }}</option>
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
        <select v-model="req.priority">
          <option :value="undefined">{{ '&lt;Not selected&gt;' }}</option>
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
        <select v-model="req.assigneeId">
          <option :value="undefined">{{ '&lt;Not selected&gt;' }}</option>
          <option v-for="user in memberUsers" :key="user.id" :value="user.id">
            {{ user.name }}
          </option>
        </select>
        <span v-if="errors.assigneeId" class="error">{{ errors.assigneeId }}</span>
      </div>

      <div class="form-group">
        <label>Parent issue</label>
        <select v-model="req.parentIssueId">
          <option :value="undefined">{{ '&lt;Not selected&gt;' }}</option>
          <option v-for="issue in epicIssues" :key="issue.id" :value="issue.id">
            {{ issue.title }}
          </option>
        </select>
        <span v-if="errors.parentIssueId" class="error">{{ errors.parentIssueId }}</span>
      </div>

      <div class="form-group">
        <label>Due date</label>
        <input v-model="req.dueDate" type="date" />
        <span v-if="errors.dueDate" class="error">{{ errors.dueDate }}</span>
      </div>

      <div class="form-group">
        <label>Estimation minutes</label>
        <input v-model="req.estimationMinutes" type="number" />
        <span v-if="errors.estimationMinutes" class="error">{{ errors.estimationMinutes }}</span>
      </div>

      <button @click="onSubmit">Create</button>
      <button @click="isCreating = false">Cancel</button>
    </div>
  </div>

  <div class="issues">
    <ul>
      <li v-for="issue in issuesStore.issues" :key="issue.id">
        <router-link
          :to="{ name: 'Issue', params: { issueKey: issue.key } }"
          custom
          v-slot="{ navigate, href }"
        >
          <div :href="href" @click="navigate" class="issue-row">
            <span class="issue-col">{{ issue.key }}</span>
            <span class="issue-col">{{ issue.title }}</span>
            <span class="issue-col">{{ getEnumLabel(IssueStatus, issue.status) }}</span>
            <span class="issue-col">{{ getEnumLabel(IssueType, issue.type) }}</span>
            <span class="issue-col">{{ getEnumLabel(IssuePriority, issue.priority) }}</span>
          </div>
        </router-link>
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

.issue-row {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  align-items: center;

  padding: 0.5rem 0;
  border-bottom: 1px solid #eee;
  cursor: pointer;
}

.issue-row:hover {
  background-color: #ddd;
}

.issue-col {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
</style>
