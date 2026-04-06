import React from 'react'

export function AnalyticsIcon({ className }) {
    return (
        <svg
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 100 100"
            width="100%"
            height="100%"
            className={className}
        >
            <rect x="20" y="45" width="14" height="40" rx="3" fill="currentColor" opacity="0.4" />
            <rect x="43" y="25" width="14" height="60" rx="3" fill="currentColor" opacity="0.6" />
            <rect x="66" y="10" width="14" height="75" rx="3" fill="currentColor" opacity="0.8" />

            <line x1="15" y1="80" x2="78" y2="26" stroke="currentColor" strokeWidth="10" strokeLinecap="round" />

            <polygon points="68,16 92,12 84,35" fill="currentColor" />
        </svg>
    )
}