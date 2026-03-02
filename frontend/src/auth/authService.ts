import { UserManager, WebStorageStateStore } from 'oidc-client-ts'

export const AUTH_CALLBACK_ROUTE = 'auth/callback'

export const userManager = new UserManager({
  authority: import.meta.env.VITE_AUTH_AUTHORITY,
  client_id: import.meta.env.VITE_AUTH_CLIENT_ID,

  redirect_uri: `${window.location.origin}/${AUTH_CALLBACK_ROUTE}`,
  post_logout_redirect_uri: window.location.origin,

  response_type: 'code',
  scope: 'openid profile',

  automaticSilentRenew: true,

  userStore: new WebStorageStateStore({
    store: window.localStorage,
  }),
})
