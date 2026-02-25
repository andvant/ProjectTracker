<script setup lang="ts">
import { ref, onMounted, watchEffect } from 'vue'
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

onMounted(async () => {
  projects.value = await getProjects()
})

watchEffect(async () => {
  const projectKey = route.params.projectKey as string

  if (projectKey) {
    selectedProjectId.value = projects.value.filter((p) => p.key === projectKey)[0]!.id

    issues.value = await getIssues(selectedProjectId.value)
  }

  const issueKey = route.params.issueKey as string

  if (issueKey) {
    selectedIssueId.value = issues.value.filter((i) => i.key === issueKey)[0]?.id
  }
})
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
