<script setup lang="ts">
import { userManager } from '@/auth/authService'

const signIn = async () => {
  await userManager.signinRedirect()
}
</script>
<template>
  <button @click="signIn">Sign in</button>
</template>
