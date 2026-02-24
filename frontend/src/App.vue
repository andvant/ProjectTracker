<script setup lang="ts">
import { ref } from 'vue'
import Sidebar from '@/components/Sidebar.vue'
import MainPanel from '@/components/MainPanel.vue'

const selectedProjectId = ref<string | null>(null)
</script>

<template>
  <div class="app">
    <Sidebar @select-project="selectedProjectId = $event" />
    <MainPanel :projectId="selectedProjectId" />
  </div>
</template>

<style scoped>
.app {
  display: flex;
  height: 100vh;
  width: 100%;
  font-family: sans-serif;
}
</style>
