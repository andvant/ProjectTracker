import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '@/api'
import type { UsersDto } from '@/types'

export const useUsersStore = defineStore('users', () => {
  const users = ref<UsersDto[]>([])

  const fetchUsers = async () => {
    users.value = await api.getUsers()
  }

  return {
    users,
    fetchUsers,
  }
})
