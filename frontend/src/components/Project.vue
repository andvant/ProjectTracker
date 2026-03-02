<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useProjectsStore } from '@/stores/projects'
import { useUsersStore } from '@/stores/users'
import { useAuth } from '@/auth/useAuth'
import { UpdateProjectRequest, type ProjectDto } from '@/types/projects'
import { ApiError, type ValidationErrors } from '@/types/api'
import { Role } from '@/types/roles'
import { applyErrorsFromApi, createDefaultErrors } from '@/utils'
import EntityTitle from '@/components/UI/EntityTitle.vue'
import Property from '@/components/UI/Property.vue'
import InputProperty from '@/components/UI/InputProperty.vue'
import InputErrors from '@/components/UI/InputErrors.vue'
import ControlButton from '@/components/UI/ControlButton.vue'

const route = useRoute()
const router = useRouter()

const projectsStore = useProjectsStore()
const usersStore = useUsersStore()

const { hasRole, userId } = useAuth()

const projectId = computed(() => projectsStore.getProjectIdByKey(route.params.projectKey as string))

const nonMemberUsers = computed(() =>
  usersStore.users.filter((u) => !project.value?.members.find((m) => m.id === u.id)),
)

const canEditProject = computed(
  () => hasRole(Role.Admin) || userId.value === project.value?.owner.id,
)

const project = ref<ProjectDto>()

const selectedMemberId = ref<string | null>(null)
const selectedOwnerId = ref<string>()

const req = new UpdateProjectRequest()
const isEditing = ref(false)
const isTransferringOwnership = ref(false)
const isAddingMember = ref(false)
const isSubmitting = ref(false)

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
  <div v-if="project" class="wrapper">
    <EntityTitle v-if="!isEditing" :title="project.name" />

    <InputProperty v-if="isEditing" label="Project name" :error="errors.name">
      <input v-model="req.name" />
    </InputProperty>

    <Property v-if="!isEditing" label="Description">{{ project.description }}</Property>

    <InputProperty v-if="isEditing" label="Description" :error="errors.description">
      <textarea v-model="req.description"></textarea>
    </InputProperty>

    <InputErrors :error="errors.general" />

    <ControlButton v-if="!isEditing && canEditProject" @click="onEditing" label="Edit" />
    <ControlButton
      v-if="isEditing"
      @click="onUpdateProject(project.id)"
      :disabled="isSubmitting"
      label="Save"
    />
    <ControlButton v-if="canEditProject" @click="onDeleteProject(project.id)" label="Delete" />
    <ControlButton v-if="isEditing" @click="isEditing = false" label="Cancel" />

    <Property v-if="!isTransferringOwnership" label="Owner">
      <RouterLink :to="{ name: 'User', params: { userId: project.owner.id } }">
        {{ project.owner.name }}
      </RouterLink>
    </Property>

    <ControlButton
      v-if="!isTransferringOwnership && canEditProject"
      @click="onTransferringOwnership"
      label="Transfer ownership"
    />
    <div v-if="isTransferringOwnership">
      <select v-model="selectedOwnerId">
        <option v-for="user in project.members" :key="user.id" :value="user.id">
          {{ user.name }}
        </option>
      </select>
      <ControlButton
        @click="onTransferOwnership"
        :disabled="selectedOwnerId === project.owner.id || isSubmitting"
        label="Transfer"
      />
      <ControlButton @click="isTransferringOwnership = false" label="Cancel" />
    </div>

    <Property label="Created at">{{ project.createdAt }}</Property>
    <Property label="Updated at">{{ project.updatedAt }}</Property>

    <Property label="Members">
      <ul>
        <li v-for="member in project.members" :key="member.id">
          <RouterLink :to="{ name: 'User', params: { userId: member.id } }">
            {{ member.name }}
          </RouterLink>
          <ControlButton
            v-if="member.id !== project.owner.id && canEditProject"
            @click="onRemoveMember(member.id)"
            label="X"
          />
        </li>
      </ul>
    </Property>

    <div v-if="canEditProject">
      <ControlButton v-if="!isAddingMember" @click="onAddingMember" label="Add member" />

      <div v-if="isAddingMember">
        <select v-model="selectedMemberId">
          <option disabled :value="null">Select a user</option>
          <option v-for="user in nonMemberUsers" :key="user.id" :value="user.id">
            {{ user.name }}
          </option>
        </select>
        <ControlButton
          @click="onAddMember"
          :disabled="!selectedMemberId || isSubmitting"
          label="Add"
        />
        <ControlButton @click="isAddingMember = false" label="Cancel" />
      </div>
    </div>

    <Property label="Attachments">
      <ul>
        <li v-for="attachment in project.attachments" :key="attachment.id">
          {{ attachment.name }}
        </li>
      </ul>
    </Property>

    <div v-if="canEditProject">
      <input type="file" multiple @change="(e) => onFilesSelected(e)" :disabled="isSubmitting" />
    </div>
  </div>
</template>
<style scoped>
.wrapper {
  padding: 1rem;
}
</style>
