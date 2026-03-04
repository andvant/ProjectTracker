<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useIssuesStore } from '@/stores/issues'
import { IssuePriority, IssueType, IssueStatus } from '@/types/issues'
import { getEnumLabel } from '@/utils'

const router = useRouter()

const goToIssue = (issueKey: string) => {
  router.push({ name: 'Issue', params: { issueKey } })
}

const issuesStore = useIssuesStore()
</script>
<template>
  <div class="issues-wrapper">
    <table class="issues-table">
      <thead>
        <tr>
          <th class="col-small">Key</th>
          <th class="col-title">Title</th>
          <th class="col-small">Status</th>
          <th class="col-small">Assignee</th>
          <th class="col-small">Type</th>
          <th class="col-small">Priority</th>
        </tr>
      </thead>
      <tbody>
        <tr
          v-for="issue in issuesStore.issues"
          :key="issue.id"
          @click="goToIssue(issue.key)"
          class="issue-row"
        >
          <td>{{ issue.key }}</td>
          <td>{{ issue.title }}</td>
          <td>{{ getEnumLabel(IssueStatus, issue.status) }}</td>
          <td :class="{ unassigned: !issue.assignee }">
            {{ issue.assignee?.name ?? 'Unassigned' }}
          </td>
          <td>{{ getEnumLabel(IssueType, issue.type) }}</td>
          <td>{{ getEnumLabel(IssuePriority, issue.priority) }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
<style scoped>
.issues-wrapper {
  flex: 1;
  width: 95%;
}

.issues-table {
  border-collapse: collapse;
}

.issues-table th,
.issues-table td {
  border: 1px solid var(--color-grey);
  border-left: none;
  border-right: none;
  padding: 0.6rem 0.75rem;
  text-align: left;
}

.issues-table th {
  border-top: none;
  font-weight: 600;
}

.col-small {
  width: 12%;
}

.col-title {
  width: 40%;
}

.issue-row {
  cursor: pointer;
}

.issue-row:hover {
  background-color: var(--color-grey);
}

.unassigned {
  color: var(--color-text-muted);
}
</style>
