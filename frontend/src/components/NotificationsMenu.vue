<script setup lang="ts">
import { useNotificationsStore } from '@/stores/notificationsStore'
import { formatDate } from '@/utils'
import { ref, onMounted, onUnmounted } from 'vue'

const isOpen = ref(false)
const rootElement = ref<HTMLElement>()

const notificationsStore = useNotificationsStore()

const toggle = async () => {
  isOpen.value = !isOpen.value
  await notificationsStore.fetchNotifications()
}

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
})

onUnmounted(() => {
  document.removeEventListener('click', onClickOutside)
  document.removeEventListener('keydown', onEscapeKey)
})
</script>
<template>
  <div ref="rootElement" class="notifications-menu">
    <div @click="toggle">Notifications</div>

    <div v-show="isOpen" class="dropdown">
      <div class="title">Notifications</div>
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
