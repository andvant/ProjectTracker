export interface ProjectsDto {
  id: string
  key: string
  name: string
}

export interface ProjectDto {
  id: string
  key: string
  name: string
  description?: string
  owner: UsersDto
  createdBy: string
  createdOn: Date
  updatedBy: string
  updatedOn: Date
  members: UsersDto[]
}

export interface CreateProjectRequest {
  key: string
  name: string
  description?: string
}

export interface UpdateProjectRequest {
  name: string
  description?: string
}

export interface IssuesDto {
  id: string
  key: string
  title: string
  status: IssueStatusEnum
  type: IssueTypeEnum
  priority: IssuePriorityEnum
}

export interface IssueDto {
  id: string
  key: string
  title: string
  description?: string
  reporter: UsersDto
  projectId: string
  assignee?: UsersDto
  status: IssueStatusEnum
  type: IssueTypeEnum
  priority: IssuePriorityEnum
  dueDate?: Date
  estimationMinutes?: number
  parentIssueId?: string
  createdBy: string
  createdOn: Date
  updatedBy: string
  updatedOn: Date
}

export interface CreateIssueRequest {
  title: string
  description?: string
  assigneeId?: string
  type?: IssueTypeEnum
  priority?: IssuePriorityEnum
  parentIssueId?: string
  dueDate?: Date
  estimationMinutes?: number
}

export interface UpdateIssueRequest {
  title: string
  description?: string
  assigneeId?: string
  issueStatus: IssueStatusEnum
  priority: IssuePriorityEnum
  dueDate?: Date
  estimationMinutes?: number
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

export const IssueStatus = {
  Open: { value: 'Open', label: 'Open' },
  InProgress: { value: 'InProgress', label: 'In progress' },
  InReview: { value: 'InReview', label: 'In review' },
  Done: { value: 'Done', label: 'Done' },
  Cancelled: { value: 'Cancelled', label: 'Cancelled' },
} as const

export const IssueType = {
  Epic: { value: 'Epic', label: 'Epic' },
  Task: { value: 'Task', label: 'Task' },
  Bug: { value: 'Bug', label: 'Bug' },
} as const

export const IssuePriority = {
  Low: { value: 'Low', label: 'Low' },
  Normal: { value: 'Normal', label: 'Normal' },
  High: { value: 'High', label: 'High' },
  Critical: { value: 'Critical', label: 'Critical' },
} as const

export type IssueStatusEnum = (typeof IssueStatus)[keyof typeof IssueStatus]['value']
export type IssueTypeEnum = (typeof IssueType)[keyof typeof IssueType]['value']
export type IssuePriorityEnum = (typeof IssuePriority)[keyof typeof IssuePriority]['value']
