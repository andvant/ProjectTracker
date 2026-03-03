<script setup lang="ts">
defineProps<{
  title: string
  subtitle?: string
}>()
</script>
<template>
  <div class="entity-header">
    <h1 class="entity-title">{{ title }}</h1>
    <p v-if="subtitle" class="entity-subtitle">{{ subtitle }}</p>
  </div>
</template>
<style scoped>
.entity-header {
  margin-bottom: 1.5rem;
}

.entity-title {
  font-size: 1.8rem;
  font-weight: 600;
  margin-bottom: 0;
}

.entity-subtitle {
  margin-top: 0;
  font-size: 1rem;
  color: var(--color-text-muted);
}
</style>
