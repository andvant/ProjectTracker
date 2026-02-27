import apiClient from '@/api/apiClient'
import type { IssuesDto, IssueDto, CreateIssueRequest } from '@/types'

const issuesApi = {
  getIssues: (projectId: string) => apiClient.get<IssuesDto[]>(`projects/${projectId}/issues`),

  getIssue: (projectId: string, issueId: string) =>
    apiClient.get<IssueDto>(`projects/${projectId}/issues/${issueId}`),

  deleteIssue: (projectId: string, issueId: string) =>
    apiClient.delete(`projects/${projectId}/issues/${issueId}`),

  createIssue: (projectId: string, request: CreateIssueRequest) =>
    apiClient.post<IssueDto, CreateIssueRequest>(`projects/${projectId}/issues`, request),
}

export default issuesApi
