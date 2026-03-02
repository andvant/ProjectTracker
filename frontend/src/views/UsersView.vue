<script setup lang="ts">
import { onMounted } from 'vue'
import { useUsersStore } from '@/stores/users'
import EntityTitle from '@/components/UI/EntityTitle.vue'

const usersStore = useUsersStore()

onMounted(async () => {
  await usersStore.fetchUsers()
})
</script>
<template>
  <div class="wrapper">
    <EntityTitle title="Users" />

    <ul>
      <li v-for="user in usersStore.users" :key="user.id">
        <RouterLink :to="{ name: 'User', params: { userId: user.id } }">
          {{ user.name }}
        </RouterLink>
      </li>
    </ul>
  </div>
</template>
<style scoped>
.wrapper {
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
