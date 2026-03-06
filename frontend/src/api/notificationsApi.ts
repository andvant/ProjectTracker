import apiClient from '@/api/apiClient'
import type { NotificationDto } from '@/types/notifications'

const notificationsApi = {
  getNotifications: (): Promise<NotificationDto[]> =>
    apiClient.get<NotificationDto[]>('notifications'),
}

export default notificationsApi
