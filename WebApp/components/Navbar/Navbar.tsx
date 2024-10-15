'use client'
import React, { useContext, useState } from 'react'
import { authContext } from '@/components/auth/AuthProvider'
import { useRouter } from 'next/navigation'
import { API_ROUTES } from '@/route_config'
import LoginButton from './LoginButton'
import { useAuth } from '../hooks/useAuth'

const Navbar = () => {
    const { session } = useAuth();
    const [isOpen, setisOpen] = useState(false);

    const toggleNavbar = () => {
        setisOpen(!isOpen);
    }


    return (
        <nav className='p-2 flex justify-between items-center'>
            <div className='font-light text-3xl'>Github Summary</div>
            <div className='flex items-center gap-2'>
                <LoginButton loggedIn={session?.isAuthenticated || null} />
                {session?.isAuthenticated && <div className='p-3 font-light text-slate-200 bg-slate-800 rounded-md cursor-pointer hover:bg-slate-900 transition ease-in duration-150'>{session.user?.name}</div>}
            </div>
        </nav>
            
    )
}

export default Navbar