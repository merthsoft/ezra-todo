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
  title?: string
  isComplete?: boolean
  completeBy?: string | null
  completedOn?: string | null
}
