import { request } from '@/api/base'
import type { ProjectsDto, ProjectDto, IssuesDto, IssueDto, UsersDto, UserDto } from '@/types'

export const getProjects = () => request<ProjectsDto[]>('projects')

export const getProject = (projectId: string) => request<ProjectDto>(`projects/${projectId}`)

export const getIssues = (projectId: string) => request<IssuesDto[]>(`projects/${projectId}/issues`)

export const getIssue = (projectId: string, issueId: string) =>
  request<IssueDto>(`projects/${projectId}/issues/${issueId}`)

export const getUsers = () => request<UsersDto[]>('users')

export const getUser = (userId: string) => request<UserDto>(`users/${userId}`)
