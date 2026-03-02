<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { userManager } from '@/auth/authService'

const router = useRouter()

onMounted(async () => {
  await userManager.signinCallback()
  router.push({ name: 'Home' })
})
</script>
<template>
  <div>Signing in...</div>
</template>
