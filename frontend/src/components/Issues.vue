<script setup lang="ts">
import { useIssuesStore } from '@/stores/issues'
import { IssuePriority, IssueType, IssueStatus } from '@/types/issues'
import { getEnumLabel } from '@/utils'

const issuesStore = useIssuesStore()
</script>
<template>
  <div class="issues">
    <ul>
      <li v-for="issue in issuesStore.issues" :key="issue.id">
        <RouterLink :to="{ name: 'Issue', params: { issueKey: issue.key } }">
          <div class="issue-row">
            <span class="issue-col">{{ issue.key }}</span>
            <span class="issue-col">{{ issue.title }}</span>
            <span class="issue-col">{{ getEnumLabel(IssueStatus, issue.status) }}</span>
            <span class="issue-col">{{ getEnumLabel(IssueType, issue.type) }}</span>
            <span class="issue-col">{{ getEnumLabel(IssuePriority, issue.priority) }}</span>
          </div>
        </RouterLink>
      </li>
    </ul>
  </div>
</template>
<style scoped>
.issues {
  flex: 1;
  background-color: white;
  padding: 1rem;
  overflow-y: auto;
}

.issues ul {
  list-style: none;
  margin: 0;
  padding: 0;
}

.issue-row {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  align-items: center;

  padding: 0.5rem 0;
  border-bottom: 1px solid #eee;
  cursor: pointer;
}

.issue-row:hover {
  background-color: #eee;
}

.issue-col {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

a:hover {
  text-decoration: none;
}
</style>
