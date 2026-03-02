<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useIssuesStore } from '@/stores/issues'
import { useProjectsStore } from '@/stores/projects'
import { useAuth } from '@/auth/useAuth'
import { CreateIssueRequest, IssuePriority, IssueType, IssueStatus } from '@/types/issues'
import { ApiError, type ValidationErrors } from '@/types/api'
import { Role } from '@/types/roles'
import { applyErrorsFromApi, createDefaultErrors, getEnumLabel, getEnumOptions } from '@/utils'
import InputProperty from '@/components/UI/InputProperty.vue'
import InputErrors from '@/components/UI/InputErrors.vue'
import ControlButton from '@/components/UI/ControlButton.vue'

const route = useRoute()

const issuesStore = useIssuesStore()
const projectsStore = useProjectsStore()

const { hasRole, userId } = useAuth()

const projectId = computed(() => projectsStore.getProjectIdByKey(route.params.projectKey as string))

const memberUsers = computed(() => projectsStore.cachedProject!.members)

const epicIssues = computed(() => issuesStore.issues.filter((i) => i.type === IssueType.Epic.value))

const canCreateIssue = computed(
  () => hasRole(Role.Admin) || memberUsers.value.map((m) => m.id).includes(userId.value!),
)

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
  <ControlButton v-if="!isCreating && canCreateIssue" @click="onCreating" label="New" />

  <div v-if="isCreating">
    <InputProperty label="Title" :error="errors.title">
      <input v-model="req.title" />
    </InputProperty>

    <InputProperty label="Description" :error="errors.description">
      <textarea v-model="req.description"></textarea>
    </InputProperty>

    <InputProperty label="Type" :error="errors.type">
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
    </InputProperty>

    <InputProperty label="Priority" :error="errors.priority">
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
    </InputProperty>

    <InputProperty label="Assignee" :error="errors.assigneeId">
      <select v-model="req.assigneeId">
        <option :value="undefined">{{ '&lt;Not selected&gt;' }}</option>
        <option v-for="user in memberUsers" :key="user.id" :value="user.id">
          {{ user.name }}
        </option>
      </select>
    </InputProperty>

    <InputProperty label="Parent issue" :error="errors.parentIssueId">
      <select v-model="req.parentIssueId">
        <option :value="undefined">{{ '&lt;Not selected&gt;' }}</option>
        <option v-for="issue in epicIssues" :key="issue.id" :value="issue.id">
          {{ issue.title }}
        </option>
      </select>
    </InputProperty>

    <InputProperty label="Due date" :error="errors.dueDate">
      <input v-model="req.dueDate" type="date" />
    </InputProperty>

    <InputProperty label="Estimation minutes" :error="errors.estimationMinutes">
      <input v-model="req.estimationMinutes" type="number" />
    </InputProperty>

    <InputErrors :error="errors.general" />

    <ControlButton @click="onSubmit" label="Create" />
    <ControlButton @click="isCreating = false" label="Cancel" />
  </div>

  <div class="issues">
    <ul>
      <li v-for="issue in issuesStore.issues" :key="issue.id">
        <RouterLink
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
        </RouterLink>
      </li>
    </ul>
  </div>
</template>
<style scoped>
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
