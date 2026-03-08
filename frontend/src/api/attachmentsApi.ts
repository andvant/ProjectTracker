import { BASE_URL } from '@/api/apiClient'

const attachmentsApi = {
  getDownloadAttachmentLink: (tempId: string): string =>
    new URL(`attachments/${tempId}`, BASE_URL).toString(),
}

export default attachmentsApi
