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
  color: red;
  font-size: 0.8rem;
}
</style>
