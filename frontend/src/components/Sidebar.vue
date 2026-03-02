<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useProjectsStore } from '@/stores/projects'
import UserMenu from '@/components/UserMenu.vue'
import SignInButton from '@/components/SignInButton.vue'
import { useAuth } from '@/auth/useAuth'

const route = useRoute()

const projectsStore = useProjectsStore()

const { isSignedIn, userId, userName, onUserLoaded } = useAuth()

const selectedProjectKey = computed(() => route.params.projectKey)
const usersSelected = computed(() => route.name === 'Users' || route.name === 'User')
const newProjectSelected = computed(() => route.name === 'NewProject')

onMounted(async () => {
  if (isSignedIn.value) {
    await projectsStore.fetchProjects()
  }

  onUserLoaded(projectsStore.fetchProjects)
})
</script>
<template>
  <aside class="sidebar">
    <div v-if="isSignedIn" class="button" :class="{ selected: newProjectSelected }">
      <router-link :to="{ name: 'NewProject' }" custom v-slot="{ navigate, href }">
        <div :href="href" @click="navigate">+ New project</div>
      </router-link>
    </div>
    <div>
      <ul>
        <li
          v-for="project in projectsStore.projects"
          :key="project.id"
          class="button"
          :class="{ selected: project.key === selectedProjectKey }"
        >
          <router-link
            :to="{ name: 'Project', params: { projectKey: project.key } }"
            custom
            v-slot="{ navigate, href }"
          >
            <div :href="href" @click="navigate">{{ project.name }}</div>
          </router-link>
        </li>
      </ul>
    </div>
    <div class="bottom">
      <UserMenu v-if="isSignedIn" :userName="userName!" :userId="userId!" class="button" />

      <div v-if="isSignedIn" class="button" :class="{ selected: usersSelected }">
        <router-link :to="{ name: 'Users' }" custom v-slot="{ navigate, href }">
          <div :href="href" @click="navigate">Users</div>
        </router-link>
      </div>

      <div v-if="!isSignedIn" class="button">
        <SignInButton />
      </div>
    </div>
  </aside>
</template>
<style scoped>
.sidebar {
  width: 180px;
  background-color: #11214a;
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
  padding: 0.5rem 0.2rem;
  border-bottom: 1px solid #eee;

  &.selected {
    background-color: rgba(255, 255, 255, 0.2);
    font-weight: bold;
  }
}

.button:hover:not(.selected) {
  background: rgba(255, 255, 255, 0.1);
}
</style>
