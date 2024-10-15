'use client';

import { API_ROUTES } from '@/route_config';
import React, { createContext, useEffect, useState } from 'react';

export const authContext = createContext<IAuthContext | null>(null);

interface IAuthContext {
  isAuthenticated: boolean | null;
  user: userData | null;
  loading: boolean;
}

interface userData {
  id?: number;
  name?: string;
  email?: string;
}

const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean | null>(null)
  const [user, setUser] = useState<userData | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const checkStatus = async () => {
      try {
        const response = await fetch(API_ROUTES.status ,{
          method: 'GET',
          credentials: 'include',
        });

        if (!response.ok) {
          setIsAuthenticated(false);
          setUser(null);
          return;
        }

        const data = await response.json();
        if(data.result === 'Authenticated') {
            setIsAuthenticated(true);
            setUser({
                id: data.user.id,
                name: data.user.username,
                email: data.user.email,
            });
        } else {
            setIsAuthenticated(false);
            setUser(null);
        }        
      } catch (error) {
        setIsAuthenticated(false);
        setUser(null);
      } finally {
        setLoading(false);
      }
    };

    checkStatus();
  }, []);

  return (
    <authContext.Provider value={{ isAuthenticated, user, loading }}>
      {children}
    </authContext.Provider>
  );
};

export default AuthProvider;
