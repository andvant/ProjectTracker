<script setup lang="ts">
import { ref, watchEffect } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '@/api'
import { useIssuesStore } from '@/stores/issues'
import type { IssueDto } from '@/types'

const props = defineProps<{
  projectId?: string
  issueId?: string
}>()

const route = useRoute()
const router = useRouter()

const issue = ref<IssueDto>()

const issuesStore = useIssuesStore()

const onDeleteIssue = async (issueId: string) => {
  router.push({ name: 'Project', params: { projectKey: route.params.projectKey } })

  await issuesStore.deleteIssue(props.projectId!, issueId)
}

watchEffect(async () => {
  if (!props.projectId || !props.issueId) return

  issue.value = await api.getIssue(props.projectId, props.issueId)
})
</script>
<template>
  <div v-if="issue" class="issue">
    <h2>{{ issue.title }}</h2>
    <p>{{ issue.description }}</p>
    <p>{{ issue.reporterId }}</p>
    <p>{{ issue.assigneeId }}</p>
    <p>{{ issue.createdOn }}</p>
    <button @click="onDeleteIssue(issue.id)">Delete</button>
  </div>
</template>
<style scoped>
.issue {
  padding: 1rem;
}
</style>
