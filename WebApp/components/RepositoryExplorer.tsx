
'use client'
import { GetAllRepositories } from '@/services/RepositoryService';
import { IRepository } from '@/types'
import Link from 'next/link';
import React, { useEffect, useState } from 'react'
import RepoCard from './RepoCard';

const RepoExplorer = () => {
  const [repositories, setRepositories] = useState<IRepository[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const data = await GetAllRepositories();
        setRepositories(data);
        setLoading(false);
      } catch {
        console.error('Failed to fetch repositories');
      }
    }
    fetchData();

  }, [])

  return (
    <div className='max-w-2/3 pt-5 flex justify-center'>
        {loading ? (<p>Loading...</p>) : (
          <div className='grid grid-col-1 md:grid-cols-2 gap-3'>
            {repositories.map((repository : IRepository) => (
              <Link href={repository.link} key={repository.id}>
                <RepoCard 
                  id={repository.id}
                  name={repository.name}
                  owner={repository.owner}
                  link={repository.link}
                  />
              </Link>
            ))}
          </div>
        )}
    </div>
  )
}

export default RepoExplorer