import type { AttachmentDto } from '@/types/attachments'
import type { UsersDto } from '@/types/users'

export interface IssuesDto {
  id: string
  key: string
  title: string
  assignee?: UsersDto
  status: IssueStatusEnum
  type: IssueTypeEnum
  priority: IssuePriorityEnum
  updatedAt: string
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
  dueDate?: string
  estimationMinutes?: number
  parentIssue?: IssuesDto
  childIssues: IssuesDto[]
  watchers: UsersDto[]
  attachments: AttachmentDto[]
  comments: CommentDto[]
  createdBy: string
  createdAt: string
  updatedBy: string
  updatedAt: string
}

export interface CommentDto {
  id: string
  user: UsersDto
  text: string
  createdAt: string
  updatedAt: string
}

export class CreateIssueRequest {
  title: string = ''
  description?: string
  assigneeId?: string
  type?: IssueTypeEnum
  priority?: IssuePriorityEnum
  parentIssueId?: string
  dueDate?: string
  estimationMinutes?: number
}

export class UpdateIssueRequest {
  title: string = ''
  description?: string
  assigneeId?: string
  status: IssueStatusEnum = IssueStatus.Open.value
  priority: IssuePriorityEnum = IssuePriority.Normal.value
  dueDate?: string
  estimationMinutes?: number
}

export class AddCommentRequest {
  text: string = ''
  status: IssueStatusEnum = IssueStatus.Open.value
  assigneeId?: string
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
