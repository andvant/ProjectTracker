<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '@/api'
import { useIssuesStore } from '@/stores/issues'
import type { IssueDto } from '@/types'
import { useProjectsStore } from '@/stores/projects'

const route = useRoute()
const router = useRouter()

const issue = ref<IssueDto>()

const projectsStore = useProjectsStore()
const issuesStore = useIssuesStore()

const projectId = computed(() => projectsStore.getProjectIdByKey(route.params.projectKey as string))

const issueId = computed(() => issuesStore.getIssueIdByKey(route.params.issueKey as string))

const onDeleteIssue = async () => {
  router.push({ name: 'Project', params: { projectKey: route.params.projectKey } })

  await issuesStore.deleteIssue(projectId.value!, issueId.value!)
}

watch(
  issueId,
  async (issueId) => {
    if (!issueId) return

    issue.value = await api.getIssue(projectId.value!, issueId)
  },
  { immediate: true },
)
</script>
<template>
  <div v-if="issue" class="issue">
    <h2>{{ issue.title }}</h2>
    <p>Description: {{ issue.description }}</p>
    <p>Reporter: {{ issue.reporterId }}</p>
    <p>Assignee: {{ issue.assigneeId ?? 'Unassigned' }}</p>
    <p>Created on: {{ issue.createdOn }}</p>
    <button @click="onDeleteIssue">Delete</button>
  </div>
</template>
<style scoped>
.issue {
  padding: 1rem;
}
</style>
