import type { ProblemDetails } from '@/types/api'

const toCamelCase = (value: string) => value.charAt(0).toLowerCase() + value.slice(1)

export const applyErrorsFromApi = (errors: Record<string, string>, problem: ProblemDetails) => {
  if (!problem.errors) {
    errors['general' as keyof Record<string, string>] = problem.title || 'Unexpected error occurred'
    return
  }

  for (const field of Object.keys(problem.errors)) {
    const camelCaseField = toCamelCase(field) as keyof Record<string, string>

    if (camelCaseField in errors) {
      errors[camelCaseField] = problem.errors[field]?.join('; ') ?? ''
    }
  }
}
