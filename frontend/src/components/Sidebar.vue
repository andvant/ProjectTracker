<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { useProjectsStore } from '@/stores/projects'
import SignInButton from '@/components/SignInButton.vue'
import SignOutButton from '@/components/SignOutButton.vue'
import { userManager } from '@/auth/authService'

const route = useRoute()

const projectsStore = useProjectsStore()

const selectedProjectKey = computed(() => route.params.projectKey)
const usersSelected = computed(() => route.name === 'Users' || route.name === 'User')
const newProjectSelected = computed(() => route.name === 'NewProject')

const isSignedIn = ref(false)

const initSidebar = async () => {
  isSignedIn.value = true
  await projectsStore.fetchProjects()
}

onMounted(async () => {
  if (await userManager.getUser()) {
    await initSidebar()
  }

  userManager.events.addUserLoaded(initSidebar)
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
      <div v-if="isSignedIn" class="button" :class="{ selected: usersSelected }">
        <router-link :to="{ name: 'Users' }" custom v-slot="{ navigate, href }">
          <div :href="href" @click="navigate">Users</div>
        </router-link>
      </div>
      <div v-if="isSignedIn" class="button">
        <SignOutButton />
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
  background-color: #11214a;
  padding: 1rem;
  display: flex;
  flex-direction: column;
}

.button {
  color: white;
  background: transparent;
  border: none;
  font-size: 1.2rem;
  cursor: pointer;
  margin: 0;
  padding: 0.5rem 0;
  border-bottom: 1px solid #eee;

  &.selected {
    background-color: rgba(255, 255, 255, 0.2);
    font-weight: bold;
  }
}

.bottom {
  margin-top: auto;
}

.sidebar ul {
  list-style: none;
  margin: 0;
  padding: 0;
}
</style>
