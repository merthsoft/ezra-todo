import { useState, useCallback } from 'react'
import { useAuth } from '../context/AuthContext'
import { useSpinner } from '../hooks/useSpinner'

export function LoginPage() {
  const { login, register, loading, error, clearError } = useAuth()
  const [isRegisterMode, setIsRegisterMode] = useState(false)
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [localError, setLocalError] = useState<string | null>(null)
  const spinnerChar = useSpinner(loading)

  const handleSubmit = useCallback(async (e: React.FormEvent) => {
    e.preventDefault()
    setLocalError(null)
    clearError()

    if (!email.trim() || !password.trim()) {
      setLocalError('Email and password are required')
      return
    }

    if (isRegisterMode) {
      if (password !== confirmPassword) {
        setLocalError('Passwords do not match')
        return
      }
      if (password.length < 8) {
        setLocalError('Password must be at least 8 characters')
        return
      }
      await register({ email: email.trim(), password })
    } else {
      await login({ email: email.trim(), password })
    }
  }, [email, password, confirmPassword, isRegisterMode, login, register, clearError])

  const toggleMode = useCallback(() => {
    setIsRegisterMode(prev => !prev)
    setLocalError(null)
    clearError()
    setConfirmPassword('')
  }, [clearError])

  const displayError = localError || error

  return (
    <div className="app-container">
      <header className="app-header">
        <h1 className="app-title">RETRO TODO</h1>
      </header>

      <main className="main-content">
        <section className="login-section">
          <div className="card">
            <div className="section-header">
              <h2 className="section-title">
                &gt; {isRegisterMode ? 'REGISTER' : 'LOGIN'}_
              </h2>
            </div>

            <form className="login-form" onSubmit={handleSubmit}>
              <div className="form-group">
                <label htmlFor="email" className="form-label">EMAIL:</label>
                <div className="input-wrapper">
                  <span className="prompt">&gt;</span>
                  <input
                    id="email"
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    placeholder="ENTER EMAIL..."
                    className="todo-input"
                    autoComplete="email"
                    disabled={loading}
                  />
                </div>
              </div>

              <div className="form-group">
                <label htmlFor="password" className="form-label">PASSWORD:</label>
                <div className="input-wrapper">
                  <span className="prompt">&gt;</span>
                  <input
                    id="password"
                    type="password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    placeholder="ENTER PASSWORD..."
                    className="todo-input"
                    autoComplete={isRegisterMode ? 'new-password' : 'current-password'}
                    disabled={loading}
                  />
                </div>
              </div>

              {isRegisterMode && (
                <div className="form-group">
                  <label htmlFor="confirmPassword" className="form-label">CONFIRM:</label>
                  <div className="input-wrapper">
                    <span className="prompt">&gt;</span>
                    <input
                      id="confirmPassword"
                      type="password"
                      value={confirmPassword}
                      onChange={(e) => setConfirmPassword(e.target.value)}
                      placeholder="CONFIRM PASSWORD..."
                      className="todo-input"
                      autoComplete="new-password"
                      disabled={loading}
                    />
                  </div>
                </div>
              )}

              {displayError && (
                <div className="error-message" role="alert">
                  <span>! ERROR: {displayError}</span>
                </div>
              )}

              <div className="login-actions">
                <button
                  type="submit"
                  className="add-button login-button"
                  disabled={loading || !email.trim() || !password.trim()}
                >
                  {loading ? `${spinnerChar} PROCESSING ${spinnerChar}` : `[${isRegisterMode ? 'REGISTER' : 'LOGIN'}]`}
                </button>
              </div>

              <div className="login-toggle">
                <span className="toggle-text">
                  {isRegisterMode ? 'ALREADY HAVE AN ACCOUNT?' : 'NEED AN ACCOUNT?'}
                </span>
                <button
                  type="button"
                  className="toggle-button-link"
                  onClick={toggleMode}
                  disabled={loading}
                >
                  [{isRegisterMode ? 'LOGIN' : 'REGISTER'}]
                </button>
              </div>
            </form>
          </div>
        </section>
      </main>
    </div>
  )
}
