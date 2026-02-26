export interface ProblemDetails {
  type?: string
  title?: string
  status?: number
  detail?: string
  instance?: string
  errors?: Record<string, string[]>
}

export type GeneralError = {
  general?: string
}

export class ApiError extends Error {
  problem?: ProblemDetails
  status: number

  constructor(status: number, problem?: ProblemDetails, message?: string) {
    super(message ?? problem?.title ?? `API Error ${status}`)

    this.status = status
    this.problem = problem
  }
}
