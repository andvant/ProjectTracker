<script setup lang="ts">
defineProps<{
  title: string
  subtitle?: string
}>()
</script>
<template>
  <div class="view-header">
    <h1 class="view-title">{{ title }}</h1>
    <div v-if="subtitle" class="view-subtitle">{{ subtitle }}</div>
  </div>
</template>
<style scoped>
.view-header {
  margin-bottom: 0.5rem;
}

.view-title {
  margin: 0;
  font-size: 1.8rem;
  font-weight: 600;
}

.view-subtitle {
  font-size: 1rem;
  color: var(--color-text-muted);
}
</style>
