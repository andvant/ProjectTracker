<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useUsersStore } from '@/stores/usersStore'
import { useAuth } from '@/auth/useAuth'
import { UpdateUserGroupsRequest, type UserDto, type UserGroupDto } from '@/types/users'
import { Role } from '@/types/roles'
import { formatDate } from '@/utils'
import ViewTitle from '@/components/UI/ViewTitle.vue'
import LabelProperty from '@/components/UI/LabelProperty.vue'
import LabelInput from '@/components/UI/LabelInput.vue'
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
  <div v-if="user" class="user-wrapper">
    <ViewTitle :title="user.fullName" />

    <LabelProperty label="Email">{{ user.email }}</LabelProperty>
    <LabelProperty label="Username">{{ user.username }}</LabelProperty>
    <LabelProperty label="Registration date">{{ formatDate(user.registrationDate) }}</LabelProperty>

    <LabelProperty v-if="!isEditing" label="Groups">
      <ul v-if="memberGroups.length" class="list">
        <li v-for="group in memberGroups" :key="group.id">
          {{ group.description }}
        </li>
      </ul>
    </LabelProperty>

    <LabelInput v-if="isEditing" label="Groups">
      <div v-for="group in userGroups" :key="group.id">
        <label class="group-option">
          <input type="checkbox" :value="group.id" v-model="req.groupIds" />
          <div>{{ group.description }}</div>
        </label>
      </div>
    </LabelInput>

    <div>
      <ControlButton v-if="!isEditing && canUpdateGroups" @click="onEditing" label="Edit" />
      <ControlButton
        v-if="isEditing"
        @click="onSubmit"
        :disabled="isSubmitting"
        label="Save"
        type="primary"
      />
      <ControlButton
        v-if="isEditing"
        @click="isEditing = false"
        :disabled="isSubmitting"
        label="Cancel"
      />
    </div>
  </div>
</template>
<style scoped>
.user-wrapper {
  padding: 2rem;
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.group-option {
  display: flex;
  align-items: center;
  gap: 0.2rem;
  margin-top: 1rem;
}

.group-option input {
  width: 1rem;
}

.list {
  width: 380px;
}
</style>
