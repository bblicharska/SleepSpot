import React, {
  createContext,
  useContext,
  useState,
  useEffect,
  ReactNode,
} from "react";
import { jwtDecode } from "jwt-decode";
import { useNavigate } from "react-router-dom";
import { API_BASE_URL } from "../types/types";

interface User {
  userId: string;
  firstName: string;
  lastName: string;
  role: string;
  email: string;
}

interface AuthContextType {
  user: User | null;
  token: string | null;
  loading: boolean;
  login: (token: string) => void;
  logout: () => void;
  loggedOutDueToExpiry: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const navigate = useNavigate();
  const [token, setToken] = useState<string | null>(
    localStorage.getItem("authToken")
  );
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const [loggedOutDueToExpiry, setLoggedOutDueToExpiry] = useState(false);

  const fetchUserData = async (userId: string, token: string) => {
    try {
      setLoading(true);
      const response = await fetch(`${API_BASE_URL}/api/users/${userId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!response.ok) throw new Error("Failed to fetch user data");
      const data = await response.json();
      console.log(response);

      setUser({
        userId,
        firstName: data.firstName,
        lastName: data.lastName,
        role: data.role,
        email: data.email,
      });
    } catch (error) {
      console.error("Error fetching user data:", error);
      setUser(null);
      setToken(null);
      localStorage.removeItem("authToken");
    } finally {
      setLoading(false);
    }
  };
  useEffect(() => {
    if (token) {
      try {
        const decoded: any = jwtDecode(token);
        const userId = decoded.sub;
        const exp = decoded.exp;

        if (!userId || !exp) throw new Error("Invalid token");

        const isExpired = exp * 1000 < Date.now();
        if (isExpired) {
          console.warn("Token expired");
          setToken(null);
          setUser(null);
          localStorage.removeItem("authToken");
          setLoggedOutDueToExpiry(true);
          setLoading(false);
          navigate("/login");
        } else {
          localStorage.setItem("authToken", token);
          fetchUserData(userId, token);
        }
      } catch (err) {
        console.error("Invalid token:", err);
        setUser(null);
        setToken(null);
        localStorage.removeItem("authToken");
        setLoading(false);
      }
    } else {
      setUser(null);
      localStorage.removeItem("authToken");
      setLoading(false);
    }
  }, [token]);

  const login = (newToken: string) => {
    setLoggedOutDueToExpiry(false);
    setToken(newToken);
    navigate("/main");
  };

  const logout = () => {
    setToken(null);
    setUser(null);
    localStorage.removeItem("authToken");
    setLoading(false);
    navigate("/login");
  };

  return (
    <AuthContext.Provider
      value={{ user, token, loading, login, logout, loggedOutDueToExpiry }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
};
