<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useProjectsStore } from '@/stores/projects'
import { useUsersStore } from '@/stores/users'
import { UpdateProjectRequest, type ProjectDto } from '@/types'
import { ApiError, type ValidationErrors } from '@/types/api'
import { applyErrorsFromApi, createDefaultErrors } from '@/utils'

const route = useRoute()
const router = useRouter()

const projectsStore = useProjectsStore()
const usersStore = useUsersStore()

const project = ref<ProjectDto>()

const projectId = computed(() => projectsStore.getProjectIdByKey(route.params.projectKey as string))

const req = new UpdateProjectRequest()
const isEditing = ref(false)
const isTransferringOwnership = ref(false)
const isAddingMember = ref(false)
const isSubmitting = ref(false)

const selectedMemberId = ref<string | null>(null)
const selectedOwnerId = ref<string>()

const nonMemberUsers = computed(() =>
  usersStore.users.filter((u) => !project.value?.members.find((m) => m.id === u.id)),
)

type Errors = ValidationErrors<UpdateProjectRequest>

const errors = ref<Errors>(createDefaultErrors(req))

const validate = () => {
  errors.value = createDefaultErrors(req)

  let isValid = true

  if (!req.name.trim()) {
    errors.value.name = 'Project name is required'
    isValid = false
  }

  return isValid
}

const onUpdateProject = async (projectId: string) => {
  if (!validate()) return

  try {
    isSubmitting.value = true

    project.value = await projectsStore.updateProject(projectId, req)

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
  errors.value = createDefaultErrors(req)
  req.name = project.value!.name
  req.description = project.value!.description

  isEditing.value = true
}

const onDeleteProject = async (projectId: string) => {
  router.push({ name: 'Home' })

  await projectsStore.deleteProject(projectId)
}

const onRemoveMember = async (memberId: string) => {
  await projectsStore.removeMember(project.value!, memberId)
}

const onAddingMember = async () => {
  await usersStore.fetchUsers()

  selectedMemberId.value = null
  isAddingMember.value = true
}

const onTransferringOwnership = async () => {
  selectedOwnerId.value = project.value!.owner.id
  isTransferringOwnership.value = true
}

const onAddMember = async () => {
  if (!selectedMemberId.value) return

  isSubmitting.value = true
  try {
    await projectsStore.addMember(project.value!, selectedMemberId.value)
    isAddingMember.value = false
  } finally {
    isSubmitting.value = false
  }
}

const onTransferOwnership = async () => {
  isSubmitting.value = true
  try {
    await projectsStore.transferOwnership(project.value!, selectedOwnerId.value!)
  } finally {
    isTransferringOwnership.value = false
    isSubmitting.value = false
  }
}

const onFilesSelected = async (event: Event) => {
  const input = event.target as HTMLInputElement
  if (!input.files) return

  isSubmitting.value = true
  try {
    for (const file of input.files) {
      const formData = new FormData()
      formData.append('file', file)

      project.value = await projectsStore.uploadAttachment(projectId.value!, formData)
    }
  } finally {
    isSubmitting.value = false
  }

  input.value = ''
}

watch(
  projectId,
  async (projectId) => {
    if (!projectId) return

    isAddingMember.value = false
    isEditing.value = false
    isTransferringOwnership.value = false

    project.value = await projectsStore.getProject(projectId)
  },
  { immediate: true },
)
</script>
<template>
  <div v-if="project" class="project">
    <h2 v-if="!isEditing">{{ project.name }}</h2>
    <div v-else class="form-group">
      <input v-model="req.name" />
      <span v-if="errors.name" class="error">{{ errors.name }}</span>
    </div>
    <p>Id: {{ project.id }}</p>
    <label>Description: </label>
    <p v-if="!isEditing">{{ project.description }}</p>
    <div v-else class="form-group">
      <textarea v-model="req.description"></textarea>
      <span v-if="errors.description" class="error">{{ errors.description }}</span>
    </div>

    <label>Owner:</label>
    <p v-if="!isTransferringOwnership">{{ project.owner.name }}</p>
    <button v-if="!isTransferringOwnership" @click="onTransferringOwnership">
      Transfer ownership
    </button>
    <div v-else>
      <select v-model="selectedOwnerId">
        <option v-for="user in project.members" :key="user.id" :value="user.id">
          {{ user.name }}
        </option>
      </select>
      <button
        @click="onTransferOwnership"
        :disabled="selectedOwnerId === project.owner.id || isSubmitting"
      >
        Transfer
      </button>
      <button @click="isTransferringOwnership = false">Cancel</button>
    </div>

    <p>Created on: {{ project.createdOn }}</p>
    <label>Members:</label>
    <ul>
      <li v-for="member in project.members" :key="member.id">
        <label @click="router.push({ name: 'User', params: { userId: member.id } })">
          {{ member.name }}
        </label>
        <button v-if="member.id !== project.owner.id" @click="onRemoveMember(member.id)">X</button>
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

    <div>
      <label>Attachments:</label>
      <ul>
        <li v-for="attachment in project.attachments" :key="attachment.id">
          {{ attachment.name }}
        </li>
      </ul>
    </div>

    <div>
      <p>Upload attachments</p>
      <input type="file" multiple @change="(e) => onFilesSelected(e)" :disabled="isSubmitting" />
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
