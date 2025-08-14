import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import './ShortUrlDetails.css';

export default function ShortUrlDetails() {
    const { id } = useParams();
    const [linkData, setLinkData] = useState(null);

    useEffect(() => {
        fetch(`${import.meta.env.VITE_API_BASE_URL}/api/shorturls/${id}`, { credentials: 'include' })
            .then(res => res.json())
            .then(data => setLinkData(data))
            .catch(() => setLinkData(null));
    }, [id]);

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
