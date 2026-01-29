import type { ToDoItem } from '../types/todo.types'

interface ToDoItemProps {
  todo: ToDoItem
  onToggle: (todo: ToDoItem) => void
  onDelete: (id: number) => void
}

export function ToDoCard({ todo, onToggle, onDelete }: ToDoItemProps) {
  // TODO: This feels a bit hacky, improve date handling later
  const today = new Date()
  const yesterday = new Date()
  yesterday.setDate(today.getDate() - 1)
  const isOverdue = todo.completeBy && !todo.isComplete && 
      new Date(todo.completeBy) <= yesterday
  
  return (
    <div
      className={`todo-item ${todo.isComplete ? 'complete' : ''} ${isOverdue ? 'overdue' : ''}`}
    >
      <div className="todo-main">
        <button
          className="toggle-button"
          onClick={() => onToggle(todo)}
          aria-label={todo.isComplete ? 'Mark as incomplete' : 'Mark as complete'}
        >
          [{todo.isComplete ? 'X' : ' '}]
        </button>
        <div className="todo-content">
          <span className="todo-title">{todo.title}</span>
          {todo.completeBy && (
            <span className="todo-date">
              <span className="date-label">DUE: </span>
              <span className="date-value">
                {new Date(todo.completeBy).toLocaleDateString()}
              </span>
            </span>
          )}
          {todo.completedOn && (
            <span className="todo-date completed">
              <span className="date-label">COMPLETED: </span>
              <span className="date-value">
                {new Date(todo.completedOn).toLocaleDateString()}
              </span>
            </span>
          )}
        </div>
      </div>
      <button
        className="delete-button"
        onClick={() => onDelete(todo.id)}
        aria-label={`Delete ${todo.title}`}
      >
        [DEL]
      </button>
    </div>
  )
}
