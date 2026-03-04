<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useProjectsStore } from '@/stores/projects'
import { useIssuesStore } from '@/stores/issues'
import { CreateProjectRequest } from '@/types/projects'
import { ApiError, type ValidationErrors } from '@/types/api'
import { applyErrorsFromApi, createDefaultErrors } from '@/utils'
import ViewTitle from '@/components/UI/ViewTitle.vue'
import InputProperty from '@/components/UI/InputProperty.vue'
import InputErrors from '@/components/UI/InputErrors.vue'
import ControlButton from '@/components/UI/ControlButton.vue'

const router = useRouter()

const projectsStore = useProjectsStore()
const issuesStore = useIssuesStore()

const req = new CreateProjectRequest()
const isSubmitting = ref(false)

type Errors = ValidationErrors<CreateProjectRequest>

const errors = ref<Errors>(createDefaultErrors(req))

const validate = () => {
  errors.value = createDefaultErrors(req)

  let isValid = true

  if (!req.key.trim()) {
    errors.value.key = 'Project key is required'
    isValid = false
  }

  if (!req.name.trim()) {
    errors.value.name = 'Project name is required'
    isValid = false
  }

  return isValid
}

const onSubmit = async () => {
  if (!validate()) return

  try {
    isSubmitting.value = true

    const project = await projectsStore.createProject(req)

    await issuesStore.clearIssues()

    router.push({ name: 'Project', params: { projectKey: project.key } })
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
</script>
<template>
  <div class="new-project-wrapper">
    <ViewTitle title="New project" />

    <InputProperty
      label="Project key"
      :error="errors.key"
      subtitle="Used in URLs and as a prefix in issues' keys that belong to this project"
    >
      <input v-model="req.key" class="text-input" />
    </InputProperty>

    <InputProperty label="Project name" :error="errors.name">
      <input v-model="req.name" class="text-input" />
    </InputProperty>

    <InputProperty label="Description" :error="errors.description">
      <textarea v-model="req.description" class="text-input"></textarea>
    </InputProperty>

    <InputErrors :error="errors.general" />

    <ControlButton @click="onSubmit" :disabled="isSubmitting" label="Create" type="primary" />
  </div>
</template>
<style scoped>
.new-project-wrapper {
  padding: 2rem;
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.text-input {
  width: 400px;
}
</style>
