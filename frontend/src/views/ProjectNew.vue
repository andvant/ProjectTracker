<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useProjectsStore } from '@/stores/projects'
import { useIssuesStore } from '@/stores/issues'
import { CreateProjectRequest } from '@/types'
import { ApiError, type ValidationErrors } from '@/types/api'
import { applyErrorsFromApi } from '@/utils'

const router = useRouter()

const projectsStore = useProjectsStore()
const issuesStore = useIssuesStore()

const req = new CreateProjectRequest()
const isSubmitting = ref(false)

type Errors = ValidationErrors<CreateProjectRequest>

const createDefaultErrors = (): Errors => ({
  key: '',
  name: '',
  description: '',
  general: '',
})

const errors = ref<Errors>(createDefaultErrors())

const validate = () => {
  errors.value = createDefaultErrors()

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
  <div class="project-new">
    <h2>New project</h2>

    <div class="form-group">
      <label>Project Key</label>
      <input v-model="req.key" />
      <span v-if="errors.key" class="error">{{ errors.key }}</span>
    </div>

    <div class="form-group">
      <label>Project Name</label>
      <input v-model="req.name" />
      <span v-if="errors.name" class="error">{{ errors.name }}</span>
    </div>

    <div class="form-group">
      <label>Description</label>
      <textarea v-model="req.description"></textarea>
      <span v-if="errors.description" class="error">{{ errors.description }}</span>
    </div>

    <div v-if="errors.general" class="error">
      {{ errors.general }}
    </div>

    <button @click="onSubmit" :disabled="isSubmitting">Create</button>
  </div>
</template>
<style scoped>
.project-new {
  padding: 1rem;
  max-width: 500px;
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
