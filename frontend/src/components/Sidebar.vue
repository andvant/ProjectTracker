<script setup lang="ts">
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import type { ProjectsDto } from '@/types'

const route = useRoute()
const router = useRouter()

defineProps<{
  projects: ProjectsDto[]
}>()

const selectedProjectKey = computed(() => route.params.projectKey)

const usersSelected = computed(() => route.name === 'Users')

const onSelectProject = (projectKey: string) => {
  router.push({ name: 'Project', params: { projectKey } })
}

const onClickUsers = () => {
  router.push({ name: 'Users' })
}
</script>
<template>
  <aside class="sidebar">
    <div>
      <ul>
        <li
          v-for="project in projects"
          :key="project.id"
          @click="onSelectProject(project.key)"
          class="button"
          :class="{ selected: project.key === selectedProjectKey }"
        >
          {{ project.name }}
        </li>
      </ul>
    </div>
    <div class="users">
      <p @click="onClickUsers" class="users button" :class="{ selected: usersSelected }">Users</p>
    </div>
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

.button {
  color: white;
  background: transparent;
  border: none;
  font-size: 1.2rem;
  cursor: pointer;
  margin: 0;
  padding: 0.5rem 0;
  border-bottom: 1px solid #eee;

  &.selected {
    background-color: rgba(255, 255, 255, 0.2);
    font-weight: bold;
  }
}

.users {
  margin-top: auto;
}

.sidebar ul {
  list-style: none;
  margin: 0;
  padding: 0;
}
</style>
