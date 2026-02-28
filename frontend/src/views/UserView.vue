<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useUsersStore } from '@/stores/users'
import type { UserDto } from '@/types'

const route = useRoute()

const userId = computed(() => route.params.userId as string)

const usersStore = useUsersStore()

const user = ref<UserDto>()

watch(
  userId,
  async (userId) => {
    if (!userId) return

    user.value = await usersStore.getUser(userId)
  },
  { immediate: true },
)
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
