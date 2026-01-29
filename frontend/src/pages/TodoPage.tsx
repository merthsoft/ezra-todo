import { useState, useCallback } from 'react'
import { ToDoCard } from '../components/ToDoCard'
import { useTodos } from '../hooks/useTodos'
import { useSpinner } from '../hooks/useSpinner'
import { useAuth } from '../context/AuthContext'
import type { ToDoItem, CreateTodoRequest, UpdateTodoRequest } from '../types/todo.types'

export function TodoPage() {
  const { todos, loading, error, addTodo, updateTodo, deleteTodo, clearCompleted, deleteAll } = useTodos()
  const { userEmail, logout } = useAuth()
  const [newTodo, setNewTodo] = useState('')
  const [completeBy, setCompleteBy] = useState('')
  const spinnerChar = useSpinner(loading)

  const handleAddTodo = useCallback(async (e: React.FormEvent) => {
    e.preventDefault()
    if (!newTodo.trim()) return

    const request: CreateTodoRequest = {
      title: newTodo.trim(),
      isComplete: false,
    }

    if (completeBy) {
      const date = new Date(completeBy)
      request.completeBy = date.toISOString()
    }

    await addTodo(request)
    setNewTodo('')
    setCompleteBy('')
  }, [newTodo, completeBy, addTodo])

  const handleToggleTodo = useCallback(async (todo: ToDoItem) => {
    const request: UpdateTodoRequest = {
      isComplete: !todo.isComplete,
      completedOn: !todo.isComplete ? new Date().toISOString() : null,
    }

    await updateTodo(todo.id, request)
  }, [updateTodo])

  const handleDeleteTodo = useCallback(async (id: number) => {
    await deleteTodo(id)
  }, [deleteTodo])

  const handleClearCompleted = useCallback(async () => {
    await clearCompleted()
  }, [clearCompleted])

  const handleDeleteAll = useCallback(async () => {
    await deleteAll()
  }, [deleteAll])

  const handleLogout = useCallback(() => {
    logout()
  }, [logout])

  const completedCount = todos.filter(t => t.isComplete).length
  const hasTodos = todos.length > 0

  return (
    <div className="app-container">
      <header className="app-header">
        <h1 className="app-title">RETRO TODO</h1>
        <div className="user-info">
          <span className="user-email">[{userEmail}]</span>
          <button className="logout-button" onClick={handleLogout}>
            [LOGOUT]
          </button>
        </div>
      </header>

      <main className="main-content">
        <section className="todo-section" aria-labelledby="todo-heading">
          <div className="card">
            <div className="section-header">
              <h2 id="todo-heading" className="section-title">&gt; TASK MANAGER_</h2>
              <div className="stats-display">
                <span className="stat">[TOTAL: {todos.length}]</span>
                <span className="stat complete">[DONE: {completedCount}]</span>
              </div>
            </div>

            <form className="todo-form" onSubmit={handleAddTodo}>
              <div className="input-wrapper">
                <span className="prompt">&gt;</span>
                <input
                  type="text"
                  value={newTodo}
                  onChange={(e) => setNewTodo(e.target.value)}
                  placeholder="ENTER NEW TASK..."
                  className="todo-input"
                  aria-label="New todo item"
                />
              </div>
              <div className="date-wrapper">
                <label htmlFor="completeBy" className="date-label">DUE:</label>
                <input
                  id="completeBy"
                  type="date"
                  value={completeBy}
                  onChange={(e) => setCompleteBy(e.target.value)}
                  className="date-input"
                  aria-label="Complete by date"
                />
              </div>
              <button type="submit" className="add-button" disabled={!newTodo.trim() || loading}>
                [ADD]
              </button>
            </form>

            <div className="bulk-actions">
              <button
                type="button"
                className="bulk-button"
                onClick={handleClearCompleted}
                disabled={completedCount === 0 || loading}
              >
                [CLEAR COMPLETED]
              </button>
              <button
                type="button"
                className="bulk-button danger"
                onClick={handleDeleteAll}
                disabled={!hasTodos || loading}
              >
                [DELETE ALL]
              </button>
            </div>

            {error && (
              <div className="error-message" role="alert" aria-live="polite">
                <span>! ERROR: {error}</span>
              </div>
            )}

            {loading && todos.length === 0 && (
              <div className="loading" role="status" aria-live="polite">
                <span className="loading-text">{spinnerChar} LOADING DATA {spinnerChar}</span>
              </div>
            )}

            <div className="todo-list">
              {todos.length === 0 && !loading ? (
                <div className="empty-state">
                  <p>NO TASKS FOUND</p>
                  <p className="hint">TYPE ABOVE TO ADD YOUR FIRST TASK</p>
                </div>
              ) : (
                todos.map((todo) => (
                  <ToDoCard
                    key={todo.id}
                    todo={todo}
                    onToggle={handleToggleTodo}
                    onDelete={handleDeleteTodo}
                  />
                ))
              )}
            </div>

          </div>
        </section>
      </main>
    </div>
  )
}
