import apiClient from '@/api/apiClient'
import type { UsersDto, UserDto, UpdateUserGroupsRequest, UserGroupDto } from '@/types/users'

const usersApi = {
  getUsers: (): Promise<UsersDto[]> => apiClient.get<UsersDto[]>('users'),

  getUser: (userId: string): Promise<UserDto> => apiClient.get<UserDto>(`users/${userId}`),

  getUserGroups: (userId: string): Promise<UserGroupDto[]> =>
    apiClient.get<UserGroupDto[]>(`users/${userId}/groups`),

  updateUserGroups: (userId: string, request: UpdateUserGroupsRequest): Promise<void> =>
    apiClient.put(`users/${userId}/groups`, request),
}

export default usersApi
