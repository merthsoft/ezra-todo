export const API_ENDPOINTS = {
  TODOS: '/api/todo',
  TODO_BY_ID: (id: number) => `/api/todo/${id}`,
  AUTH_LOGIN: '/api/auth/login',
  AUTH_REGISTER: '/api/auth/register',
} as const

export const STORAGE_KEYS = {
  AUTH_TOKEN: 'auth_token',
  USER_EMAIL: 'user_email',
} as const

export const SPINNER_CHARS = ['/', '|', '\\', '|'] as const
export const SPINNER_INTERVAL_MS = 150

export const ERROR_MESSAGES = {
  FETCH_FAILED: 'Failed to fetch todos',
  ADD_FAILED: 'Failed to add todo',
  UPDATE_FAILED: 'Failed to update todo',
  DELETE_FAILED: 'Failed to delete todo',
  CLEAR_COMPLETED_FAILED: 'Failed to clear completed todos',
  DELETE_ALL_FAILED: 'Failed to delete all todos',
  LOGIN_FAILED: 'Invalid email or password',
  REGISTER_FAILED: 'Failed to register',
} as const
