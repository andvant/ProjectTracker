<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import { getProjects, getIssues } from '@/api'
import type { ProjectsDto, IssuesDto } from '@/types'
import Sidebar from '@/components/Sidebar.vue'
import MainPanel from '@/components/MainPanel.vue'

const route = useRoute()

const selectedProjectId = ref<string | null>()
const selectedIssueId = ref<string | null>()

const projects = ref<ProjectsDto[]>([])
const issues = ref<IssuesDto[]>([])

const updateSelectedProject = async (projectKey?: string) => {
  if (!projectKey || !projects.value.length) return

  selectedProjectId.value = projects.value.find((p) => p.key === projectKey)!.id

  issues.value = await getIssues(selectedProjectId.value)
}

const updateSelectedIssue = (issueKey?: string) => {
  if (!issueKey || !issues.value.length) return

  selectedIssueId.value = issues.value.find((i) => i.key === issueKey)!.id
}

onMounted(async () => {
  projects.value = await getProjects()

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
    <Sidebar :projects="projects" />
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
