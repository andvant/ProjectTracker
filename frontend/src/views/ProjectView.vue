<script setup lang="ts">
import Project from '@/components/Project.vue'
import Issues from '@/components/Issues.vue'

defineProps<{ projectId?: string }>()
</script>
<template>
  <div>
    <Project :projectId="projectId" />
    <Issues :projectId="projectId" />
  </div>
</template>
