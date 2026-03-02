<script setup lang="ts">
import { userManager } from '@/auth/authService'

const signOut = async () => {
  await userManager.signoutRedirect()
}
</script>
<template>
  <div @click="signOut">Sign out</div>
</template>
