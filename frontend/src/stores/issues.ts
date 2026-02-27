import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '@/api'
import type { IssuesDto } from '@/types'

export const useIssuesStore = defineStore('issues', () => {
  const issues = ref<IssuesDto[]>([])

  const fetchIssues = async (projectId: string) => {
    issues.value = await api.getIssues(projectId)
  }

  const clearIssues = () => {
    issues.value = []
  }

  const deleteIssue = async (projectId: string, issueId: string) => {
    issues.value = issues.value.filter((p) => p.id !== issueId)

    await api.deleteIssue(projectId, issueId)
  }

  const getIssueIdByKey = (issueKey: string) => {
    return issues.value.find((i) => i.key === issueKey)?.id
  }

  const getIssue = async (projectId: string, issueId: string) => {
    const issue = await api.getIssue(projectId, issueId)

    const existing = issues.value.find((i) => i.id === issue.id)

    if (existing) {
      Object.assign(existing, issue)
    } else {
      issues.value.push(issue)
    }

    return issue
  }

  return {
    issues,
    fetchIssues,
    clearIssues,
    deleteIssue,
    getIssueIdByKey,
    getIssue,
  }
})
