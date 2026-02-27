import client from '@/api/client'
import type {
  ProjectsDto,
  ProjectDto,
  CreateProjectRequest,
  UpdateProjectRequest,
  IssuesDto,
  IssueDto,
  UsersDto,
  UserDto,
} from '@/types'

const api = {
  getProjects: () => client.get<ProjectsDto[]>('projects'),

  getProject: (projectId: string) => client.get<ProjectDto>(`projects/${projectId}`),

  createProject: (request: CreateProjectRequest) =>
    client.post<ProjectDto, CreateProjectRequest>('projects', request),

  updateProject: (projectId: string, request: UpdateProjectRequest) =>
    client.put(`projects/${projectId}`, request),

  deleteProject: (projectId: string) => client.delete(`projects/${projectId}`),

  removeMember: (projectId: string, memberId: string) =>
    client.delete(`projects/${projectId}/members/${memberId}`),

  addMember: (projectId: string, memberId: string) =>
    client.put(`projects/${projectId}/members/${memberId}`, null),

  getIssues: (projectId: string) => client.get<IssuesDto[]>(`projects/${projectId}/issues`),

  getIssue: (projectId: string, issueId: string) =>
    client.get<IssueDto>(`projects/${projectId}/issues/${issueId}`),

  deleteIssue: (projectId: string, issueId: string) =>
    client.delete(`projects/${projectId}/issues/${issueId}`),

  getUsers: () => client.get<UsersDto[]>('users'),

  getUser: (userId: string) => client.get<UserDto>(`users/${userId}`),
}

export default api
