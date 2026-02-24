type HttpMethod = 'GET' | 'POST' | 'PUT' | 'DELETE'

interface RequestOptions<TBody = unknown> {
  method?: HttpMethod
  body?: TBody
}

const BASE_URL = import.meta.env.VITE_BASE_API_URL

export const request = async <TResponse, TBody = unknown>(
  url: string,
  options: RequestOptions<TBody> = {},
): Promise<TResponse> => {
  const { method = 'GET', body } = options

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

  return res.json() as Promise<TResponse>
}
