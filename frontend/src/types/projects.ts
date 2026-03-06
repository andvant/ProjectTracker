import type { AttachmentDto } from '@/types/attachments'
import type { UsersDto } from '@/types/users'

export interface ProjectsDto {
  id: string
  key: string
  name: string
  createdAt: string
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
  createdAt: string
  updatedBy: string
  updatedAt: string
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
