<script setup lang="ts">
import { onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useProjectsStore } from '@/stores/projects'
import { useIssuesStore } from '@/stores/issues'
import Sidebar from '@/layouts/Sidebar.vue'
import MainPanel from '@/layouts/MainPanel.vue'

const route = useRoute()

const projectsStore = useProjectsStore()
const issuesStore = useIssuesStore()

const updateSelectedProject = async (projectKey?: string) => {
  if (!projectKey || !projectsStore.projects.length) return

  const projectId = projectsStore.getProjectIdByKey(projectKey)!

  await issuesStore.fetchIssues(projectId)
}

onMounted(async () => {
  await projectsStore.fetchProjects()

  await updateSelectedProject(route.params.projectKey as string)
})

watch(
  () => route.params.projectKey,
  async (projectKey) => {
    await updateSelectedProject(projectKey as string)
  },
)
</script>
<template>
  <div class="app">
    <Sidebar />
    <MainPanel>
      <router-view />
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
