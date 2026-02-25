<script setup lang="ts">
import { ref, watchEffect } from 'vue'
import { getProject } from '@/api'
import type { ProjectDto } from '@/types'

const project = ref<ProjectDto>()

const props = defineProps<{ projectId?: string }>()

watchEffect(async () => {
  if (!props.projectId) return

  project.value = await getProject(props.projectId)
})
</script>
<template>
  <div v-if="project" class="project">
    <h2>{{ project.name }}</h2>
    <p>{{ project.description }}</p>
    <p>{{ project.ownerId }}</p>
    <p>{{ project.createdOn }}</p>
  </div>
</template>
<style scoped>
.project {
  padding: 1rem;
}
</style>
