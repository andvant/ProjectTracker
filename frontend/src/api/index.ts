import { request } from '@/api/base'
import type { ProjectsDto, ProjectDto, IssuesDto, IssueDto, UsersDto, UserDto } from '@/types'

const api = {
  getProjects: () => request<ProjectsDto[]>('projects'),

  getProject: (projectId: string) => request<ProjectDto>(`projects/${projectId}`),

  deleteProject: (projectId: string) =>
    request<ProjectDto>(`projects/${projectId}`, { method: 'DELETE' }),

  getIssues: (projectId: string) => request<IssuesDto[]>(`projects/${projectId}/issues`),

  getIssue: (projectId: string, issueId: string) =>
    request<IssueDto>(`projects/${projectId}/issues/${issueId}`),

  deleteIssue: (projectId: string, issueId: string) =>
    request<IssueDto>(`projects/${projectId}/issues/${issueId}`, { method: 'DELETE' }),

  getUsers: () => request<UsersDto[]>('users'),

  getUser: (userId: string) => request<UserDto>(`users/${userId}`),
}

export default api
