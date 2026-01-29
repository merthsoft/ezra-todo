import { API_ENDPOINTS, STORAGE_KEYS } from '../constants/app.constants'
import type { ToDoItem, CreateTodoRequest, UpdateTodoRequest } from '../types/todo.types'

class TodoApiService {
  private getAuthHeaders(): HeadersInit {
    const token = localStorage.getItem(STORAGE_KEYS.AUTH_TOKEN)
    const headers: HeadersInit = {
      'Content-Type': 'application/json',
    }
    if (token) {
      headers['Authorization'] = `Bearer ${token}`
    }
    return headers
  }

  private async handleResponse<T>(response: Response): Promise<T> {
    if (response.status === 401) {
      localStorage.removeItem(STORAGE_KEYS.AUTH_TOKEN)
      localStorage.removeItem(STORAGE_KEYS.USER_EMAIL)
      window.location.reload()
      throw new Error('Unauthorized')
    }
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }
    return response.json()
  }

  async fetchTodos(): Promise<ToDoItem[]> {
    const response = await fetch(API_ENDPOINTS.TODOS, {
      headers: this.getAuthHeaders(),
    })
    return this.handleResponse<ToDoItem[]>(response)
  }

  async createTodo(request: CreateTodoRequest): Promise<ToDoItem> {
    const response = await fetch(API_ENDPOINTS.TODOS, {
      method: 'POST',
      headers: this.getAuthHeaders(),
      body: JSON.stringify(request),
    })
    return this.handleResponse<ToDoItem>(response)
  }

  async updateTodo(id: number, request: UpdateTodoRequest): Promise<ToDoItem> {
    const response = await fetch(API_ENDPOINTS.TODO_BY_ID(id), {
      method: 'PUT',
      headers: this.getAuthHeaders(),
      body: JSON.stringify(request),
    })
    return this.handleResponse<ToDoItem>(response)
  }

  async deleteTodo(id: number): Promise<void> {
    const response = await fetch(API_ENDPOINTS.TODO_BY_ID(id), {
      method: 'DELETE',
      headers: this.getAuthHeaders(),
    })
    if (response.status === 401) {
      localStorage.removeItem(STORAGE_KEYS.AUTH_TOKEN)
      localStorage.removeItem(STORAGE_KEYS.USER_EMAIL)
      window.location.reload()
      throw new Error('Unauthorized')
    }
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }
  }
}

export const todoApi = new TodoApiService()
