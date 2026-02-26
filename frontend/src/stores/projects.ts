import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '@/api'
import type { ProjectsDto } from '@/types'

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

  return {
    projects,
    fetchProjects,
    deleteProject,
    getProjectIdByKey,
  }
})
