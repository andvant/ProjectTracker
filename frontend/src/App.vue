<script setup lang="ts">
import { ref, onMounted } from 'vue'

const projects = ref<{ id: number; name: string }[]>([])

async function fetchProjects() {
  try {
    const response = await fetch('http://localhost:5050/projects')

    if (!response.ok) throw new Error('Failed to fetch projects')

    projects.value = await response.json()
  } catch (err) {
    console.error(err)
  }
}

onMounted(fetchProjects)
</script>

<template>
  <div class="app">
    <aside class="sidebar">
      <button class="projects-btn">Projects</button>
    </aside>

    <main class="main-panel">
      <ul>
        <li v-for="project in projects" :key="project.id">{{ project.name }}</li>
      </ul>
    </main>
  </div>
</template>

<style scoped>
.app {
  display: flex;
  height: 100vh;
  width: 100%;
  font-family: sans-serif;
}

.sidebar {
  width: 200px;
  background-color: purple;
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
