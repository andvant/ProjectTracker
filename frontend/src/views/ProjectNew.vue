<script setup lang="ts">
import { useRouter } from 'vue-router'
import api from '@/api'
import { useProjectsStore } from '@/stores/projects'
import { useIssuesStore } from '@/stores/issues'

const router = useRouter()

const projectsStore = useProjectsStore()
const issuesStore = useIssuesStore()

const onCreate = async () => {
  const project = await api.createProject({
    key: 'project7',
    name: 'Project 7',
    description: 'description 7',
  })

  await projectsStore.fetchProjects()
  await issuesStore.clearIssues()

  router.push({ name: 'Project', params: { projectKey: project.key } })
}
</script>
<template>
  <div class="project">
    <h2>New project</h2>
    <button @click="onCreate">Create</button>
  </div>
</template>
<style scoped>
.project {
  padding: 1rem;
}
</style>
