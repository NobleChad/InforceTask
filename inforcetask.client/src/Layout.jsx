import React from 'react';
import { Link } from 'react-router-dom';
import './Layout.css';

export default function Layout({ userEmail, onLogout, children }) {
    return (
        <div>
            <nav className="layout-nav">
                <div>
                    <Link to="/shorturl">Home</Link>
                </div>
                <div>
                    {!userEmail ? (
                        <Link to="/login">Login / Register</Link>
                    ) : (
                        <>
                            <span>Hello, {userEmail}</span>
                            <button onClick={onLogout}>Logout</button>
                        </>
                    )}
                </div>
            </nav>
            <div className="layout-content">{children}</div>
        </div>
    );
}
