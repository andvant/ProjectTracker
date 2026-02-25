<script setup lang="ts">
import { useRouter } from 'vue-router'
import type { IssuesDto } from '@/types'

const router = useRouter()

defineProps<{ projectId?: string; issues?: IssuesDto[] }>()

const onSelectIssue = (issueKey: string) => {
  router.push({ name: 'Issue', params: { issueKey } })
}
</script>
<template>
  <div class="issues">
    <ul>
      <li v-for="issue in issues" :key="issue.id" @click="onSelectIssue(issue.key)">
        {{ issue.title }}
      </li>
    </ul>
  </div>
</template>
<style scoped>
.issues {
  flex: 1;
  background-color: white;
  padding: 1rem;
  overflow-y: auto;
}

.issues ul {
  list-style: none;
  margin: 0;
  padding: 0;
}

.issues li {
  padding: 0.5rem 0;
  border-bottom: 1px solid #eee;
}
</style>
