import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '@/api'
import { useUsersStore } from '@/stores/users'
import type { CreateProjectRequest, ProjectDto, ProjectsDto, UpdateProjectRequest } from '@/types'

export const useProjectsStore = defineStore('projects', () => {
  const projects = ref<ProjectsDto[]>([])

  const fetchProjects = async () => {
    projects.value = await api.getProjects()
  }

  const deleteProject = async (projectId: string) => {
    projects.value = projects.value.filter((p) => p.id !== projectId)

    await api.deleteProject(projectId)
  }

  const getProjectIdByKey = (projectKey: string) => {
    return projects.value.find((p) => p.key === projectKey)?.id
  }

  const createProject = async (request: CreateProjectRequest): Promise<ProjectDto> => {
    const project = await api.createProject({
      key: request.key,
      name: request.name,
      description: request.description,
    })

    await fetchProjects()

    return project
  }

  const updateProject = async (
    projectId: string,
    request: UpdateProjectRequest,
  ): Promise<ProjectDto> => {
    await api.updateProject(projectId, {
      name: request.name,
      description: request.description,
    })

    return await getProject(projectId)
  }

  const getProject = async (projectId: string) => {
    const project = await api.getProject(projectId)

    const existing = projects.value.find((u) => u.id === project.id)

    if (existing) {
      Object.assign(existing, project)
    } else {
      projects.value.push(project)
    }

    return project
  }

  const addMember = async (project: ProjectDto, memberId: string) => {
    const usersStore = useUsersStore()

    await api.addMember(project.id, memberId)

    if (!usersStore.users.length) {
      await usersStore.fetchUsers()
    }

    const user = usersStore.users.find((u) => u.id === memberId)
    project.members.push(user!)
  }

  const removeMember = async (project: ProjectDto, memberId: string) => {
    await api.removeMember(project.id, memberId)

    project.members = project.members.filter((m) => m.id !== memberId)
  }

  return {
    projects,
    fetchProjects,
    deleteProject,
    getProjectIdByKey,
    createProject,
    updateProject,
    getProject,
    addMember,
    removeMember,
  }
})
