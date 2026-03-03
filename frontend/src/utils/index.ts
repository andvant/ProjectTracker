import type { ProblemDetails, ValidationErrors } from '@/types/api'

interface ValueLabel {
  value: string
  label: string
}

type Enum = Record<string, ValueLabel>

export const getEnumOptions = <T extends Enum>(enumObject: T): ValueLabel[] =>
  Object.values(enumObject)

export const getEnumLabel = <T extends Enum>(enumObject: T, enumValue: string): string =>
  getEnumOptions(enumObject).find((e) => e.value === enumValue)!.label

export const createDefaultErrors = <T extends object>(source: T): ValidationErrors<T> => {
  const errors = {} as Record<keyof T, string>

  for (const key in source) {
    errors[key] = ''
  }

  return {
    ...errors,
    general: '',
  }
}

const toCamelCase = (value: string): string => value.charAt(0).toLowerCase() + value.slice(1)

export const applyErrorsFromApi = (
  errors: Record<string, string>,
  problem: ProblemDetails,
): void => {
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

export const formatDate = (dateString?: string): string | undefined => {
  if (!dateString) return undefined

  const date = new Date(dateString)

  return new Intl.DateTimeFormat('en-GB', {
    dateStyle: 'long',
    timeStyle: 'short',
  }).format(date)
}

export const removeNonDigits = (e: Event) => {
  const target = e.target as HTMLInputElement
  target.value = target.value.replace(/[^0-9]/, '')
}
