<script setup lang="ts">
import Sidebar from '@/components/Sidebar.vue'
</script>
<template>
  <div class="app">
    <Sidebar />
    <main>
      <RouterView />
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
</style>
