import { request } from '@/api/base'

export interface Project {
  id: string
  key: string
  name: string
}

export interface Issue {
  id: string
  title: string
}

export const fetchProjects = () => request<Project[]>('projects')

export const fetchIssues = (projectId: string) => request<Issue[]>(`projects/${projectId}/issues`)
