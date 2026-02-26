<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import Project from '@/components/Project.vue'
import Issues from '@/components/Issues.vue'
import Issue from '@/components/Issue.vue'

defineProps<{
  projectId?: string
  issueId?: string
}>()

const route = useRoute()

const issueKey = computed(() => route.params.issueKey as string)
</script>
<template>
  <div v-if="!issueKey">
    <Project :projectId="projectId" />
    <Issues :projectId="projectId" />
  </div>
  <div v-if="issueKey">
    <Issue :projectId="projectId" :issueId="issueId" />
  </div>
</template>
