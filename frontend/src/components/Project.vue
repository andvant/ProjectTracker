<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useProjectsStore } from '@/stores/projects'
import { useUsersStore } from '@/stores/users'
import type { ProjectDto, UpdateProjectRequest } from '@/types'
import { ApiError, type GeneralError } from '@/types/api'
import { applyErrorsFromApi } from '@/utils'

const route = useRoute()
const router = useRouter()

const projectsStore = useProjectsStore()
const usersStore = useUsersStore()

const project = ref<ProjectDto>()

const projectId = computed(() => projectsStore.getProjectIdByKey(route.params.projectKey as string))

const description = ref('')
const name = ref('')
const isEditing = ref(false)
const isAddingMember = ref(false)
const isSubmitting = ref(false)

const selectedMemberId = ref<string | null>(null)

const nonMemberUsers = computed(() =>
  usersStore.users.filter((u) => !project.value?.members.find((m) => m.id === u.id)),
)

type Errors = UpdateProjectRequest & GeneralError

const createDefaultErrors = () => ({
  name: '',
  description: '',
  general: '',
})

const errors = ref<Errors>(createDefaultErrors())

const validate = () => {
  errors.value = createDefaultErrors()

  let isValid = true

  if (!name.value.trim()) {
    errors.value.name = 'Project name is required'
    isValid = false
  }

  return isValid
}

const onUpdateProject = async (projectId: string) => {
  if (!validate()) return

  try {
    isSubmitting.value = true

    project.value = await projectsStore.updateProject(projectId, {
      name: name.value,
      description: description.value,
    })

    isEditing.value = false
  } catch (e) {
    if (e instanceof ApiError && e.problem) {
      applyErrorsFromApi(errors.value, e.problem)
    } else {
      errors.value.general = 'Unexpected error occurred'
    }
  } finally {
    isSubmitting.value = false
  }
}

const onEditing = () => {
  errors.value = createDefaultErrors()
  name.value = project.value!.name
  description.value = project.value!.description || ''

  isEditing.value = true
}

const onDeleteProject = async (projectId: string) => {
  router.push({ name: 'Home' })

  await projectsStore.deleteProject(projectId)
}

const onRemoveMember = async (memberId: string) => {
  if (!project.value) return

  await projectsStore.removeMember(project.value, memberId)
}

const onAddingMember = async () => {
  isAddingMember.value = true
  selectedMemberId.value = null

  await usersStore.fetchUsers()
}

const onAddMember = async () => {
  if (!selectedMemberId.value) return

  isSubmitting.value = true
  try {
    await projectsStore.addMember(project.value!, selectedMemberId.value)
  } finally {
    isAddingMember.value = false
    isSubmitting.value = false
  }
}

watch(
  projectId,
  async (projectId) => {
    if (!projectId) return

    project.value = await projectsStore.getProject(projectId)
  },
  { immediate: true },
)
</script>
<template>
  <div v-if="project" class="project">
    <h2 v-if="!isEditing">{{ project.name }}</h2>
    <div v-else class="form-group">
      <input v-model="name" />
      <span v-if="errors.name" class="error">{{ errors.name }}</span>
    </div>
    <p>Id: {{ project.id }}</p>
    <label>Description: </label>
    <p v-if="!isEditing">{{ project.description }}</p>
    <div v-else class="form-group">
      <input v-model="description" />
      <span v-if="errors.description" class="error">{{ errors.description }}</span>
    </div>
    <p>Owner: {{ project.ownerId }}</p>
    <p>Created on: {{ project.createdOn }}</p>
    <label>Members:</label>
    <ul>
      <li v-for="member in project.members" :key="member.id">
        <label @click="router.push({ name: 'User', params: { userId: member.id } })">
          {{ member.name }}
        </label>
        <button @click="onRemoveMember(member.id)">X</button>
      </li>
    </ul>

    <button @click="onDeleteProject(project.id)">Delete</button>
    <button v-if="!isEditing" @click="onEditing">Edit</button>
    <button v-if="isEditing" @click="onUpdateProject(project.id)" :disabled="isSubmitting">
      Save
    </button>
    <button v-if="isEditing" @click="isEditing = false">Cancel</button>

    <div>
      <button v-if="!isAddingMember" @click="onAddingMember">Add member</button>

      <div v-else>
        <select v-model="selectedMemberId">
          <option disabled :value="null">Select a user</option>
          <option v-for="user in nonMemberUsers" :key="user.id" :value="user.id">
            {{ user.name }}
          </option>
        </select>
        <button @click="onAddMember" :disabled="!selectedMemberId || isSubmitting">Add</button>
        <button @click="isAddingMember = false">Cancel</button>
      </div>
    </div>
  </div>
</template>
<style scoped>
.project {
  padding: 1rem;
}

.form-group {
  margin-bottom: 1rem;
  display: flex;
  flex-direction: column;
}

.error {
  color: red;
  font-size: 0.8rem;
}
</style>
