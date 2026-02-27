import apiClient from '@/api/apiClient'
import type { UsersDto, UserDto } from '@/types'

const usersApi = {
  getUsers: () => apiClient.get<UsersDto[]>('users'),

  getUser: (userId: string) => apiClient.get<UserDto>(`users/${userId}`),
}

export default usersApi
