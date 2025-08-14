import React, { useState } from 'react';
import './LoginPage.css';

export default function LoginPage({ onLogin }) {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [role, setRole] = useState('User');
    const [message, setMessage] = useState('');

    const register = async () => {
        const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/api/Accounts`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password, role }),
        });

        const text = await response.text();
        setMessage(text);
    };

    const login = async () => {
        const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/api/sessions`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password }),
            credentials: 'include'
        });

        const data = await response.json().catch(() => ({}));
        setMessage(data.message || 'Login failed');

        if (response.ok) onLogin(email);
    };

    return (
        <div className="login-page-container">
            <h2>Register / Login</h2>
            <input
                type="email"
                placeholder="Email"
                value={email}
                onChange={e => setEmail(e.target.value)}
            />
            <input
                type="password"
                placeholder="Password"
                value={password}
                onChange={e => setPassword(e.target.value)}
            />
            <select value={role} onChange={e => setRole(e.target.value)}>
                <option value="User">User</option>
                <option value="Admin">Admin</option>
            </select>
            <div>
                <button onClick={register}>Register</button>
                <button onClick={login}>Login</button>
            </div>
            <p>{message}</p>
        </div>
    );
}
