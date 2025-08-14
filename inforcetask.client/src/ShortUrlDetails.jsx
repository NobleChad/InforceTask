import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import './ShortUrlDetails.css';

export default function ShortUrlDetails() {
    const { id } = useParams();
    const [linkData, setLinkData] = useState(null);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        // Check authentication
        fetch(`${import.meta.env.VITE_API_BASE_URL}/api/sessions/current`, { credentials: 'include' })
            .then(res => res.json())
            .then(data => {
                if (!data.isAuthenticated) {
                    navigate('/shorturls', { replace: true });
                } else {
                    setIsAuthenticated(true);

                    fetch(`${import.meta.env.VITE_API_BASE_URL}/api/shorturls/${id}`, { credentials: 'include' })
                        .then(res => res.json())
                        .then(data => setLinkData(data))
                        .catch(() => setLinkData(null));
                }
            })
            .catch(() => {
                navigate('/shorturls', { replace: true });
            });
    }, [id, navigate]);

    if (!linkData) return <p>Loading...</p>;

    const createdDate = new Date(linkData.createdAt).toLocaleString();

    return (
        <div className="shorturl-details-container">
            <h2>Short URL Details</h2>
            <p><strong>Original URL:</strong> {linkData.originalUrl}</p>
            <p><strong>Short URL:</strong> {linkData.shortUrl}</p>
            <p><strong>Author:</strong> {linkData.authorEmail}</p>
            <p><strong>Created At:</strong> {createdDate}</p>
        </div>
    );
}
