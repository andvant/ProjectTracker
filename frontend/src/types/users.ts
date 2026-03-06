export interface UsersDto {
  id: string
  fullName: string
}

export interface UserDto {
  id: string
  username: string
  email: string
  fullName: string
  registrationDate: string
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
