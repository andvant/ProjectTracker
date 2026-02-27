import { ApiError, type ProblemDetails } from '@/types/api'

type HttpMethod = 'GET' | 'POST' | 'PUT' | 'DELETE'

interface RequestOptions<TBody = unknown> {
  method?: HttpMethod
  body?: TBody
}

const BASE_URL = import.meta.env.VITE_API_BASE_URL

const sendRequest = async <TResponse, TBody = unknown>(
  url: string,
  options: RequestOptions<TBody> = {},
): Promise<TResponse> => {
  const { method = 'GET', body } = options

  const res = await fetch(new URL(url, BASE_URL), {
    method,
    headers: body
      ? {
          'Content-Type': 'application/json',
        }
      : undefined,
    body: body ? JSON.stringify(body) : undefined,
  })

  if (res.status === 204 || res.status === 304) {
    return undefined as TResponse
  }

  const contentType = res.headers.get('content-type')

  const data = contentType?.includes('json')
    ? await res.json().catch(() => res.statusText)
    : await res.text().catch(() => res.statusText)

  if (!res.ok) {
    if (data && typeof data === 'object' && 'title' in data) {
      throw new ApiError(res.status, data as ProblemDetails)
    }

    throw new ApiError(res.status, undefined, data)
  }

  return data as TResponse
}

const apiClient = {
  get: <TResponse>(url: string): Promise<TResponse> =>
    sendRequest<TResponse>(url, { method: 'GET' }),

  post: <TResponse, TBody>(url: string, body: TBody): Promise<TResponse> =>
    sendRequest<TResponse, TBody>(url, { method: 'POST', body }),

  put: <TBody>(url: string, body: TBody): Promise<void> =>
    sendRequest<void, TBody>(url, { method: 'PUT', body }),

  delete: (url: string): Promise<void> => sendRequest(url, { method: 'DELETE' }),
}

export default apiClient
