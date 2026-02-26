<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import api from '@/api'
import type { IssuesDto } from '@/types'
import { useProjectsStore } from '@/stores/projects'
import Sidebar from '@/components/Sidebar.vue'
import MainPanel from '@/components/MainPanel.vue'

const route = useRoute()

const projectsStore = useProjectsStore()

const selectedProjectId = ref<string | null>()
const selectedIssueId = ref<string | null>()

const issues = ref<IssuesDto[]>([])

const updateSelectedProject = async (projectKey?: string) => {
  if (!projectKey || !projectsStore.projects.length) return

  selectedProjectId.value = projectsStore.projects.find((p) => p.key === projectKey)!.id

  issues.value = await api.getIssues(selectedProjectId.value)
}

const updateSelectedIssue = (issueKey?: string) => {
  if (!issueKey || !issues.value.length) return

  selectedIssueId.value = issues.value.find((i) => i.key === issueKey)!.id
}

onMounted(async () => {
  await projectsStore.fetchProjects()

  await updateSelectedProject(route.params.projectKey as string)
  updateSelectedIssue(route.params.issueKey as string)
})

watch(
  () => route.params.projectKey,
  async (projectKey) => {
    await updateSelectedProject(projectKey as string)
  },
)

watch(
  () => route.params.issueKey,
  (issueKey) => {
    updateSelectedIssue(issueKey as string)
  },
)
</script>
<template>
  <div class="app">
    <Sidebar />
    <MainPanel>
      <router-view :projectId="selectedProjectId" :issues="issues" :issueId="selectedIssueId" />
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
