<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useIssuesStore } from '@/stores/issuesStore'
import { useProjectsStore } from '@/stores/projectsStore'
import { useAuth } from '@/auth/useAuth'
import { CreateIssueRequest, IssuePriority, IssueType } from '@/types/issues'
import { ApiError, type ValidationErrors } from '@/types/api'
import { Role } from '@/types/roles'
import { applyErrorsFromApi, createDefaultErrors, getEnumOptions, removeNonDigits } from '@/utils'
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

const req = reactive(new CreateIssueRequest())
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

watch(projectId, async () => {
  isCreating.value = false
})
</script>
<template>
  <ControlButton v-if="!isCreating && canCreateIssue" @click="onCreating" label="New issue" />

  <div v-if="isCreating" class="new-issue-wrapper">
    <InputProperty label="Title" :error="errors.title">
      <input v-model="req.title" class="text-input" />
    </InputProperty>

    <InputProperty label="Description" :error="errors.description">
      <textarea v-model="req.description" class="text-input"></textarea>
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

    <InputProperty label="Estimation (minutes)" :error="errors.estimationMinutes">
      <input v-model="req.estimationMinutes" type="text" @input="removeNonDigits" />
    </InputProperty>

    <InputErrors :error="errors.general" />

    <div>
      <ControlButton @click="onSubmit" label="Create" type="primary" />
      <ControlButton @click="isCreating = false" label="Cancel" />
    </div>
  </div>
</template>
<style scoped>
.new-issue-wrapper {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.text-input {
  width: 500px;
}
</style>
