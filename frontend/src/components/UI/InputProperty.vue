<script setup lang="ts">
import InputErrors from '@/components/UI/InputErrors.vue'

defineProps<{
  label: string
  error?: string
}>()
</script>
<template>
  <div class="wrapper">
    <label>
      <div>{{ label }}</div>
      <slot></slot>
    </label>
    <InputErrors :error="error" />
  </div>
</template>
<style scoped>
.wrapper {
  margin-bottom: 1rem;
  display: flex;
  flex-direction: column;
}
</style>
