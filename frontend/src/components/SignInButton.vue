<script setup lang="ts">
import { userManager } from '@/auth/authService'

const signIn = async () => {
  await userManager.signinRedirect()
}
</script>
<template>
  <div @click="signIn">Sign in</div>
</template>
