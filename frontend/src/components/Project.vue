<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '@/api'
import { useProjectsStore } from '@/stores/projects'
import type { ProjectDto, UpdateProjectRequest } from '@/types'
import { ApiError, type GeneralError } from '@/types/api'
import { applyErrorsFromApi } from '@/utils'

const route = useRoute()
const router = useRouter()

const projectsStore = useProjectsStore()

const project = ref<ProjectDto>()

const projectId = computed(() => projectsStore.getProjectIdByKey(route.params.projectKey as string))

const description = ref('')
const name = ref('')
const isEditing = ref(false)
const isSubmitting = ref(false)

type Errors = UpdateProjectRequest & GeneralError

const createDefaultErrors = () => ({
  name: '',
  description: '',
  general: '',
})

const errors = ref<Errors>(createDefaultErrors())

const validate = () => {
  errors.value = createDefaultErrors()

  let isValid = true

  if (!name.value.trim()) {
    errors.value.name = 'Project name is required'
    isValid = false
  }

  return isValid
}

const onUpdateProject = async (projectId: string) => {
  if (!validate()) return

  try {
    isSubmitting.value = true

    await api.updateProject(projectId, {
      name: name.value,
      description: description.value,
    })

    project.value = await api.getProject(projectId)
    await projectsStore.fetchProjects()
  } catch (e) {
    if (e instanceof ApiError && e.problem) {
      applyErrorsFromApi(errors.value, e.problem)
    } else {
      errors.value.general = 'Unexpected error occurred'
    }
  } finally {
    isSubmitting.value = false
    isEditing.value = false
  }
}

const onEditing = () => {
  name.value = project.value!.name
  description.value = project.value!.description || ''

  isEditing.value = true
}

const onDeleteProject = async (projectId: string) => {
  router.push({ name: 'Home' })

  await projectsStore.deleteProject(projectId)
}

watch(
  projectId,
  async (projectId) => {
    if (!projectId) return

    project.value = await api.getProject(projectId)
  },
  { immediate: true },
)
</script>
<template>
  <div v-if="project" class="project">
    <h2 v-if="!isEditing">{{ project.name }}</h2>
    <input v-else v-model="name" />
    <p>{{ project.id }}</p>
    <p v-if="!isEditing">{{ project.description }}</p>
    <input v-else v-model="description" />
    <p>{{ project.ownerId }}</p>
    <p>{{ project.createdOn }}</p>
    <button @click="onDeleteProject(project.id)">Delete</button>
    <button v-if="!isEditing" @click="onEditing">Edit</button>
    <button v-if="isEditing" @click="onUpdateProject(project.id)" :disabled="isSubmitting">
      Save
    </button>
    <button v-if="isEditing" @click="isEditing = false">Cancel</button>
  </div>
</template>
<style scoped>
.project {
  padding: 1rem;
}
</style>
