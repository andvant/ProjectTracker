<script setup lang="ts">
import ProjectDetails from '@/components/ProjectDetails.vue'
import IssuesList from '@/components/IssuesList.vue'

defineProps<{ projectId?: string }>()
</script>
<template>
  <div>
    <ProjectDetails :projectId="projectId" />
    <IssuesList :projectId="projectId" />
  </div>
</template>
