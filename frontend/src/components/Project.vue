<script setup lang="ts">
import { ref, watchEffect } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/api'
import { useProjectsStore } from '@/stores/projects'
import type { ProjectDto } from '@/types'

const router = useRouter()

const props = defineProps<{ projectId?: string }>()

const projectsStore = useProjectsStore()

const project = ref<ProjectDto>()

const onDeleteProject = async (projectId: string) => {
  router.push({ name: 'Home' })

  await projectsStore.deleteProject(projectId)
}

watchEffect(async () => {
  if (!props.projectId) return

  project.value = await api.getProject(props.projectId)
})
</script>
<template>
  <div v-if="project" class="project">
    <h2>{{ project.name }}</h2>
    <p>{{ project.description }}</p>
    <p>{{ project.ownerId }}</p>
    <p>{{ project.createdOn }}</p>
    <button @click="onDeleteProject(project.id)">Delete</button>
  </div>
</template>
<style scoped>
.project {
  padding: 1rem;
}
</style>
