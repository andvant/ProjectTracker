import apiClient from '@/api/apiClient'
import type { NotificationDto } from '@/types/notifications'

const notificationsApi = {
  getNotifications: (): Promise<NotificationDto[]> =>
    apiClient.get<NotificationDto[]>('notifications'),

  getUnreadCount: (): Promise<number> => apiClient.get<number>('notifications/unread-count'),

  markRead: (): Promise<void> => apiClient.post('notifications/mark-read', null),
}

export default notificationsApi
