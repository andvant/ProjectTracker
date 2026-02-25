import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import MainLayout from '@/layouts/MainLayout.vue'
import ProjectView from '@/views/ProjectView.vue'
import UsersView from '@/views/UsersView.vue'
import Issue from '@/components/Issue.vue'
import User from '@/components/User.vue'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: MainLayout,
    children: [
      {
        path: 'projects/:projectKey?',
        name: 'Project',
        component: ProjectView,
        children: [
          {
            path: 'issues/:issueKey',
            name: 'Issue',
            component: Issue,
          },
        ],
      },
      {
        path: 'users',
        name: 'Users',
        component: UsersView,
        children: [
          {
            path: ':userId',
            name: 'User',
            component: User,
          },
        ],
      },
    ],
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

export default router
