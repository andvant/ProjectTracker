import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '@/api'
import type { IssuesDto } from '@/types'

export const useIssuesStore = defineStore('issues', () => {
  const issues = ref<IssuesDto[]>([])

  const fetchIssues = async (projectId: string) => {
    issues.value = await api.getIssues(projectId)
  }

  const deleteIssue = async (projectId: string, issueId: string) => {
    issues.value = issues.value.filter((p) => p.id !== issueId)

    await api.deleteIssue(projectId, issueId)
  }

  return {
    issues,
    fetchIssues,
    deleteIssue,
  }
})
