<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { getUsers } from '@/api'
import type { UsersDto } from '@/types'

const users = ref<UsersDto[]>([])

onMounted(async () => {
  users.value = await getUsers()
})
</script>
<template>
  <div class="users">
    <ul>
      <li v-for="user in users" :key="user.id">{{ user.name }}</li>
    </ul>
  </div>
</template>
<style scoped>
.users {
  flex: 1;
  background-color: white;
  padding: 1rem;
  overflow-y: auto;
}

.users ul {
  list-style: none;
  margin: 0;
  padding: 0;
}

.users li {
  padding: 0.5rem 0;
  border-bottom: 1px solid #eee;
}
</style>
