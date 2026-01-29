export interface ToDoItem {
  id: number
  title: string
  isComplete: boolean
  completeBy?: string
  completedOn?: string
}

export interface CreateTodoRequest {
  title: string
  isComplete: boolean
  completeBy?: string
}

export interface UpdateTodoRequest {
  isComplete: boolean
  completedOn?: string | null
}
