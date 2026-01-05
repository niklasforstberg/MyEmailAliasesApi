// API service using fetch
const getAuthHeaders = () => {
  const rawToken = localStorage.getItem('token') || ''
  // Normalize token in case it already includes "Bearer "
  const token = rawToken.replace(/^Bearer\s+/i, '').trim()
  const headers = {
    'Content-Type': 'application/json',
  }
  if (token) {
    headers['Authorization'] = `Bearer ${token}`
  }
  return headers
}

const handleResponse = async (response) => {
  if (response.status === 401) {
    localStorage.removeItem('token')
    window.location.reload()
    throw new Error('Unauthorized')
  }
  
  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: response.statusText }))
    throw { response: { data: error, status: response.status } }
  }
  
  return response.json().then(data => ({ data }))
}

const api = {
  get: async (url) => {
    const response = await fetch(`/api${url}`, {
      method: 'GET',
      headers: getAuthHeaders(),
    })
    return handleResponse(response)
  },
  
  post: async (url, data) => {
    const response = await fetch(`/api${url}`, {
      method: 'POST',
      headers: getAuthHeaders(),
      body: JSON.stringify(data),
    })
    return handleResponse(response)
  },
  
  put: async (url, data) => {
    const response = await fetch(`/api${url}`, {
      method: 'PUT',
      headers: getAuthHeaders(),
      body: JSON.stringify(data),
    })
    return handleResponse(response)
  },
  
  delete: async (url) => {
    const response = await fetch(`/api${url}`, {
      method: 'DELETE',
      headers: getAuthHeaders(),
    })
    return handleResponse(response)
  },

  forgotPassword: async (email) => {
    return api.post('/auth/forgot-password', { email })
  },

  resetPassword: async (token, newPassword) => {
    return api.post('/auth/reset-password', { token, newPassword })
  },
}

export { api }

