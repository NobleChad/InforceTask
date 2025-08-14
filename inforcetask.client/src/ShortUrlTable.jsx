import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './ShortUrlTable.css';

export default function ShortUrlTable({ userEmail }) {
    const [links, setLinks] = useState([]);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const navigate = useNavigate();

    const fetchLinks = () => {
        fetch(`${import.meta.env.VITE_API_BASE_URL}/api/shorturls`, { credentials: 'include' })
            .then(res => res.json())
            .then(data => setLinks(data))
            .catch(() => setLinks([]));
    };

    useEffect(() => {
        fetchLinks();

        setIsAuthenticated(!!userEmail);

        fetch(`${import.meta.env.VITE_API_BASE_URL}/api/sessions/current`, { credentials: 'include' })
            .then(res => res.json())
            .then(data => setIsAuthenticated(data.isAuthenticated))
            .catch(() => setIsAuthenticated(false));
    }, [userEmail]);

    const handleRowClick = (id) => {
        navigate(`/shorturl/${id}`);
    };

    const handleCreate = async () => {
        const originalUrl = prompt("Enter the URL to shorten:");
        if (!originalUrl) return;

        try {
            const res = await fetch(`${import.meta.env.VITE_API_BASE_URL}/api/shorturls`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(originalUrl),
                credentials: 'include'
            });

            const data = await res.json();

            if (res.ok) {
                alert(data.message + (data.shortUrl ? `: ${data.shortUrl}` : ''));
                fetchLinks();
            } else {
                alert(data.message || "Error creating short URL");
            }
        } catch (err) {
            alert("Network error: " + err.message);
        }
    };

    const handleDelete = async (linkId) => {
        if (!window.confirm("Are you sure you want to delete this link?")) return;

        try {
            const res = await fetch(`${import.meta.env.VITE_API_BASE_URL}/api/shorturls/${linkId}`, {
                method: 'DELETE',
                credentials: 'include'
            });

            const data = await res.json();

            if (res.ok) {
                alert(data.message || "Link deleted successfully");
                fetchLinks();
            } else {
                alert(data.message || "Error deleting link");
            }
        } catch (err) {
            alert("Network error: " + err.message);
        }
    };

    const handleEdit = async (linkId, originalUrl) => {
        const newUrl = prompt("Enter the new Original URL:", originalUrl);
        if (!newUrl) return;

        try {
            const res = await fetch(`${import.meta.env.VITE_API_BASE_URL}/api/shorturls/${linkId}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(newUrl),
                credentials: 'include'
            });

            const data = await res.json();

            if (res.ok) {
                alert(data.message || "Link updated successfully");
                fetchLinks();
            } else {
                alert(data.message || "Error updating link");
            }
        } catch (err) {
            alert("Network error: " + err.message);
        }
    };

    return (
        <div className="shorturl-container">
            {isAuthenticated && (
                <button onClick={handleCreate}>Create Short URL</button>
            )}

            {links.length === 0 ? (
                <p>No links found.</p>
            ) : (
                <table>
                    <thead>
                        <tr>
                            <th>Original URL</th>
                            <th>Short URL</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {links.map(link => (
                            <tr key={link.shortCode} onClick={() => handleRowClick(link.id)} style={{ cursor: 'pointer' }}>
                                <td>{link.originalUrl}</td>
                                <td>{link.shortUrl}</td>
                                <td>
                                    {link.canEdit && (
                                        <div>
                                            <button onClick={(e) => { e.stopPropagation(); handleEdit(link.id, link.originalUrl); }}>Edit</button>
                                            <button
                                                onClick={(e) => { e.stopPropagation(); handleDelete(link.id); }}
                                                className="delete"
                                            >
                                                Delete
                                            </button>
                                        </div>
                                    )}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
}
