import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '@/api'
import type { UsersDto } from '@/types'

export const useUsersStore = defineStore('users', () => {
  const users = ref<UsersDto[]>([])

  const fetchUsers = async () => {
    users.value = await api.getUsers()
  }

  const getUser = async (userId: string) => {
    const user = await api.getUser(userId)

    const existing = users.value.find((u) => u.id === user.id)

    if (existing) {
      Object.assign(existing, user)
    } else {
      users.value.push(user)
    }

    return user
  }

  return {
    users,
    fetchUsers,
    getUser,
  }
})
