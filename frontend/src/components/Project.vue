<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '@/api'
import { useProjectsStore } from '@/stores/projects'
import type { ProjectDto } from '@/types'

const route = useRoute()
const router = useRouter()

const projectsStore = useProjectsStore()

const project = ref<ProjectDto>()

const projectId = computed(() => projectsStore.getProjectIdByKey(route.params.projectKey as string))

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
    <h2>{{ project.name }}</h2>
    <p>{{ project.id }}</p>
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
