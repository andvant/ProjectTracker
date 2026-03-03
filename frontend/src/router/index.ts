import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import { AUTH_CALLBACK_ROUTE } from '@/auth/authService'
import MainLayout from '@/layouts/MainLayout.vue'
import ProjectView from '@/views/ProjectView.vue'
import UsersView from '@/views/UsersView.vue'
import NewProjectView from '@/views/NewProjectView.vue'
import IssueView from '@/views/IssueView.vue'
import UserView from '@/views/UserView.vue'
import AuthCallbackView from '@/views/AuthCallbackView.vue'
import NotFoundView from '@/views/NotFoundView.vue'

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
      {
        path: AUTH_CALLBACK_ROUTE,
        component: AuthCallbackView,
      },
      {
        path: ':pathMatch(.*)*',
        name: 'NotFound',
        component: NotFoundView,
      },
    ],
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,

  scrollBehavior: (to, from, savedPosition) =>
    savedPosition ? savedPosition : { top: 0, behavior: 'instant' },
})

export default router
