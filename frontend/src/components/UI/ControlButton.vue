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
  margin: 0 0.5rem 0 0;
  padding: 0.35rem 0.75rem;
  font-size: 0.9rem;
  border-radius: 6px;
  border: 1px solid transparent;
  cursor: pointer;
  align-self: flex-start;
}

.btn-remove {
  margin: 0 0.5rem;
  padding: 0.2rem 0.4rem;
  font-size: 0.7rem;
  border-color: var(--color-dark-grey);
  border-radius: 20px;
  background-color: white;
  color: var(--color-red);
}

.btn-remove:hover {
  background-color: var(--color-red);
  color: white;
  border-color: var(--color-red);
}

.btn-danger {
  background-color: white;
  color: var(--color-red);
  border-color: var(--color-dark-grey);
}

.btn-danger:hover {
  background-color: var(--color-red);
  color: white;
  border-color: var(--color-red);
}

.btn-primary {
  background-color: #1868db;
  color: white;
}

.btn-primary:hover {
  background-color: #1558bc;
}

.btn-default {
  background-color: white;
  color: #333;
  border-color: var(--color-dark-grey);
}

.btn-default:hover {
  background-color: #f2f2f2;
}

.btn:disabled {
  background-color: #aaa;
  border-color: #aaa;
  color: white;
}
</style>
