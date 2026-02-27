<script setup lang="ts">
import { ref, watchEffect } from 'vue'
import api from '@/api'
import type { UserDto } from '@/types'

const props = defineProps<{
  userId?: string
}>()

const user = ref<UserDto>()

watchEffect(async () => {
  if (!props.userId) return

  user.value = await api.getUser(props.userId)
})
</script>
<template>
  <div v-if="user" class="user">
    <h2>{{ user.name }}</h2>
    <p>Id: {{ user.id }}</p>
    <p>Email: {{ user.email }}</p>
    <p>Registration date: {{ user.registrationDate }}</p>
  </div>
</template>
<style scoped>
.user {
  padding: 1rem;
}
</style>
