import client from '@/api/client'
import type {
  ProjectsDto,
  ProjectDto,
  CreateProjectRequest,
  IssuesDto,
  IssueDto,
  UsersDto,
  UserDto,
} from '@/types'

const api = {
  getProjects: () => client.get<ProjectsDto[]>('projects'),

  getProject: (projectId: string) => client.get<ProjectDto>(`projects/${projectId}`),

  deleteProject: (projectId: string) => client.delete(`projects/${projectId}`),

  getIssues: (projectId: string) => client.get<IssuesDto[]>(`projects/${projectId}/issues`),

  getIssue: (projectId: string, issueId: string) =>
    client.get<IssueDto>(`projects/${projectId}/issues/${issueId}`),

  deleteIssue: (projectId: string, issueId: string) =>
    client.delete(`projects/${projectId}/issues/${issueId}`),

  getUsers: () => client.get<UsersDto[]>('users'),

  getUser: (userId: string) => client.get<UserDto>(`users/${userId}`),

  createProject: (request: CreateProjectRequest) =>
    client.post<ProjectDto, CreateProjectRequest>('projects', request),
}

export default api
