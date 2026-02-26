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
  ownerId: string
  createdBy: string
  createdOn: Date
  updatedBy: string
  updatedOn: Date
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
  status: IssueStatus
  type: IssueType
  priority: IssuePriority
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
