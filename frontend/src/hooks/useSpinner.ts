import { useState, useEffect } from 'react'
import { SPINNER_CHARS, SPINNER_INTERVAL_MS } from '../constants/app.constants'

export function useSpinner(isActive: boolean) {
  const [spinnerIndex, setSpinnerIndex] = useState(0)

  useEffect(() => {
    if (isActive) {
      const interval = setInterval(() => {
        setSpinnerIndex(prev => (prev + 1) % SPINNER_CHARS.length)
      }, SPINNER_INTERVAL_MS)
      return () => clearInterval(interval)
    }
  }, [isActive])

  return SPINNER_CHARS[spinnerIndex]
}
