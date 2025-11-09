import { useState, useEffect } from 'react'
import { api } from '../services/api'
import './AliasesList.css'

function AliasesList() {
  const [aliases, setAliases] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

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

  return (
    <div className="aliases-container">
      <div className="aliases-header">
        <h2>Your Email Aliases</h2>
        <button onClick={fetchAliases} className="refresh-btn">Refresh</button>
      </div>
      
      {aliases.length === 0 ? (
        <div className="no-aliases">
          <p>No email aliases found.</p>
        </div>
      ) : (
        <div className="aliases-grid">
          {aliases.map((alias) => (
            <div key={alias.id} className="alias-card">
              <div className="alias-header">
                <h3 className="alias-address">{alias.alias}</h3>
                <span className={`status-badge status-${alias.status?.toLowerCase()}`}>
                  {alias.status || 'Unknown'}
                </span>
              </div>
              <div className="alias-details">
                <p className="alias-date">
                  Created: {new Date(alias.createdAt).toLocaleDateString()}
                </p>
                {alias.forwardingAddresses && alias.forwardingAddresses.length > 0 && (
                  <div className="forwarding-addresses">
                    <strong>Forwarding to:</strong>
                    <ul>
                      {alias.forwardingAddresses.map((forward) => (
                        <li key={forward.id}>{forward.forwardingAddress}</li>
                      ))}
                    </ul>
                  </div>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}

export default AliasesList

