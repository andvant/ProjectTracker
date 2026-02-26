<script setup lang="ts">
import { ref, watchEffect } from 'vue'
import api from '@/api'
import type { IssueDto } from '@/types'

const issue = ref<IssueDto>()

const props = defineProps<{
  projectId?: string
  issueId?: string
}>()

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
  </div>
</template>
<style scoped>
.issue {
  padding: 1rem;
}
</style>
