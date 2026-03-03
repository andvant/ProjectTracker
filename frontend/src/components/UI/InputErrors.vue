<script setup lang="ts">
defineProps<{
  error?: string
}>()
</script>
<template>
  <div v-if="error" class="error">{{ error }}</div>
</template>
<style scoped>
.error {
  color: #f22;
  font-size: 0.85rem;
  font-weight: 350;
}
</style>
