<script setup lang="ts">
import { onMounted } from 'vue'
import { useNotificationsStore } from '@/stores/notificationsStore'
import { formatDate } from '@/utils'
import ViewTitle from '@/components/UI/ViewTitle.vue'

const notificationsStore = useNotificationsStore()

onMounted(async () => {
  await notificationsStore.fetchNotifications()
})
</script>
<template>
  <div class="notifications-wrapper">
    <ViewTitle title="Notifications" />

    <div v-for="notification in notificationsStore.notifications" :key="notification.id">
      <div class="timestamp">{{ formatDate(notification.timestamp) }}</div>
      <div><span v-html="notification.message"></span></div>
    </div>
  </div>
</template>
<style scoped>
.notifications-wrapper {
  padding: 2rem;
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.notifications-wrapper :deep(a) {
  color: var(--color-red);
  font-weight: 600;
}

.timestamp {
  color: var(--color-text-muted);
  font-size: 0.9rem;
}
</style>
