import { defineStore } from 'pinia'
import { ref } from 'vue'
import projectsApi from '@/api/projectsApi'
import { useUsersStore } from '@/stores/users'
import type { CreateProjectRequest, ProjectDto, ProjectsDto, UpdateProjectRequest } from '@/types'

export const useProjectsStore = defineStore('projects', () => {
  const projects = ref<ProjectsDto[]>([])
  const cachedProject = ref<ProjectDto>()

  const fetchProjects = async () => {
    projects.value = await projectsApi.getProjects()
  }

  const deleteProject = async (projectId: string) => {
    projects.value = projects.value.filter((p) => p.id !== projectId)

    await projectsApi.deleteProject(projectId)
  }

  const getProjectIdByKey = (projectKey: string) => {
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
    await projectsApi.updateProject(projectId, {
      name: request.name,
      description: request.description,
    })

    return await getProject(projectId)
  }

  const getProject = async (projectId: string) => {
    const project = await projectsApi.getProject(projectId)

    cachedProject.value = project

    const existing = projects.value.find((u) => u.id === project.id)

    if (existing) {
      Object.assign(existing, project)
    } else {
      projects.value.push(project)
    }

    return project
  }

  const getCachedProject = () => cachedProject.value

  const addMember = async (project: ProjectDto, memberId: string) => {
    const usersStore = useUsersStore()

    await projectsApi.addMember(project.id, memberId)

    if (!usersStore.users.length) {
      await usersStore.fetchUsers()
    }

    const user = usersStore.users.find((u) => u.id === memberId)
    project.members.push(user!)
  }

  const removeMember = async (project: ProjectDto, memberId: string) => {
    await projectsApi.removeMember(project.id, memberId)

    project.members = project.members.filter((m) => m.id !== memberId)
  }

  const transferOwnership = async (project: ProjectDto, newOwnerId: string) => {
    const usersStore = useUsersStore()

    await projectsApi.transferOwnership(project.id, newOwnerId)

    if (!usersStore.users.length) {
      await usersStore.fetchUsers()
    }

    const user = usersStore.users.find((u) => u.id === newOwnerId)
    project.owner = user!
  }

  return {
    projects,
    fetchProjects,
    deleteProject,
    getProjectIdByKey,
    createProject,
    updateProject,
    getProject,
    getCachedProject,
    addMember,
    removeMember,
    transferOwnership,
  }
})
