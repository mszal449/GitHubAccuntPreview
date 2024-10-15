'use client';
import { useRouter } from 'next/navigation';
import React, { useContext } from 'react'
import { authContext } from '../auth/AuthProvider';
import { API_ROUTES } from '@/route_config';


const LoginButton = ({loggedIn} : {loggedIn : boolean | null}) => {
    const router = useRouter();

    const handleLogin = () => {
        router.push(API_ROUTES.login);
    }

    const handleLogout = () => {
        router.push(API_ROUTES.logout);
      }
    
    return (
        <div>
            {loggedIn ? (
                <button 
                className='login-button' 
                onClick={handleLogout}>
                Sign Out
                </button>
            ) : (
                <button
                className='login-button' 
                onClick={handleLogin}>
                Sign In
                </button>
            )}
        </div>
    );
        
}


export default LoginButton