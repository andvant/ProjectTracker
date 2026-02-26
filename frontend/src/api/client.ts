type HttpMethod = 'GET' | 'POST' | 'PUT' | 'DELETE'

interface RequestOptions<TBody = unknown> {
  method?: HttpMethod
  body?: TBody
}

const BASE_URL = import.meta.env.VITE_API_BASE_URL

const request = async <TResponse, TBody = unknown>(
  url: string,
  options: RequestOptions<TBody> = {},
): Promise<TResponse> => {
  const { method, body } = options

  const res = await fetch(`${BASE_URL}/${url}`, {
    method,
    headers: body
      ? {
          'Content-Type': 'application/json',
        }
      : undefined,
    body: body ? JSON.stringify(body) : undefined,
  })

  if (!res.ok) {
    const message = await res.text().catch(() => res.statusText)
    throw new Error(`API Error ${res.status}: ${message}`)
  }

  if (res.status === 204 || res.status === 304) {
    return null as TResponse
  }

  return res.json() as Promise<TResponse>
}

const client = {
  get: <TResponse>(url: string): Promise<TResponse> => request<TResponse>(url, { method: 'GET' }),

  post: <TResponse, TBody>(url: string, body: TBody): Promise<TResponse> =>
    request<TResponse, TBody>(url, { method: 'POST', body }),

  put: <TResponse, TBody>(url: string, body: TBody): Promise<TResponse> =>
    request<TResponse, TBody>(url, { method: 'PUT', body }),

  delete: <TResponse>(url: string): Promise<TResponse> =>
    request<TResponse>(url, { method: 'DELETE' }),
}

export default client
