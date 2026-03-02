<script setup lang="ts">
import { userManager } from '@/auth/authService'

const signOut = async () => {
  await userManager.signoutRedirect()
}
</script>
<template>
  <button @click="signOut">Sign out</button>
</template>
