import apiClient from '@/api/apiClient'
import { BASE_URL } from '@/api/apiClient'
import type { IssuesDto, IssueDto, CreateIssueRequest, UpdateIssueRequest } from '@/types/issues'

const issuesApi = {
  getIssues: (projectId: string): Promise<IssuesDto[]> =>
    apiClient.get<IssuesDto[]>(`projects/${projectId}/issues`),

  getIssue: (projectId: string, issueId: string): Promise<IssueDto> =>
    apiClient.get<IssueDto>(`projects/${projectId}/issues/${issueId}`),

  deleteIssue: (projectId: string, issueId: string): Promise<void> =>
    apiClient.delete(`projects/${projectId}/issues/${issueId}`),

  createIssue: (projectId: string, request: CreateIssueRequest): Promise<IssueDto> =>
    apiClient.post<IssueDto, CreateIssueRequest>(`projects/${projectId}/issues`, request),

  updateIssue: (projectId: string, issueId: string, request: UpdateIssueRequest): Promise<void> =>
    apiClient.put(`projects/${projectId}/issues/${issueId}`, request),

  removeWatcher: (projectId: string, issueId: string, watcherId: string): Promise<void> =>
    apiClient.delete(`projects/${projectId}/issues/${issueId}/watchers/${watcherId}`),

  addWatcher: (projectId: string, issueId: string, watcherId: string): Promise<void> =>
    apiClient.put(`projects/${projectId}/issues/${issueId}/watchers/${watcherId}`, null),

  uploadAttachment: (projectId: string, issueId: string, file: FormData): Promise<void> =>
    apiClient.post<void, FormData>(`projects/${projectId}/issues/${issueId}/attachments`, file),

  getDownloadAttachmentLink: (projectId: string, issueId: string, attachmentId: string): string =>
    new URL(
      `projects/${projectId}/issues/${issueId}/attachments/${attachmentId}`,
      BASE_URL,
    ).toString(),
}

export default issuesApi
