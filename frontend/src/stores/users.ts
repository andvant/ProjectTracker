import { defineStore } from 'pinia'
import { ref } from 'vue'
import usersApi from '@/api/usersApi'
import type { UpdateUserGroupsRequest, UsersDto } from '@/types/users'

export const useUsersStore = defineStore('users', () => {
  const users = ref<UsersDto[]>([])

  const fetchUsers = async () => {
    users.value = await usersApi.getUsers()
  }

  const getUser = async (userId: string) => {
    return await usersApi.getUser(userId)
  }

  const getUserGroups = async (userId: string) => {
    return await usersApi.getUserGroups(userId)
  }

  const updateUserGroups = async (userId: string, request: UpdateUserGroupsRequest) => {
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
