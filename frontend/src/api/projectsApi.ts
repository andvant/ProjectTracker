import apiClient from '@/api/apiClient'
import type {
  ProjectsDto,
  ProjectDto,
  CreateProjectRequest,
  UpdateProjectRequest,
} from '@/types/projects'

const projectsApi = {
  getProjects: (): Promise<ProjectsDto[]> => apiClient.get<ProjectsDto[]>('projects'),

  getProject: (projectId: string): Promise<ProjectDto> =>
    apiClient.get<ProjectDto>(`projects/${projectId}`),

  createProject: (request: CreateProjectRequest): Promise<ProjectDto> =>
    apiClient.post<ProjectDto, CreateProjectRequest>('projects', request),

  updateProject: (projectId: string, request: UpdateProjectRequest): Promise<void> =>
    apiClient.put(`projects/${projectId}`, request),

  deleteProject: (projectId: string): Promise<void> => apiClient.delete(`projects/${projectId}`),

  removeMember: (projectId: string, memberId: string): Promise<void> =>
    apiClient.delete(`projects/${projectId}/members/${memberId}`),

  addMember: (projectId: string, memberId: string): Promise<void> =>
    apiClient.put(`projects/${projectId}/members/${memberId}`, null),

  transferOwnership: (projectId: string, newOwnerId: string): Promise<void> =>
    apiClient.put(`projects/${projectId}/new-owner/${newOwnerId}`, null),

  uploadAttachment: (projectId: string, file: FormData): Promise<void> =>
    apiClient.post<void, FormData>(`projects/${projectId}/attachments`, file),
}

export default projectsApi
