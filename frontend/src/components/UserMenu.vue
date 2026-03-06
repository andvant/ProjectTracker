<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import SignOutButton from '@/components/SignOutButton.vue'

const props = defineProps<{
  userId: string
  fullName: string
}>()

const router = useRouter()

const isOpen = ref(false)
const rootElement = ref<HTMLElement>()

const toggle = () => {
  isOpen.value = !isOpen.value
}

const goToProfile = () => {
  isOpen.value = false
  router.push({ name: 'User', params: { userId: props.userId } })
}

const onClickOutside = (e: MouseEvent) => {
  if (!rootElement.value!.contains(e.target as Node)) {
    isOpen.value = false
  }
}

onMounted(() => {
  document.addEventListener('click', onClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', onClickOutside)
})
</script>
<template>
  <div ref="rootElement" class="user-menu">
    <div @click="toggle">
      {{ fullName }}
    </div>
    <div v-if="isOpen" class="dropdown user-menu">
      <div class="dropdown-item" @click="goToProfile">Profile</div>
      <SignOutButton class="dropdown-item" />
    </div>
  </div>
</template>
<style scoped>
.user-menu {
  position: relative;
}

.dropdown {
  position: absolute;
  top: 0;
  left: 100%;
  min-width: 140px;
  background: #15223d;
  border-radius: 8px;
  padding: 6px;
  margin-left: 20px;
}

.dropdown-item {
  padding: 8px;
  border-radius: 8px;
}

.dropdown-item:hover {
  background: rgba(255, 255, 255, 0.1);
}
</style>
