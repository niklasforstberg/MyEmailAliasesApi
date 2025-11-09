import { createContext, useState, useEffect } from 'react'
import { api } from '../services/api'

export const AuthContext = createContext()

export function AuthProvider({ children }) {
  const [isAuthenticated, setIsAuthenticated] = useState(false)
  const [user, setUser] = useState(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const token = localStorage.getItem('token')
    if (token) {
      // Verify token by fetching user info
      console.log('Verifying token')
      console.log(token)
      api.get('/auth/me')
        .then(response => {
          console.log('Token verified')
          console.log(response.data)
          setUser(response.data)
          setIsAuthenticated(true)
        })
        .catch(() => {
          // Token is invalid, remove it
          console.log('Token invalid')
          localStorage.removeItem('token')
          setIsAuthenticated(false)
        })
        .finally(() => {
          setLoading(false)
        })
    } else {
      setLoading(false)
    }
  }, [])

  const login = async (email, password) => {
    try {
      const response = await api.post('/auth/login', { email, password })
      // API returns { Token: "..." } with capital T
      const token = response.data.Token || response.data.token
      if (!token) {
        return { 
          success: false, 
          error: 'No token received from server' 
        }
      }
      localStorage.setItem('token', token)
      
      // Fetch user info
      const userResponse = await api.get('/auth/me')
      setUser(userResponse.data)
      setIsAuthenticated(true)
      
      return { success: true }
    } catch (error) {
      // Handle error response - could be string or object
      let errorMessage = 'Login failed'
      if (error.response?.data) {
        errorMessage = typeof error.response.data === 'string' 
          ? error.response.data 
          : error.response.data.message || error.response.data
      } else if (error.message) {
        errorMessage = error.message
      }
      return { 
        success: false, 
        error: errorMessage
      }
    }
  }

  const logout = () => {
    localStorage.removeItem('token')
    setIsAuthenticated(false)
    setUser(null)
  }

  return (
    <AuthContext.Provider value={{ isAuthenticated, user, login, logout, loading }}>
      {children}
    </AuthContext.Provider>
  )
}

