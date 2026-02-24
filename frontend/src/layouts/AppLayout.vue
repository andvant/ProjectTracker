<script setup lang="ts">
import { ref, onMounted, watchEffect } from 'vue'
import { useRoute } from 'vue-router'
import { getProjects } from '@/api'
import type { ProjectsDto } from '@/api'
import Sidebar from '@/components/Sidebar.vue'
import MainPanel from '@/components/MainPanel.vue'

const route = useRoute()

const selectedProjectId = ref<string | null>()

const projects = ref<ProjectsDto[]>([])

onMounted(async () => {
  projects.value = await getProjects()
})

watchEffect(async () => {
  const projectKey = route.params.projectKey as string

  selectedProjectId.value =
    projectKey && projects.value.length
      ? projects.value.filter((p) => p.key === projectKey)[0]!.id
      : null
})
</script>
<template>
  <div class="app">
    <Sidebar :projects="projects" />
    <MainPanel>
      <router-view :projectId="selectedProjectId" />
    </MainPanel>
  </div>
</template>
<style scoped>
.app {
  display: flex;
  height: 100vh;
  width: 100%;
  font-family: sans-serif;
}
</style>
