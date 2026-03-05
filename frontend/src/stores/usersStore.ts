import { defineStore } from 'pinia'
import { ref } from 'vue'
import usersApi from '@/api/usersApi'
import type { UpdateUserGroupsRequest, UserDto, UserGroupDto, UsersDto } from '@/types/users'

export const useUsersStore = defineStore('users', () => {
  const users = ref<UsersDto[]>([])

  const fetchUsers = async (): Promise<void> => {
    users.value = await usersApi.getUsers()
  }

  const getUser = async (userId: string): Promise<UserDto> => {
    return await usersApi.getUser(userId)
  }

  const getUserGroups = async (userId: string): Promise<UserGroupDto[]> => {
    return await usersApi.getUserGroups(userId)
  }

  const updateUserGroups = async (
    userId: string,
    request: UpdateUserGroupsRequest,
  ): Promise<void> => {
    await usersApi.updateUserGroups(userId, request)
  }

  return {
    users,
    fetchUsers,
    getUser,
    getUserGroups,
    updateUserGroups,
  }
})
