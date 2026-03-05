<script setup lang="ts">
import { onMounted } from 'vue'
import { useUsersStore } from '@/stores/usersStore'
import ViewTitle from '@/components/UI/ViewTitle.vue'

const usersStore = useUsersStore()

onMounted(async () => {
  await usersStore.fetchUsers()
})
</script>
<template>
  <div class="users-wrapper">
    <ViewTitle title="Users" />

    <ul class="list">
      <li v-for="user in usersStore.users" :key="user.id">
        <RouterLink :to="{ name: 'User', params: { userId: user.id } }">
          {{ user.name }}
        </RouterLink>
      </li>
    </ul>
  </div>
</template>
<style scoped>
.users-wrapper {
  padding: 2rem;
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.list {
  margin-top: 0;
}
</style>
