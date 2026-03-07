<script setup lang="ts">
import { useNotificationsStore } from '@/stores/notificationsStore'
import { formatDate } from '@/utils'
import { ref, onMounted, onUnmounted, computed } from 'vue'

const isOpen = ref(false)
const rootElement = ref<HTMLElement>()

const notificationsStore = useNotificationsStore()

const toggle = async () => {
  isOpen.value = !isOpen.value
  await notificationsStore.fetchNotifications()
}

const unreadNotificationsCount = computed(
  () => notificationsStore.notifications.filter((n) => !n.readTime).length,
)

const onClickOutside = (e: MouseEvent) => {
  if (!rootElement.value!.contains(e.target as Node)) {
    isOpen.value = false
  }
}

const onEscapeKey = (e: KeyboardEvent) => {
  if (e.key === 'Escape') {
    isOpen.value = false
  }
}

onMounted(async () => {
  document.addEventListener('click', onClickOutside)
  document.addEventListener('keydown', onEscapeKey)
  await notificationsStore.fetchNotifications()
})

onUnmounted(() => {
  document.removeEventListener('click', onClickOutside)
  document.removeEventListener('keydown', onEscapeKey)
})
</script>
<template>
  <div ref="rootElement" class="notifications-menu">
    <div @click="toggle" class="notifications-button">
      <div>Notifications</div>
      <div v-if="unreadNotificationsCount" class="unread-count">
        {{ unreadNotificationsCount }}
      </div>
    </div>

    <div v-show="isOpen" class="dropdown">
      <div class="title">Notifications</div>

      <div v-if="!notificationsStore.notifications.length">You have no notifications</div>

      <div
        v-for="notification in notificationsStore.notifications"
        :key="notification.id"
        :class="{ unread: !notification.readTime }"
      >
        <div class="timestamp">{{ formatDate(notification.timestamp) }}</div>
        <div><span v-html="notification.message"></span></div>
      </div>
    </div>
  </div>
</template>
<style scoped>
.notifications-menu {
  position: relative;
}

.notifications-button {
  display: flex;
  justify-content: space-between;
}

.unread-count {
  display: flex;
  justify-content: center;
  align-items: center;
  min-width: 25px;
  height: 25px;
  border-radius: 50%;
  background-color: var(--color-red);
  color: white;
  font-size: 0.9rem;
  font-weight: 600;
}

.dropdown {
  position: absolute;
  bottom: 0;
  left: 100%;
  width: 420px;
  height: 520px;
  background: white;
  border: 1px solid var(--color-dark-grey);
  border-radius: 8px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
  padding: 1.2rem;
  margin-left: 20px;
  color: var(--color-black);
  font-size: 1rem;
  cursor: default;
}

.title {
  font-size: 1.2rem;
  font-weight: 600;
  margin-bottom: 1.2rem;
}

.timestamp {
  color: var(--color-text-muted);
  font-size: 0.9rem;
}

.dropdown :deep(a) {
  color: var(--color-red);
  font-weight: 600;
}

.unread {
  font-weight: 600;
}
</style>
