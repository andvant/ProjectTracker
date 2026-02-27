import { defineStore } from 'pinia'
import { ref } from 'vue'
import usersApi from '@/api/usersApi'
import type { UsersDto } from '@/types'

export const useUsersStore = defineStore('users', () => {
  const users = ref<UsersDto[]>([])

  const fetchUsers = async () => {
    users.value = await usersApi.getUsers()
  }

  const getUser = async (userId: string) => {
    return await usersApi.getUser(userId)
  }

  return {
    users,
    fetchUsers,
    getUser,
  }
})
