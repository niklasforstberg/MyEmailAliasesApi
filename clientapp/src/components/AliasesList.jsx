import { useState, useEffect } from 'react'
import { api } from '../services/api'
import './AliasesList.css'

function AliasesList() {
  const [aliases, setAliases] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [searchTerm, setSearchTerm] = useState('')

  useEffect(() => {
    fetchAliases()
  }, [])

  const fetchAliases = async () => {
    try {
      setLoading(true)
      setError('')
      const response = await api.get('/aliases')
      setAliases(response.data)
    } catch (err) {
      setError(err.response?.data || 'Failed to load aliases')
    } finally {
      setLoading(false)
    }
  }

  if (loading) {
    return (
      <div className="aliases-container">
        <div className="loading">Loading aliases...</div>
      </div>
    )
  }

  if (error) {
    return (
      <div className="aliases-container">
        <div className="error-message">{error}</div>
        <button onClick={fetchAliases} className="retry-btn">Retry</button>
      </div>
    )
  }

  const filteredAliases = aliases.filter(alias =>
    alias.alias.toLowerCase().includes(searchTerm.toLowerCase())
  )

  return (
    <div className="aliases-container">
      <div className="aliases-header">
        <h2>Your Email Aliases</h2>
        <button onClick={fetchAliases} className="refresh-btn">Refresh</button>
      </div>
      
      <input
        type="text"
        className="search-input"
        placeholder="Search aliases..."
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
      />
      
      {filteredAliases.length === 0 ? (
        <div className="no-aliases">
          <p>{searchTerm ? 'No aliases match your search.' : 'No email aliases found.'}</p>
        </div>
      ) : (
        <div className="aliases-list">
          {filteredAliases.map((alias) => (
            <div key={alias.id} className="alias-item">
              {alias.alias}
            </div>
          ))}
        </div>
      )}
    </div>
  )
}

export default AliasesList

