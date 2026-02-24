<script setup lang="ts">
import { ref, watch } from 'vue'
import { fetchIssues } from '@/api'
import type { Issue } from '@/api'

const issues = ref<Issue[]>([])

const props = defineProps<{
  projectId: string | null
}>()

watch(
  () => props.projectId,
  async (projectId) => {
    issues.value = projectId ? await fetchIssues(projectId) : []
  },
)
</script>
<template>
  <main class="main-panel">
    <ul>
      <li v-for="issue in issues" :key="issue.id">{{ issue.title }}</li>
    </ul>
  </main>
</template>
<style scoped>
.main-panel {
  flex: 1;
  background-color: white;
  padding: 1rem;
  overflow-y: auto;
}

.main-panel ul {
  list-style: none;
  margin: 0;
  padding: 0;
}

.main-panel li {
  padding: 0.5rem 0;
  border-bottom: 1px solid #eee;
}
</style>
