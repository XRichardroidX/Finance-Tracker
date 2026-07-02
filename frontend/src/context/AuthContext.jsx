import { createContext, useContext, useState } from 'react'
import { setToken as setApiToken } from '../api/client'

// Holds "is someone logged in, and as whom" for the whole app.
// Pages read this with useAuth() instead of managing their own token state.
const AuthContext = createContext(null)

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null) // { email } | null

  // Called after a successful login or register response ({ token, email }).
  function signIn({ token, email }) {
    setApiToken(token)
    setUser({ email })
  }

  function signOut() {
    setApiToken(null)
    setUser(null)
  }

  return (
    <AuthContext.Provider value={{ user, signIn, signOut }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used inside <AuthProvider>')
  return ctx
}
