import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { todoApi } from '../services/todoApi'
import type { ToDoItem, CreateTodoRequest, UpdateTodoRequest } from '../types/todo.types'

const TODOS_QUERY_KEY = ['todos'] as const

export function useTodos() {
  const queryClient = useQueryClient()

  const {
    data: todos = [],
    isLoading: loading,
    error: queryError,
  } = useQuery({
    queryKey: TODOS_QUERY_KEY,
    queryFn: () => todoApi.fetchTodos(),
  })

  const error = queryError instanceof Error ? queryError.message : queryError ? String(queryError) : null

  const addMutation = useMutation({
    mutationFn: (request: CreateTodoRequest) => todoApi.createTodo(request),
    onSuccess: (newTodo: ToDoItem) => {
      queryClient.setQueryData<ToDoItem[]>(TODOS_QUERY_KEY, (old: ToDoItem[] | undefined) => [...(old ?? []), newTodo])
    },
  })

  const updateMutation = useMutation({
    mutationFn: ({ id, request }: { id: number; request: UpdateTodoRequest }) => todoApi.updateTodo(id, request),
    onSuccess: (updatedTodo: ToDoItem) => {
      queryClient.setQueryData<ToDoItem[]>(TODOS_QUERY_KEY, (old: ToDoItem[] | undefined) =>
        old?.map((t: ToDoItem) => (t.id === updatedTodo.id ? updatedTodo : t)) ?? []
      )
    },
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => todoApi.deleteTodo(id),
    onSuccess: (_: void, id: number) => {
      queryClient.setQueryData<ToDoItem[]>(TODOS_QUERY_KEY, (old: ToDoItem[] | undefined) => old?.filter((t: ToDoItem) => t.id !== id) ?? [])
    },
  })

  const clearCompletedMutation = useMutation({
    mutationFn: async () => {
      const completedTodos = todos.filter((t: ToDoItem) => t.isComplete)
      await Promise.all(completedTodos.map((todo: ToDoItem) => todoApi.deleteTodo(todo.id)))
      return completedTodos.map((t: ToDoItem) => t.id)
    },
    onSuccess: (deletedIds: number[]) => {
      queryClient.setQueryData<ToDoItem[]>(TODOS_QUERY_KEY, (old: ToDoItem[] | undefined) =>
        old?.filter((t: ToDoItem) => !deletedIds.includes(t.id)) ?? []
      )
    },
  })

  const deleteAllMutation = useMutation({
    mutationFn: async () => {
      await Promise.all(todos.map((todo: ToDoItem) => todoApi.deleteTodo(todo.id)))
    },
    onSuccess: () => {
      queryClient.setQueryData<ToDoItem[]>(TODOS_QUERY_KEY, [])
    },
  })

  const addTodo = async (request: CreateTodoRequest) => {
    return addMutation.mutateAsync(request)
  }

  const updateTodo = async (id: number, request: UpdateTodoRequest) => {
    return updateMutation.mutateAsync({ id, request })
  }

  const deleteTodo = async (id: number) => {
    return deleteMutation.mutateAsync(id)
  }

  const clearCompleted = async () => {
    if (todos.filter((t: ToDoItem) => t.isComplete).length === 0) return
    return clearCompletedMutation.mutateAsync()
  }

  const deleteAll = async () => {
    if (todos.length === 0) return
    return deleteAllMutation.mutateAsync()
  }

  return {
    todos,
    loading,
    error,
    addTodo,
    updateTodo,
    deleteTodo,
    clearCompleted,
    deleteAll,
  }
}
