import apiClient from '@/api/apiClient'
import type { IssuesDto, IssueDto, CreateIssueRequest, UpdateIssueRequest } from '@/types/issues'

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

  removeWatcher: (projectId: string, issueId: string, watcherId: string) =>
    apiClient.delete(`projects/${projectId}/issues/${issueId}/watchers/${watcherId}`),

  addWatcher: (projectId: string, issueId: string, watcherId: string) =>
    apiClient.put(`projects/${projectId}/issues/${issueId}/watchers/${watcherId}`, null),

  uploadAttachment: (projectId: string, issueId: string, file: FormData) =>
    apiClient.post<void, FormData>(`projects/${projectId}/issues/${issueId}/attachments`, file),
}

export default issuesApi
