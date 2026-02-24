<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { fetchProjects } from '@/api'
import type { Project } from '@/api'

const router = useRouter()

const projects = ref<Project[]>([])

const emit = defineEmits<{
  (e: 'select-project', projectId: string): void
}>()

const selectedProjectKey = ref<string | null>()

const selectProject = (id: string, key: string) => {
  emit('select-project', id)
  selectedProjectKey.value = key
  router.push(`/projects/${key}`)
}

onMounted(async () => {
  projects.value = await fetchProjects()

  const selectedProjectKey = router.currentRoute.value.params.projectKey as string

  if (selectedProjectKey) {
    const selectedProjectId = projects.value.filter((p) => p.key === selectedProjectKey)[0]!.id
    selectProject(selectedProjectId, selectedProjectKey)
  }
})
</script>
<template>
  <aside class="sidebar">
    <ul>
      <li
        v-for="project in projects"
        :key="project.id"
        @click="selectProject(project.id, project.key)"
        class="projects-btn"
        :class="{ selected: project.key === selectedProjectKey }"
      >
        {{ project.name }}
      </li>
    </ul>
  </aside>
</template>
<style scoped>
.sidebar {
  width: 200px;
  background-color: #11214a;
  padding: 1rem;
  display: flex;
  flex-direction: column;
}

.projects-btn {
  color: white;
  background: transparent;
  border: none;
  font-size: 1.2rem;
  cursor: pointer;
}

.sidebar ul {
  list-style: none;
  margin: 0;
  padding: 0;
}

.sidebar li {
  padding: 0.5rem 0;
  border-bottom: 1px solid #eee;
}

.sidebar li.selected {
  background-color: rgba(255, 255, 255, 0.2);
  font-weight: bold;
}
</style>
