<script setup lang="ts">
import { ref, watchEffect } from 'vue'
import { getUser } from '@/api'
import type { UserDto } from '@/types'

const props = defineProps<{
  userId?: string
}>()

const user = ref<UserDto>()

watchEffect(async () => {
  if (!props.userId) return

  user.value = await getUser(props.userId)
})
</script>
<template>
  <div v-if="user" class="user">
    <h2>{{ user.name }}</h2>
    <p>{{ user.email }}</p>
    <p>{{ user.registrationDate }}</p>
  </div>
</template>
<style scoped>
.user {
  padding: 1rem;
}
</style>
