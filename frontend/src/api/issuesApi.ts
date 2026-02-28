import apiClient from '@/api/apiClient'
import type { IssuesDto, IssueDto, CreateIssueRequest, UpdateIssueRequest } from '@/types'

const issuesApi = {
  getIssues: (projectId: string) => apiClient.get<IssuesDto[]>(`projects/${projectId}/issues`),

  getIssue: (projectId: string, issueId: string) =>
    apiClient.get<IssueDto>(`projects/${projectId}/issues/${issueId}`),

  deleteIssue: (projectId: string, issueId: string) =>
    apiClient.delete(`projects/${projectId}/issues/${issueId}`),

  createIssue: (projectId: string, request: CreateIssueRequest) =>
    apiClient.post<IssueDto, CreateIssueRequest>(`projects/${projectId}/issues`, request),

  updateIssue: (projectId: string, issueId: string, request: UpdateIssueRequest) =>
    apiClient.put(`projects/${projectId}/issues/${issueId}`, request),
}

export default issuesApi
