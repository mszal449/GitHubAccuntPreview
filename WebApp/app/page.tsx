'use client';

import { useRouter } from "next/navigation";
import { useContext, useEffect, useState } from "react";
import { authContext } from "@/components/auth/AuthProvider";
import { useAuth } from "@/components";

export default function Home() {
  const { session, loading , error, setLoading, setError } = useAuth();
  const router = useRouter();

  if (loading) {
    // Show a loading state while the context is being populated
    return <div>Loading...</div>;
  }

  return (
    <div className="flex flex-col gap-4 items-center">
      {/* {session?.isAuthenticated && <div className="text-red-500">Authenticated as {session?.user?.name} | {session?.user?.id} | {session?.user?.email}</div>} */}
      <div className="text-2xl">User {session?.user?.name}</div>
      <div></div>
    </div>
  );
}
