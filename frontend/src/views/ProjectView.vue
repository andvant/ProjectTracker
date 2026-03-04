<script setup lang="ts">
import { computed, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useIssuesStore } from '@/stores/issues'
import { useProjectsStore } from '@/stores/projects'
import Project from '@/components/Project.vue'
import NewIssue from '@/components/NewIssue.vue'
import Issues from '@/components/Issues.vue'

const route = useRoute()

const projectsStore = useProjectsStore()
const issuesStore = useIssuesStore()

const projectId = computed(() => projectsStore.getProjectIdByKey(route.params.projectKey as string))

watch(
  projectId,
  async (projectId) => {
    if (!projectId) return

    await issuesStore.fetchIssues(projectId)
  },
  { immediate: true },
)
</script>
<template>
  <div>
    <Project />
    <NewIssue />
    <Issues />
  </div>
</template>
