import { request } from '@/api/base'

export interface ProjectsDto {
  id: string
  key: string
  name: string
}

export interface ProjectDto {
  id: string
  key: string
  name: string
  description: string
  ownerId: string
  createdBy: string
  createdOn: Date
  updatedBy: string
  updatedOn: Date
}

export interface IssuesDto {
  id: string
  title: string
}

export interface IssueDto {
  id: string
  key: string
  title: string
  description: string
  reporterId: string
  projectId: string
  assigneeId: string
  status: IssueStatus
  type: IssueType
  priority: IssuePriority
  dueDate: Date
  estimationMinutes: number
  parentIssueId: string
  createdBy: string
  createdOn: Date
  updatedBy: string
  updatedOn: Date
}

export interface UsersDto {
  id: string
  name: string
}

export interface UserDto {
  id: string
  name: string
  email: string
  registrationDate: Date
}

enum IssueStatus {
  Open = 'Open',
  InProgress = 'InProgress',
  InReview = 'InReview',
  Done = 'Done',
  Cancelled = 'Cancelled',
}

enum IssueType {
  Epic = 'Epic',
  Task = 'Task',
  Bug = 'Bug',
}

enum IssuePriority {
  Low = 'Low',
  Normal = 'Normal',
  High = 'High',
  Critical = 'Critical',
}

export const getProjects = () => request<ProjectsDto[]>('projects')

export const getProject = (projectId: string) => request<ProjectDto>(`projects/${projectId}`)

export const getIssues = (projectId: string) => request<IssuesDto[]>(`projects/${projectId}/issues`)

export const getIssue = (projectId: string, issueId: string) =>
  request<IssueDto>(`projects/${projectId}/issues/${issueId}`)

export const getUsers = () => request<UsersDto[]>('users')

export const getUser = (userId: string) => request<UserDto>(`users/${userId}`)
