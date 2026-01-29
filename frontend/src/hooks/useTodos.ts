import { useState, useEffect, useCallback } from 'react'
import { todoApi } from '../services/todoApi'
import { ERROR_MESSAGES } from '../constants/app.constants'
import type { ToDoItem, CreateTodoRequest, UpdateTodoRequest } from '../types/todo.types'

export function useTodos() {
  const [todos, setTodos] = useState<ToDoItem[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const fetchTodos = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const data = await todoApi.fetchTodos()
      setTodos(data)
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : ERROR_MESSAGES.FETCH_FAILED
      setError(errorMessage)
      console.error('Error fetching todos:', err)
    } finally {
      setLoading(false)
    }
  }, [])

  const addTodo = useCallback(async (request: CreateTodoRequest) => {
    try {
      const created = await todoApi.createTodo(request)
      setTodos(prev => [...prev, created])
      return created
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : ERROR_MESSAGES.ADD_FAILED
      setError(errorMessage)
      throw err
    }
  }, [])

  const updateTodo = useCallback(async (id: number, request: UpdateTodoRequest) => {
    try {
      const updated = await todoApi.updateTodo(id, request)
      setTodos(prev => prev.map(t => t.id === updated.id ? updated : t))
      return updated
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : ERROR_MESSAGES.UPDATE_FAILED
      setError(errorMessage)
      throw err
    }
  }, [])

  const deleteTodo = useCallback(async (id: number) => {
    try {
      await todoApi.deleteTodo(id)
      setTodos(prev => prev.filter(t => t.id !== id))
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : ERROR_MESSAGES.DELETE_FAILED
      setError(errorMessage)
      throw err
    }
  }, [])

  const clearCompleted = useCallback(async () => {
    const completedTodos = todos.filter(t => t.isComplete)
    if (completedTodos.length === 0) {
      return
    }

    try {
      await Promise.all(completedTodos.map(todo => todoApi.deleteTodo(todo.id)))
      setTodos(prev => prev.filter(t => !t.isComplete))
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : ERROR_MESSAGES.CLEAR_COMPLETED_FAILED
      setError(errorMessage)
      throw err
    }
  }, [todos])

  const deleteAll = useCallback(async () => {
    if (todos.length === 0) {
      return
    }

    try {
      await Promise.all(todos.map(todo => todoApi.deleteTodo(todo.id)))
      setTodos([])
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : ERROR_MESSAGES.DELETE_ALL_FAILED
      setError(errorMessage)
      throw err
    }
  }, [todos])

  useEffect(() => {
    fetchTodos()
  }, [fetchTodos])

  return {
    todos,
    loading,
    error,
    addTodo,
    updateTodo,
    deleteTodo,
    clearCompleted,
    deleteAll,
    refetch: fetchTodos,
  }
}
