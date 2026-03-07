import { defineStore } from 'pinia'
import { ref } from 'vue'
import notificationsApi from '@/api/notificationsApi'
import type { NotificationDto } from '@/types/notifications'

export const useNotificationsStore = defineStore('notifications', () => {
  const notifications = ref<NotificationDto[]>([])
  const unreadCount = ref(0)
  const pollingInterval = 60_000
  let pollingTimer: number | null = null

  const fetchNotifications = async (): Promise<void> => {
    notifications.value = await notificationsApi.getNotifications()
  }

  const fetchUnreadCount = async (): Promise<void> => {
    unreadCount.value = await notificationsApi.getUnreadCount()
  }

  const markRead = async (): Promise<void> => {
    await notificationsApi.markRead()
    unreadCount.value = 0
  }

  const startPollingUnreadCount = () => {
    if (pollingTimer) return

    fetchUnreadCount()

    pollingTimer = window.setInterval(async () => {
      await fetchUnreadCount()
    }, pollingInterval)
  }

  const stopPollingUnreadCount = () => {
    if (!pollingTimer) return

    window.clearInterval(pollingTimer)
    pollingTimer = null
  }

  return {
    notifications,
    unreadCount,
    fetchNotifications,
    markRead,
    startPollingUnreadCount,
    stopPollingUnreadCount,
  }
})
