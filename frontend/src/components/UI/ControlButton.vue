<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  label?: string
  type?: 'primary' | 'danger' | 'remove'
}>()

const cssClass = computed(() => {
  if (props.type === 'primary') return 'btn-primary'
  if (props.type === 'danger') return 'btn-danger'
  if (props.type === 'remove') return 'btn-remove'

  return 'btn-default'
})
</script>
<template>
  <button class="btn" :class="cssClass">{{ type === 'remove' ? '✖' : label }}</button>
</template>
<style scoped>
.btn {
  font-size: 0.9rem;
  padding: 0.35rem 0.75rem;
  margin: 0 0.3rem;
  border-radius: 6px;
  border: 1px solid transparent;
  cursor: pointer;
}

.btn-remove {
  background-color: #fff;
  color: var(--color-red);
  border-color: #ddd;
  border-radius: 20px;
  padding: 0.2rem 0.4rem;
  margin: 0 0.5rem;
  font-size: 0.7rem;
}

.btn-remove:hover {
  background-color: var(--color-red);
  color: #fff;
  border-color: var(--color-red);
}

.btn-danger {
  background-color: #fff;
  color: var(--color-red);
  border-color: #ddd;
}

.btn-danger:hover {
  background-color: var(--color-red);
  color: #fff;
  border-color: var(--color-red);
}

.btn-primary {
  background-color: #1868db;
  color: #fff;
}

.btn-primary:hover {
  background-color: #1558bc;
}

.btn-default {
  background-color: #fff;
  color: #333;
  border-color: #ddd;
}

.btn-default:hover {
  background-color: #f2f2f2;
}
</style>
