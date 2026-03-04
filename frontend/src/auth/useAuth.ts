import { ref, computed } from 'vue'
import type { User } from 'oidc-client-ts'
import { userManager } from '@/auth/authService'
import type { Role } from '@/types/roles'

const user = ref<User | null>(null)

const userRoles = computed(() => (user.value?.profile?.roles as string[]) ?? [])
const userId = computed(() => user.value?.profile?.sub)
const userName = computed(() => user.value?.profile?.preferred_username)
const accessToken = computed(() => user.value?.access_token)
const isSignedIn = computed(() => !!user.value)

const hasRole = (roles: Role | Role[]) => {
  return Array.isArray(roles)
    ? roles.some((role) => userRoles.value.includes(role))
    : userRoles.value.includes(roles)
}

export const initAuth = async () => {
  user.value = await userManager.getUser()
}

const onUserLoaded = (callback: () => Promise<void>) => {
  userManager.events.addUserLoaded(async () => {
    await initAuth()
    await callback()
  })
}

export const useAuth = () => {
  return {
    user,
    userRoles,
    userId,
    userName,
    accessToken,
    isSignedIn,
    hasRole,
    onUserLoaded,
  }
}
