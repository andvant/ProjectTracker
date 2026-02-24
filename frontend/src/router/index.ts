import { createRouter, createWebHistory } from 'vue-router'
import MainLayout from '@/layouts/MainLayout.vue'
import ProjectView from '@/views/ProjectView.vue'
import UsersView from '@/views/UsersView.vue'

const routes = [
  {
    path: '/',
    component: MainLayout,
    children: [
      {
        path: 'projects/:projectKey?',
        name: 'Project',
        component: ProjectView,
      },
      {
        path: 'users',
        name: 'Users',
        component: UsersView,
      },
    ],
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

export default router
