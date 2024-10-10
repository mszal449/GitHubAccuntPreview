import { IRepository } from '@/types'
import React from 'react'
import { Card, CardContent, CardDescription, CardHeader, CardTitle, CardFooter } from './ui/card'

const RepoCard = ({id, name, owner, link} : IRepository) => {
    return (
        <Card className='cursor-pointer hover:bg-[#3b3b3b] transition duration-100 ease-in'>
            <CardHeader>
                <CardTitle>{name}</CardTitle>
                <CardDescription>{owner}</CardDescription>
            </CardHeader>
            <CardContent>
                <p>Card Content</p>
            </CardContent>
            <CardFooter>
                <p>Card Footer</p>
            </CardFooter>
        </Card>
  )
}

export default RepoCard