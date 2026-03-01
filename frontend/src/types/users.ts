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

export interface UserGroupDto {
  id: string
  name: string
  description: string
  isMember: boolean
}

export class UpdateUserGroupsRequest {
  groupIds: string[] = []
}
