import type { AttachmentDto } from '@/types/attachments'
import type { UsersDto } from '@/types/users'

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
  members: UsersDto[]
  attachments: AttachmentDto[]
  createdBy: string
  createdOn: Date
  updatedBy: string
  updatedOn: Date
}

export class CreateProjectRequest {
  key: string = ''
  name: string = ''
  description?: string
}

export class UpdateProjectRequest {
  name: string = ''
  description?: string
}
