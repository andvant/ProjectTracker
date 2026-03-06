import { defineStore } from 'pinia'
import { ref } from 'vue'
import projectsApi from '@/api/projectsApi'
import { useUsersStore } from '@/stores/usersStore'
import type {
  CreateProjectRequest,
  ProjectDto,
  ProjectsDto,
  UpdateProjectRequest,
} from '@/types/projects'

export const useProjectsStore = defineStore('projects', () => {
  const projects = ref<ProjectsDto[]>([])
  const cachedProject = ref<ProjectDto>()

  const fetchProjects = async (): Promise<void> => {
    projects.value = await projectsApi.getProjects()

    projects.value.sort((a, b) => a.createdAt.localeCompare(b.createdAt))
  }

  const deleteProject = async (projectId: string): Promise<void> => {
    projects.value = projects.value.filter((p) => p.id !== projectId)

    await projectsApi.deleteProject(projectId)
  }

  const getProjectIdByKey = (projectKey: string): string | undefined => {
    return projects.value.find((p) => p.key === projectKey)?.id
  }

  const createProject = async (request: CreateProjectRequest): Promise<ProjectDto> => {
    const project = await projectsApi.createProject(request)

    await fetchProjects()

    return project
  }

  const updateProject = async (
    projectId: string,
    request: UpdateProjectRequest,
  ): Promise<ProjectDto> => {
    await projectsApi.updateProject(projectId, request)

    return await getProject(projectId)
  }

  const getProject = async (projectId: string): Promise<ProjectDto> => {
    const project = await projectsApi.getProject(projectId)

    cachedProject.value = project

    const existing = projects.value.find((u) => u.id === project.id)!
    existing.name = project.name

    return project
  }

  const addMember = async (project: ProjectDto, memberId: string): Promise<void> => {
    const usersStore = useUsersStore()

    await projectsApi.addMember(project.id, memberId)

    if (!usersStore.users.length) {
      await usersStore.fetchUsers()
    }

    const user = usersStore.users.find((u) => u.id === memberId)
    project.members.push(user!)
  }

  const removeMember = async (project: ProjectDto, memberId: string): Promise<void> => {
    await projectsApi.removeMember(project.id, memberId)

    project.members = project.members.filter((m) => m.id !== memberId)
  }

  const transferOwnership = async (project: ProjectDto, newOwnerId: string): Promise<void> => {
    const usersStore = useUsersStore()

    await projectsApi.transferOwnership(project.id, newOwnerId)

    if (!usersStore.users.length) {
      await usersStore.fetchUsers()
    }

    const user = usersStore.users.find((u) => u.id === newOwnerId)
    project.owner = user!
  }

  const uploadAttachment = async (projectId: string, file: FormData): Promise<ProjectDto> => {
    await projectsApi.uploadAttachment(projectId, file)

    return await getProject(projectId)
  }

  const removeAttachment = async (project: ProjectDto, attachmentId: string): Promise<void> => {
    await projectsApi.removeAttachment(project.id, attachmentId)

    project.attachments = project.attachments.filter((a) => a.id !== attachmentId)
  }

  return {
    projects,
    cachedProject,
    fetchProjects,
    deleteProject,
    getProjectIdByKey,
    createProject,
    updateProject,
    getProject,
    addMember,
    removeMember,
    transferOwnership,
    uploadAttachment,
    removeAttachment,
  }
})
