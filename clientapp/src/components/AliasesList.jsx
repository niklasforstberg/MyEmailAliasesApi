import { useState, useEffect } from 'react'
import { api } from '../services/api'
import './AliasesList.css'

// Search icon
function SearchIcon() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <circle cx="11" cy="11" r="8" />
      <line x1="21" y1="21" x2="16.65" y2="16.65" />
    </svg>
  )
}

// Refresh icon
function RefreshIcon() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <polyline points="23 4 23 10 17 10" />
      <polyline points="1 20 1 14 7 14" />
      <path d="M3.51 9a9 9 0 0 1 14.85-3.36L23 10M1 14l4.64 4.36A9 9 0 0 0 20.49 15" />
    </svg>
  )
}

// Mail icon for alias cards
function MailIcon() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <rect x="2" y="4" width="20" height="16" rx="2" />
      <path d="M22 7l-10 7L2 7" />
    </svg>
  )
}

// Forward arrow icon
function ForwardIcon() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <polyline points="9 18 15 12 9 6" />
    </svg>
  )
}

// Empty mailbox icon
function EmptyMailboxIcon() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
      <path d="M22 12h-6l-2 3h-4l-2-3H2" />
      <path d="M5.45 5.11L2 12v6a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2v-6l-3.45-6.89A2 2 0 0 0 16.76 4H7.24a2 2 0 0 0-1.79 1.11z" />
    </svg>
  )
}

function AliasesList() {
  const [aliases, setAliases] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [searchTerm, setSearchTerm] = useState('')
  const [refreshing, setRefreshing] = useState(false)

  useEffect(() => {
    fetchAliases()
  }, [])

  const fetchAliases = async () => {
    try {
      setRefreshing(true)
      setError('')
      const response = await api.get('/aliases')
      setAliases(response.data)
    } catch (err) {
      setError(err.response?.data || 'Failed to load aliases')
    } finally {
      setLoading(false)
      setRefreshing(false)
    }
  }

  if (loading) {
    return (
      <div className="aliases-container">
        <div className="loading">
          <div className="loading-spinner" />
          <div className="loading-text">Loading your aliases...</div>
        </div>
      </div>
    )
  }

  if (error) {
    return (
      <div className="aliases-container">
        <div className="error-container">
          <div className="error-message">{error}</div>
          <button onClick={fetchAliases} className="retry-btn">
            <RefreshIcon />
            <span>Try Again</span>
          </button>
        </div>
      </div>
    )
  }

  const filteredAliases = aliases.filter(alias =>
    alias.alias.toLowerCase().includes(searchTerm.toLowerCase())
  )

  return (
    <div className="aliases-container">
      <div className="aliases-header">
        <div>
          <h2>Your Email Aliases</h2>
          <div className="aliases-count">
            {filteredAliases.length} {filteredAliases.length === 1 ? 'alias' : 'aliases'}
            {searchTerm && ` matching "${searchTerm}"`}
          </div>
        </div>
        <button
          onClick={fetchAliases}
          className={`refresh-btn ${refreshing ? 'loading' : ''}`}
          disabled={refreshing}
        >
          <RefreshIcon />
          <span>{refreshing ? 'Refreshing...' : 'Refresh'}</span>
        </button>
      </div>

      <div className="search-wrapper">
        <SearchIcon />
        <input
          type="text"
          className="search-input"
          placeholder="Search aliases..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
        />
      </div>

      {filteredAliases.length === 0 ? (
        <div className="no-aliases">
          <div className="empty-icon">
            <EmptyMailboxIcon />
          </div>
          <p>{searchTerm ? 'No aliases match your search' : 'No email aliases found'}</p>
          {searchTerm && (
            <p className="subtext">Try adjusting your search terms</p>
          )}
        </div>
      ) : (
        <div className="aliases-list">
          {filteredAliases.map((alias) => (
            <div key={alias.id} className="alias-item">
              <div className="alias-email">
                <div className="alias-icon">
                  <MailIcon />
                </div>
                <span className="alias-text">{alias.alias}</span>
              </div>

              {alias.forwardings && alias.forwardings.length > 0 && (
                <div className="alias-forwardings">
                  <div className="forwardings-label">Forwards to</div>
                  {alias.forwardings.map((forwarding, index) => (
                    <div key={index} className="forwarding-item">
                      <ForwardIcon />
                      <span>{forwarding.forwardTo || forwarding}</span>
                    </div>
                  ))}
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  )
}

export default AliasesList
