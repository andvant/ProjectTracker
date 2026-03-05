<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useProjectsStore } from '@/stores/projectsStore'
import { useAuth } from '@/auth/useAuth'
import { Role } from '@/types/roles'
import UserMenu from '@/components/UserMenu.vue'
import SignInButton from '@/components/SignInButton.vue'

const route = useRoute()

const projectsStore = useProjectsStore()

const { isSignedIn, userId, userName, onUserLoaded, hasRole } = useAuth()

const selectedProjectKey = computed(() => route.params.projectKey)
const usersSelected = computed(() => route.name === 'Users' || route.name === 'User')
const newProjectSelected = computed(() => route.name === 'NewProject')

const canCreateProject = computed(() => hasRole([Role.Admin, Role.ProjectManager]))

onMounted(async () => {
  if (isSignedIn.value) {
    await projectsStore.fetchProjects()
  }

  onUserLoaded(projectsStore.fetchProjects)
})
</script>
<template>
  <aside class="sidebar">
    <div
      v-if="isSignedIn && canCreateProject"
      class="button"
      :class="{ selected: newProjectSelected }"
    >
      <RouterLink :to="{ name: 'NewProject' }"><div>+ New project</div></RouterLink>
    </div>
    <div>
      <ul>
        <li
          v-for="project in projectsStore.projects"
          :key="project.id"
          class="button"
          :class="{ selected: project.key === selectedProjectKey }"
        >
          <RouterLink :to="{ name: 'Project', params: { projectKey: project.key } }">
            <div>{{ project.name }}</div>
          </RouterLink>
        </li>
      </ul>
    </div>
    <div class="bottom">
      <UserMenu v-if="isSignedIn" :userName="userName!" :userId="userId!" class="button" />

      <div v-if="isSignedIn" class="button" :class="{ selected: usersSelected }">
        <RouterLink :to="{ name: 'Users' }"><div>Users</div></RouterLink>
      </div>

      <div v-if="!isSignedIn" class="button">
        <SignInButton />
      </div>
    </div>
  </aside>
</template>
<style scoped>
.sidebar {
  width: 200px;
  background-color: #15223d;
  padding: 1rem;
  display: flex;
  flex-direction: column;
}

.sidebar ul {
  list-style: none;
  margin: 0;
  padding: 0;
}

.bottom {
  margin-top: auto;
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
