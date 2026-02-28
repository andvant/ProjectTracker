<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import Issue from '@/views/Issue.vue'
import Project from '@/components/Project.vue'
import Issues from '@/components/Issues.vue'

const route = useRoute()

const issueKey = computed(() => route.params.issueKey as string)
</script>
<template>
  <div v-if="!issueKey">
    <Project />
    <Issues />
  </div>
  <div v-if="issueKey">
    <Issue />
  </div>
</template>
