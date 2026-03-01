import apiClient from '@/api/apiClient'
import type { UsersDto, UserDto, UpdateUserGroupsRequest, UserGroupDto } from '@/types/users'

const usersApi = {
  getUsers: () => apiClient.get<UsersDto[]>('users'),

  getUser: (userId: string) => apiClient.get<UserDto>(`users/${userId}`),

  getUserGroups: (userId: string) => apiClient.get<UserGroupDto[]>(`users/${userId}/groups`),

  updateUserGroups: (userId: string, request: UpdateUserGroupsRequest) =>
    apiClient.put(`users/${userId}/groups`, request),
}

export default usersApi
