<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useProjectsStore } from '@/stores/projectsStore'
import { useAuth } from '@/auth/useAuth'
import { Role } from '@/types/roles'
import NotificationsMenu from '@/components/NotificationsMenu.vue'
import ProfileMenu from '@/components/ProfileMenu.vue'
import SignInButton from '@/components/SignInButton.vue'

const route = useRoute()

const projectsStore = useProjectsStore()

const { isSignedIn, userId, fullName, onUserLoaded, hasRole } = useAuth()

const selectedProjectKey = computed(() => route.params.projectKey)
const newProjectSelected = computed(() => route.name === 'NewProject')
const usersSelected = computed(() => route.name === 'Users' || route.name === 'User')

const canCreateProject = computed(() => hasRole([Role.Admin, Role.ProjectManager]))

onMounted(async () => {
  if (isSignedIn.value) {
    await projectsStore.fetchProjects()
  }

  onUserLoaded(projectsStore.fetchProjects)
})
</script>
<template>
  <nav class="sidebar">
    <div class="projects-list">
      <div
        v-for="project in projectsStore.projects"
        :key="project.id"
        class="button"
        :class="{ selected: project.key === selectedProjectKey }"
      >
        <RouterLink :to="{ name: 'Project', params: { projectKey: project.key } }">
          <div>{{ project.name }}</div>
        </RouterLink>
      </div>
    </div>
    <div>
      <div
        v-if="isSignedIn && canCreateProject"
        class="button"
        :class="{ selected: newProjectSelected }"
      >
        <RouterLink :to="{ name: 'NewProject' }"><div>+ New project</div></RouterLink>
      </div>

      <div v-if="isSignedIn" class="button" :class="{ selected: usersSelected }">
        <RouterLink :to="{ name: 'Users' }"><div>Users</div></RouterLink>
      </div>

      <NotificationsMenu v-if="isSignedIn" class="button" />
      <ProfileMenu v-if="isSignedIn" :userId="userId!" :fullName="fullName!" class="button" />

      <div v-if="!isSignedIn" class="button">
        <SignInButton />
      </div>
    </div>
  </nav>
</template>
<style scoped>
.sidebar {
  width: 200px;
  background-color: #15223d;
  padding: 1rem;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}

.projects-list {
  overflow-y: auto;
  scrollbar-width: none;
}

.button {
  color: white;
  background: transparent;
  border: none;
  font-size: 1.2rem;
  cursor: pointer;
  margin: 0;
  padding: 0.5rem 0.3rem;
  border-bottom: 1px solid var(--color-grey);

  &.selected {
    background-color: rgba(255, 255, 255, 0.15);
    font-weight: 500;
  }
}

.button:hover:not(.selected) {
  background: rgba(255, 255, 255, 0.08);
}

a:hover {
  text-decoration: none;
}
</style>
