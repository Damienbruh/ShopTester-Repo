import { createContext, useContext, useState, useEffect, useMemo } from 'react';

const UserContext = createContext();

export function UserProvider({ children }) {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const verifySession = async () => {
            try {
                const response = await fetch('/api/login', {
                    credentials: 'include'
                });
                if (response.ok) {
                    const userData = await response.json();
                    if (userData.id) {
                        setUser(userData);
                        //console.log('Session verified:', userData);
                    }
                }
                //if (!response.ok) {
                //    console.log('No active session found');
                //}
            } catch (error) {
                console.error('Session verification failed:', error);
            } finally {
                setLoading(false);
            }
        };

        verifySession();
    }, []);

    const login = (userData) => {
        setUser(userData);
        console.log(userData);
    };

    const logout = () => {
        setUser(null);
    };

    const value = useMemo(() => ({ user, login, logout }), [user]);

    if (loading) {
        return <div>Loading...</div>;
    }

    return (
        <UserContext.Provider value={value}>
            {children}
        </UserContext.Provider>
    );
}

export function useUser() {
    return useContext(UserContext);
}
