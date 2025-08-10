import React, { useState } from "react";
import {
  Box,
  Card,
  CardContent,
  Typography,
  TextField,
  Button,
  Alert,
  CircularProgress,
  InputAdornment,
  IconButton,
} from "@mui/material";
import { Visibility, VisibilityOff } from "@mui/icons-material";
import { toast } from "react-toastify";
import { useAuth } from "./AuthContext";

type Mode = "signin" | "signup";

export const AuthForm: React.FC = () => {
  const [mode, setMode] = useState<Mode>("signin");
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    confirmPassword: "",
  });
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();

  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const toggleMode = () => {
    setError(null);
    setFormData({
      firstName: "",
      lastName: "",
      email: "",
      password: "",
      confirmPassword: "",
    });
    setMode((prev) => (prev === "signin" ? "signup" : "signin"));
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData((prev) => ({
      ...prev,
      [e.target.name]: e.target.value,
    }));
  };

  const validateForm = () => {
    const isEmailValid = /\S+@\S+\.\S+/.test(formData.email);
    const doPasswordsMatch = formData.password === formData.confirmPassword;

    if (mode === "signin") {
      return isEmailValid && formData.password.trim();
    }

    return (
      formData.firstName.trim() &&
      formData.lastName.trim() &&
      isEmailValid &&
      formData.password.trim() &&
      formData.confirmPassword.trim() &&
      doPasswordsMatch
    );
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validateForm()) return;

    setLoading(true);
    setError(null);

    const endpoint =
      mode === "signin" ? "/api/auth/login" : "/api/auth/register";

    const payload =
      mode === "signin"
        ? {
            email: formData.email,
            password: formData.password,
          }
        : {
            firstName: formData.firstName,
            lastName: formData.lastName,
            email: formData.email,
            password: formData.password,
            confirmPassword: formData.confirmPassword,
          };

    try {
      const response = await fetch(endpoint, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        const text = await response.text();
        let errorMsg = "Authentication failed";
        try {
          const data = JSON.parse(text);
          errorMsg = data.message || errorMsg;
        } catch {
          errorMsg = text || errorMsg;
        }
        throw new Error(errorMsg);
      }

      const data = await response.json();
      toast.success(
        mode === "signin"
          ? "Successfully signed in!"
          : "Account created successfully!"
      );
      console.log("Received token:", data.token);
      login(data.accessToken);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box
      display="flex"
      justifyContent="center"
      alignItems="center"
      height="100vh"
    >
      <Card sx={{ width: 400, padding: 3, borderRadius: 2, boxShadow: 4 }}>
        <CardContent>
          <Typography variant="h5" gutterBottom fontWeight={600}>
            {mode === "signin" ? "Sign In" : "Sign Up"}
          </Typography>
          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}
          <form onSubmit={handleSubmit}>
            {mode === "signup" && (
              <>
                <TextField
                  fullWidth
                  label="First Name"
                  name="firstName"
                  value={formData.firstName}
                  onChange={handleChange}
                  margin="normal"
                  required
                />
                <TextField
                  fullWidth
                  label="Last Name"
                  name="lastName"
                  value={formData.lastName}
                  onChange={handleChange}
                  margin="normal"
                  required
                />
              </>
            )}
            <TextField
              fullWidth
              label="Email"
              name="email"
              type="email"
              value={formData.email}
              onChange={handleChange}
              margin="normal"
              required
              error={
                formData.email.length > 0 &&
                !/\S+@\S+\.\S+/.test(formData.email)
              }
              helperText={
                formData.email.length > 0 &&
                !/\S+@\S+\.\S+/.test(formData.email)
                  ? "Invalid email format"
                  : ""
              }
            />
            <TextField
              fullWidth
              label="Password"
              name="password"
              type={showPassword ? "text" : "password"}
              value={formData.password}
              onChange={handleChange}
              margin="normal"
              required
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton
                      onClick={() => setShowPassword(!showPassword)}
                      edge="end"
                      aria-label="toggle password visibility"
                    >
                      {showPassword ? <VisibilityOff /> : <Visibility />}
                    </IconButton>
                  </InputAdornment>
                ),
              }}
            />
            {mode === "signup" && (
              <TextField
                fullWidth
                label="Confirm Password"
                name="confirmPassword"
                type={showConfirmPassword ? "text" : "password"}
                value={formData.confirmPassword}
                onChange={handleChange}
                margin="normal"
                required
                error={
                  formData.confirmPassword.length > 0 &&
                  formData.password !== formData.confirmPassword
                }
                helperText={
                  formData.confirmPassword.length > 0 &&
                  formData.password !== formData.confirmPassword
                    ? "Passwords do not match"
                    : ""
                }
                InputProps={{
                  endAdornment: (
                    <InputAdornment position="end">
                      <IconButton
                        onClick={() =>
                          setShowConfirmPassword(!showConfirmPassword)
                        }
                        edge="end"
                        aria-label="toggle confirm password visibility"
                      >
                        {showConfirmPassword ? (
                          <VisibilityOff />
                        ) : (
                          <Visibility />
                        )}
                      </IconButton>
                    </InputAdornment>
                  ),
                }}
              />
            )}
            <Button
              type="submit"
              variant="contained"
              fullWidth
              disabled={!validateForm() || loading}
              sx={{
                mt: 2,
                borderRadius: 2,
                background: "linear-gradient(45deg, #8E44AD, #AF7AC5)",
                boxShadow: "0 3px 5px 2px rgba(142, 68, 173, .3)",
                textTransform: "none",
                fontWeight: 600,
                position: "relative",
                opacity: !validateForm() || loading ? 0.5 : 1,
                pointerEvents: !validateForm() || loading ? "none" : "auto",
              }}
            >
              {loading ? (
                <CircularProgress size={24} color="inherit" />
              ) : mode === "signin" ? (
                "Sign In"
              ) : (
                "Sign Up"
              )}
            </Button>
            <Button
              onClick={toggleMode}
              fullWidth
              sx={{ mt: 1, textTransform: "none" }}
              disabled={loading}
            >
              {mode === "signin"
                ? "Don't have an account? Sign Up"
                : "Already have an account? Sign In"}
            </Button>
          </form>
        </CardContent>
      </Card>
    </Box>
  );
};
