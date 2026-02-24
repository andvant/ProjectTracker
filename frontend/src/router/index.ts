import { createRouter, createWebHistory } from 'vue-router'
import type { RouteLocationNormalized } from 'vue-router'
import App from '@/App.vue'

const routes = [
  {
    path: '/projects/:projectKey?',
    name: 'Dashboard',
    component: App,
    props: (route: RouteLocationNormalized) => ({
      projectKey: (route.params.projectKey as string) || null,
    }),
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

export default router
