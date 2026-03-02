import '@/assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { initAuth } from '@/auth/useAuth'
import App from '@/App.vue'
import router from '@/router'

await initAuth()

const app = createApp(App)
const pinia = createPinia()

app.use(router)
app.use(pinia)

app.mount('#app')
