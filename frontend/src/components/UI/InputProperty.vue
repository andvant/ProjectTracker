<script setup lang="ts">
import InputErrors from '@/components/UI/InputErrors.vue'

defineProps<{
  label: string
  subtitle?: string
  error?: string
}>()
</script>
<template>
  <div class="wrapper">
    <label>
      <div class="label">{{ label }}</div>
      <div>
        <slot></slot>
      </div>
      <div v-if="subtitle" class="subtitle">{{ subtitle }}</div>
    </label>
    <InputErrors :error="error" />
  </div>
</template>
<style scoped>
.wrapper {
  display: flex;
  flex-direction: column;
}

.label {
  font-size: 0.95rem;
  font-weight: 600;
  margin-bottom: 0.2rem;
}

.subtitle {
  color: var(--color-text-muted);
  font-size: 0.85rem;
  font-weight: 350;
}
</style>
