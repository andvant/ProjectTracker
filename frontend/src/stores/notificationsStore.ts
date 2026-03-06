import { defineStore } from 'pinia'
import { ref } from 'vue'
import notificationsApi from '@/api/notificationsApi'
import type { NotificationDto } from '@/types/notifications'

export const useNotificationsStore = defineStore('notifications', () => {
  const notifications = ref<NotificationDto[]>([])

  const fetchNotifications = async (): Promise<void> => {
    notifications.value = await notificationsApi.getNotifications()
  }

  return {
    notifications,
    fetchNotifications,
  }
})
