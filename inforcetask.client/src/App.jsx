import React, { useState, useEffect } from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import Layout from './Layout';
import LoginPage from './LoginPage';
import ShortUrlTable from './ShortUrlTable';
import ShortUrlDetails from './ShortUrlDetails';

function AppContent() {
    const [userEmail, setUserEmail] = useState(null);

    useEffect(() => {
        fetch(`${import.meta.env.VITE_API_BASE_URL}/api/Accounts/current`, { credentials: 'include' })
            .then(res => res.ok ? res.text() : null)
            .then(email => setUserEmail(email))
            .catch(() => setUserEmail(null));
    }, []);

    const handleLogout = async () => {
        await fetch(`${import.meta.env.VITE_API_BASE_URL}/api/sessions/current`, {
            method: 'DELETE',
            credentials: 'include'
        });
        setUserEmail(null);
        window.location.reload();
    };

    return (
        <Layout userEmail={userEmail} onLogout={handleLogout}>
            <Routes>
                <Route path="/login" element={userEmail ? <Navigate to="/shorturl" /> : <LoginPage onLogin={setUserEmail} />} />
                <Route path="/shorturl" element={<ShortUrlTable userEmail={userEmail} />} />
                <Route path="/shorturl/:id" element={<ShortUrlDetails />} />
                <Route path="*" element={<Navigate to="/shorturl" />} />
            </Routes>
        </Layout>
    );
}

function App() {
    return (
        <AppContent />
    );
}

export default App;