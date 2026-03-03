<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useUsersStore } from '@/stores/users'
import { useAuth } from '@/auth/useAuth'
import { UpdateUserGroupsRequest, type UserDto, type UserGroupDto } from '@/types/users'
import { Role } from '@/types/roles'
import { formatDate } from '@/utils'
import EntityTitle from '@/components/UI/EntityTitle.vue'
import Property from '@/components/UI/Property.vue'
import ControlButton from '@/components/UI/ControlButton.vue'

const route = useRoute()

const usersStore = useUsersStore()

const { hasRole } = useAuth()

const userId = computed(() => route.params.userId as string)

const canUpdateGroups = computed(() => hasRole(Role.Admin))

const user = ref<UserDto>()
const userGroups = ref<UserGroupDto[]>()

const memberGroups = computed(() => userGroups.value?.filter((g) => g.isMember) ?? [])

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
  <div v-if="user" class="wrapper">
    <EntityTitle :title="user.name" />

    <Property label="Email">{{ user.email }}</Property>
    <Property label="Registration date">{{ formatDate(user.registrationDate) }}</Property>

    <div>
      <Property v-if="!isEditing" label="Groups">
        <ul v-if="memberGroups.length">
          <li v-for="group in memberGroups" :key="group.id">
            {{ group.description }}
          </li>
        </ul>
      </Property>

      <div v-if="isEditing">
        <div>Groups</div>
        <div v-for="group in userGroups" :key="group.id">
          <label>
            <input type="checkbox" :value="group.id" v-model="req.groupIds" />
            {{ group.description }}
          </label>
        </div>
      </div>

      <ControlButton v-if="!isEditing && canUpdateGroups" @click="onEditing" label="Edit" />
      <ControlButton v-if="isEditing" @click="onSubmit" label="Save" type="primary" />
      <ControlButton v-if="isEditing" @click="isEditing = false" label="Cancel" />
    </div>
  </div>
</template>
<style scoped>
.wrapper {
  padding: 1rem;
}
</style>
