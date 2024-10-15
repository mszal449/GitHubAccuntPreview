'use client';
import { useContext, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { authContext } from "@/components/auth/AuthProvider";

interface UseAuthProps {
  redirectOnUnauthorized?: boolean;
}


export const useAuth = ({ redirectOnUnauthorized = true } : UseAuthProps = {}) => {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(false);
  const session = useContext(authContext);
  const router = useRouter();

  useEffect(() => {
    if (session?.isAuthenticated == null) {
      return;
    }

    if (session.isAuthenticated === false) {
      setError(true);
      setLoading(false);
      if (redirectOnUnauthorized) {
        router.push("/unauthenticated");
      }
      return;
    } else {
      setError(false);
      setLoading(false);
    }
  }, [session]);

  return { session, loading, error, setLoading, setError };
};
