<script setup lang="ts">
import { ref, watchEffect } from 'vue'
import { getIssues } from '@/api'
import type { IssuesDto } from '@/types'

const issues = ref<IssuesDto[]>([])

const props = defineProps<{ projectId?: string }>()

watchEffect(async () => {
  if (!props.projectId) return

  issues.value = await getIssues(props.projectId)
})
</script>
<template>
  <div class="issues">
    <ul>
      <li v-for="issue in issues" :key="issue.id">{{ issue.title }}</li>
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
