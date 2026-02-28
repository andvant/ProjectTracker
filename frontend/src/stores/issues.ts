import { defineStore } from 'pinia'
import { ref } from 'vue'
import issuesApi from '@/api/issuesApi'
import type { CreateIssueRequest, UpdateIssueRequest, IssueDto, IssuesDto } from '@/types'

export const useIssuesStore = defineStore('issues', () => {
  const issues = ref<IssuesDto[]>([])

  const fetchIssues = async (projectId: string) => {
    issues.value = await issuesApi.getIssues(projectId)
  }

  const clearIssues = () => {
    issues.value = []
  }

  const deleteIssue = async (projectId: string, issueId: string) => {
    issues.value = issues.value.filter((p) => p.id !== issueId)

    await issuesApi.deleteIssue(projectId, issueId)
  }

  const getIssueIdByKey = (issueKey: string) => {
    return issues.value.find((i) => i.key === issueKey)?.id
  }

  const getIssue = async (projectId: string, issueId: string) => {
    return await issuesApi.getIssue(projectId, issueId)
  }

  const createIssue = async (projectId: string, request: CreateIssueRequest): Promise<IssueDto> => {
    const issue = await issuesApi.createIssue(projectId, request)

    await fetchIssues(projectId)

    return issue
  }

  const updateIssue = async (
    projectId: string,
    issueId: string,
    request: UpdateIssueRequest,
  ): Promise<IssueDto> => {
    await issuesApi.updateIssue(projectId, issueId, request)

    return await getIssue(projectId, issueId)
  }

  return {
    issues,
    fetchIssues,
    clearIssues,
    deleteIssue,
    getIssueIdByKey,
    getIssue,
    createIssue,
    updateIssue,
  }
})
