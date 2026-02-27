import apiClient from '@/api/apiClient'
import type { IssuesDto, IssueDto } from '@/types'

const issuesApi = {
  getIssues: (projectId: string) => apiClient.get<IssuesDto[]>(`projects/${projectId}/issues`),

  getIssue: (projectId: string, issueId: string) =>
    apiClient.get<IssueDto>(`projects/${projectId}/issues/${issueId}`),

  deleteIssue: (projectId: string, issueId: string) =>
    apiClient.delete(`projects/${projectId}/issues/${issueId}`),
}

export default issuesApi
