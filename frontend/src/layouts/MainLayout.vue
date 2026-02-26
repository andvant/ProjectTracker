<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useProjectsStore } from '@/stores/projects'
import { useIssuesStore } from '@/stores/issues'
import Sidebar from '@/components/Sidebar.vue'
import MainPanel from '@/components/MainPanel.vue'

const route = useRoute()

const projectsStore = useProjectsStore()
const issuesStore = useIssuesStore()

const selectedProjectId = ref<string | null>()
const selectedIssueId = ref<string | null>()

const updateSelectedProject = async (projectKey?: string) => {
  if (!projectKey || !projectsStore.projects.length) return

  selectedProjectId.value = projectsStore.projects.find((p) => p.key === projectKey)!.id

  await issuesStore.fetchIssues(selectedProjectId.value)
}

const updateSelectedIssue = (issueKey?: string) => {
  if (!issueKey || !issuesStore.issues.length) return

  selectedIssueId.value = issuesStore.issues.find((i) => i.key === issueKey)!.id
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
      <router-view :projectId="selectedProjectId" :issueId="selectedIssueId" />
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
