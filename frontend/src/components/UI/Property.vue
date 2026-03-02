<script setup lang="ts">
import { computed, useSlots, Comment, Text } from 'vue'

defineProps<{
  label?: string
}>()

const slots = useSlots()

const isEmpty = computed(() => {
  const content = slots.default?.()

  if (!content) return true

  return content.every((node) => {
    if (node.type === Comment) return true

    if (node.type === Text && !(node.children as string).trim()) return true

    return false
  })
})
</script>
<template>
  <div class="wrapper">
    <div v-if="label">
      {{ label }}
    </div>
    <div>
      <slot v-if="!isEmpty"></slot>
      <span v-else>None</span>
    </div>
  </div>
</template>
<style scoped>
.wrapper {
  padding: 10px 0px;
}
</style>
