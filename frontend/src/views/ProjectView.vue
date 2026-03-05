<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useProjectsStore } from '@/stores/projectsStore'
import { useIssuesStore } from '@/stores/issuesStore'
import { useUsersStore } from '@/stores/usersStore'
import projectsApi from '@/api/projectsApi'
import { useAuth } from '@/auth/useAuth'
import { UpdateProjectRequest, type ProjectDto } from '@/types/projects'
import { ApiError, type ValidationErrors } from '@/types/api'
import { Role } from '@/types/roles'
import { applyErrorsFromApi, createDefaultErrors, formatDate } from '@/utils'
import NewIssue from '@/components/NewIssue.vue'
import Issues from '@/components/Issues.vue'
import ConfirmModal from '@/components/ConfirmModal.vue'
import ViewTitle from '@/components/UI/ViewTitle.vue'
import Property from '@/components/UI/Property.vue'
import InputProperty from '@/components/UI/InputProperty.vue'
import InputErrors from '@/components/UI/InputErrors.vue'
import ControlButton from '@/components/UI/ControlButton.vue'

const route = useRoute()
const router = useRouter()

const projectsStore = useProjectsStore()
const issuesStore = useIssuesStore()
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
const showConfirmModal = ref(false)

const req = reactive(new UpdateProjectRequest())
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

const onDeleteProject = async () => {
  router.push({ name: 'Home' })

  await projectsStore.deleteProject(projectId.value!)
  showConfirmModal.value = false
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

const fileInputRef = ref<HTMLInputElement>()

const openFileDialog = () => fileInputRef.value!.click()

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

const onRemoveAttachment = async (attachmentId: string) => {
  await projectsStore.removeAttachment(project.value!, attachmentId)
}

watch(
  projectId,
  async (projectId) => {
    if (!projectId) return

    isAddingMember.value = false
    isEditing.value = false
    isTransferringOwnership.value = false

    project.value = await projectsStore.getProject(projectId)
    await issuesStore.fetchIssues(projectId)
  },
  { immediate: true },
)
</script>
<template>
  <div v-if="project" class="project-wrapper">
    <div class="project-column">
      <ViewTitle v-if="!isEditing" :title="project.name" :subtitle="project.key" />

      <InputProperty v-if="isEditing" label="Project name" :error="errors.name">
        <input v-model="req.name" class="text-input" />
      </InputProperty>

      <Property v-if="!isEditing" label="Description">{{ project.description }}</Property>

      <InputProperty v-if="isEditing" label="Description" :error="errors.description">
        <textarea v-model="req.description" class="text-input"></textarea>
      </InputProperty>

      <InputErrors :error="errors.general" />

      <div>
        <ControlButton v-if="!isEditing && canEditProject" @click="onEditing" label="Edit" />
        <ControlButton
          v-if="isEditing"
          @click="onUpdateProject(project.id)"
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
        <ControlButton
          v-if="canEditProject"
          @click="showConfirmModal = true"
          :disabled="isSubmitting"
          label="Delete project"
          type="danger"
        />
      </div>

      <NewIssue />
      <Issues v-if="issuesStore.issues.length" />
    </div>

    <div class="project-column">
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

      <InputProperty v-if="isTransferringOwnership" label="Owner">
        <div class="user-select">
          <select v-model="selectedOwnerId">
            <option v-for="user in project.members" :key="user.id" :value="user.id">
              {{ user.name }}
            </option>
          </select>
          <div>
            <ControlButton
              @click="onTransferOwnership"
              :disabled="selectedOwnerId === project.owner.id || isSubmitting"
              label="Transfer"
              type="primary"
            />
            <ControlButton @click="isTransferringOwnership = false" label="Cancel" />
          </div>
        </div>
      </InputProperty>

      <Property label="Created">{{ formatDate(project.createdAt) }}</Property>
      <Property label="Updated">{{ formatDate(project.updatedAt) }}</Property>

      <Property label="Attachments">
        <ul v-if="project.attachments.length" class="list">
          <li v-for="attachment in project.attachments" :key="attachment.id">
            <a :href="projectsApi.getDownloadAttachmentLink(projectId!, attachment.id)">
              {{ attachment.name }}
            </a>
            <ControlButton
              v-if="canEditProject"
              @click="onRemoveAttachment(attachment.id)"
              type="remove"
            />
          </li>
        </ul>
      </Property>

      <div v-if="canEditProject">
        <input
          ref="fileInputRef"
          type="file"
          multiple
          @change="onFilesSelected"
          style="display: none"
        />
        <ControlButton @click="openFileDialog" :disabled="isSubmitting" label="Add attachments" />
      </div>

      <Property label="Members">
        <ul class="list">
          <li v-for="member in project.members" :key="member.id">
            <RouterLink :to="{ name: 'User', params: { userId: member.id } }">
              {{ member.name }}
            </RouterLink>
            <ControlButton
              v-if="member.id !== project.owner.id && canEditProject"
              @click="onRemoveMember(member.id)"
              type="remove"
            />
          </li>
        </ul>
      </Property>

      <ControlButton
        v-if="!isAddingMember && canEditProject"
        @click="onAddingMember"
        label="Add member"
      />

      <div v-if="isAddingMember" class="user-select">
        <select v-model="selectedMemberId">
          <option disabled :value="null">Select a user</option>
          <option v-for="user in nonMemberUsers" :key="user.id" :value="user.id">
            {{ user.name }}
          </option>
        </select>
        <div>
          <ControlButton
            @click="onAddMember"
            :disabled="!selectedMemberId || isSubmitting"
            label="Add"
            type="primary"
          />
          <ControlButton @click="isAddingMember = false" label="Cancel" />
        </div>
      </div>
    </div>

    <ConfirmModal
      :show="showConfirmModal"
      :title="`Delete project ${project!.key}`"
      @cancel="showConfirmModal = false"
      @confirm="onDeleteProject"
    />
  </div>
</template>
<style scoped>
.project-wrapper {
  padding: 2rem;
  display: grid;
  grid-template-columns: 3fr 1fr;
  align-items: start;
  width: 70vw;
}

.project-column {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.user-select {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.text-input {
  width: 500px;
}
</style>
