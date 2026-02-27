import apiClient from '@/api/apiClient'
import type { ProjectsDto, ProjectDto, CreateProjectRequest, UpdateProjectRequest } from '@/types'

const projectsApi = {
  getProjects: () => apiClient.get<ProjectsDto[]>('projects'),

  getProject: (projectId: string) => apiClient.get<ProjectDto>(`projects/${projectId}`),

  createProject: (request: CreateProjectRequest) =>
    apiClient.post<ProjectDto, CreateProjectRequest>('projects', request),

  updateProject: (projectId: string, request: UpdateProjectRequest) =>
    apiClient.put(`projects/${projectId}`, request),

  deleteProject: (projectId: string) => apiClient.delete(`projects/${projectId}`),

  removeMember: (projectId: string, memberId: string) =>
    apiClient.delete(`projects/${projectId}/members/${memberId}`),

  addMember: (projectId: string, memberId: string) =>
    apiClient.put(`projects/${projectId}/members/${memberId}`, null),

  transferOwnership: (projectId: string, newOwnerId: string) =>
    apiClient.put(`projects/${projectId}/new-owner/${newOwnerId}`, null),
}

export default projectsApi
