<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useUsersStore } from '@/stores/users'

const router = useRouter()

const usersStore = useUsersStore()

const onSelectUser = (userId: string) => {
  router.push({ name: 'User', params: { userId } })
}

onMounted(async () => {
  await usersStore.fetchUsers()
})
</script>
<template>
  <div class="users">
    <ul>
      <li v-for="user in usersStore.users" :key="user.id" @click="onSelectUser(user.id)">
        {{ user.name }}
      </li>
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
