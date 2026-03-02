<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useUsersStore } from '@/stores/users'
import { useAuth } from '@/auth/useAuth'
import { UpdateUserGroupsRequest, type UserDto, type UserGroupDto } from '@/types/users'
import { Role } from '@/types/roles'

const route = useRoute()

const usersStore = useUsersStore()

const { hasRole } = useAuth()

const userId = computed(() => route.params.userId as string)

const canUpdateGroups = computed(() => hasRole(Role.Admin))

const user = ref<UserDto>()
const userGroups = ref<UserGroupDto[]>()

const req = reactive(new UpdateUserGroupsRequest())
const isEditing = ref(false)
const isSubmitting = ref(false)

const onEditing = async () => {
  req.groupIds = userGroups.value!.filter((g) => g.isMember).map((g) => g.id)
  isEditing.value = true
}

const onSubmit = async () => {
  isSubmitting.value = true
  try {
    await usersStore.updateUserGroups(userId.value, req)
    userGroups.value?.forEach((g) => (g.isMember = req.groupIds.includes(g.id)))
    isEditing.value = false
  } finally {
    isSubmitting.value = false
  }
}

watch(
  userId,
  async (userId) => {
    if (!userId) return

    user.value = await usersStore.getUser(userId)
    userGroups.value = await usersStore.getUserGroups(userId)
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

    <div>
      <label>Groups:</label>
      <ul v-if="!isEditing">
        <li v-for="group in userGroups?.filter((g) => g.isMember)" :key="group.id">
          {{ group.description }}
        </li>
      </ul>
      <div v-else v-for="group in userGroups" :key="group.id" class="user-option">
        <label>
          <input type="checkbox" :value="group.id" v-model="req.groupIds" />
          {{ group.description }}
        </label>
      </div>
      <button v-if="!isEditing && canUpdateGroups" @click="onEditing">Edit</button>
      <button v-if="isEditing" @click="onSubmit">Submit</button>
      <button v-if="isEditing" @click="isEditing = false">Cancel</button>
    </div>
  </div>
</template>
<style scoped>
.user {
  padding: 1rem;
}
</style>
