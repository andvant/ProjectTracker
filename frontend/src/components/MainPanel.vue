<script setup lang="ts">
import { ref, watch } from 'vue'

interface Issue {
  id: string
  title: string
}

const issues = ref<Issue[]>([])

const props = defineProps<{
  projectId: string | null
}>()

const fetchIssues = async (projectId: string) => {
  try {
    const res = await fetch(`http://localhost:5050/projects/${projectId}/issues`)

    if (!res.ok) throw new Error('Failed to fetch issues')

    issues.value = await res.json()
  } catch (err) {
    console.error(err)
  }
}

watch(
  () => props.projectId,
  (projectId) => {
    if (projectId) {
      fetchIssues(projectId)
    } else {
      issues.value = []
    }
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
