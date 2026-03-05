<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import { useIssuesStore } from '@/stores/issuesStore'
import { useAuth } from '@/auth/useAuth'
import { CreateIssueRequest, IssuePriority, IssueType } from '@/types/issues'
import type { UsersDto } from '@/types/users'
import { ApiError, type ValidationErrors } from '@/types/api'
import { Role } from '@/types/roles'
import {
  applyErrorsFromApi,
  createDefaultErrors,
  getEnumOptions,
  removeNonDigits,
  timespanToMinutes,
} from '@/utils'
import LabelInput from '@/components/UI/LabelInput.vue'
import InputErrors from '@/components/UI/InputErrors.vue'
import ControlButton from '@/components/UI/ControlButton.vue'

const props = defineProps<{
  projectId: string
  memberUsers: UsersDto[]
}>()

const issuesStore = useIssuesStore()

const { hasRole, userId } = useAuth()

const epicIssues = computed(() => issuesStore.issues.filter((i) => i.type === IssueType.Epic.value))

const canCreateIssue = computed(
  () => hasRole(Role.Admin) || props.memberUsers.map((m) => m.id).includes(userId.value!),
)

const req = reactive(new CreateIssueRequest())
const estimationHours = ref('')
const estimationMinutes = ref('')
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

  return isValid
}

const onSubmit = async () => {
  if (!validate()) return

  try {
    isSubmitting.value = true

    req.estimationMinutes = timespanToMinutes(estimationHours.value, estimationMinutes.value)
    await issuesStore.createIssue(props.projectId, req)

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
  () => props.projectId,
  () => {
    isCreating.value = false
  },
)
</script>
<template>
  <ControlButton v-if="!isCreating && canCreateIssue" @click="onCreating" label="New issue" />

  <div v-if="isCreating" class="new-issue-wrapper">
    <LabelInput label="Title" :error="errors.title">
      <input v-model="req.title" class="text-input" />
    </LabelInput>

    <LabelInput label="Description" :error="errors.description">
      <textarea v-model="req.description" class="text-input"></textarea>
    </LabelInput>

    <LabelInput label="Type" :error="errors.type">
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
    </LabelInput>

    <LabelInput label="Priority" :error="errors.priority">
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
    </LabelInput>

    <LabelInput label="Assignee" :error="errors.assigneeId">
      <select v-model="req.assigneeId">
        <option :value="undefined">{{ '&lt;Not selected&gt;' }}</option>
        <option v-for="user in memberUsers" :key="user.id" :value="user.id">
          {{ user.name }}
        </option>
      </select>
    </LabelInput>

    <LabelInput label="Parent issue" :error="errors.parentIssueId">
      <select v-model="req.parentIssueId">
        <option :value="undefined">{{ '&lt;Not selected&gt;' }}</option>
        <option v-for="issue in epicIssues" :key="issue.id" :value="issue.id">
          {{ issue.title }}
        </option>
      </select>
    </LabelInput>

    <LabelInput label="Due date" :error="errors.dueDate">
      <input v-model="req.dueDate" type="date" />
    </LabelInput>

    <LabelInput label="Estimation" :error="errors.estimationMinutes">
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

.estimation {
  display: flex;
  gap: 0.3rem;
  align-items: center;
}

.estimation input {
  width: 1.6rem;
}
</style>
