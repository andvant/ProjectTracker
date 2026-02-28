import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import MainLayout from '@/layouts/MainLayout.vue'
import ProjectView from '@/views/ProjectView.vue'
import UsersView from '@/views/UsersView.vue'
import NewProjectView from '@/views/NewProjectView.vue'
import IssueView from '@/views/IssueView.vue'
import UserView from '@/views/UserView.vue'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'Home',
    component: MainLayout,
    children: [
      {
        path: 'projects',
        redirect: '/',
      },
      {
        path: 'projects/:projectKey',
        name: 'Project',
        component: ProjectView,
      },
      {
        path: 'projects/:projectKey/issues/:issueKey',
        name: 'Issue',
        component: IssueView,
      },
      {
        path: 'projects/new',
        name: 'NewProject',
        component: NewProjectView,
      },
      {
        path: 'users',
        name: 'Users',
        component: UsersView,
      },
      {
        path: 'users/:userId',
        name: 'User',
        component: UserView,
      },
    ],
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

export default router
