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
import { useAuth } from "./AuthContext";
import { toast } from "react-toastify";
import { changePassword } from "../queries/changePassword";

export const ChangePasswordPage: React.FC = () => {
  const { user, token } = useAuth();
  const [formData, setFormData] = useState({
    oldPassword: "",
    newPassword: "",
    confirmNewPassword: "",
  });
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [showPasswords, setShowPasswords] = useState({
    old: false,
    new: false,
    confirm: false,
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData((prev) => ({
      ...prev,
      [e.target.name]: e.target.value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (formData.newPassword !== formData.confirmNewPassword) {
      setError("New passwords do not match");
      return;
    }

    setLoading(true);

    try {
      await changePassword(
        token,
        formData.oldPassword,
        formData.newPassword,
        user?.userId
      );
      toast.success("Password changed successfully!");
      setFormData({ oldPassword: "", newPassword: "", confirmNewPassword: "" });
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
          <Typography variant="h5" fontWeight={600} gutterBottom>
            Change Password
          </Typography>
          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}
          <form onSubmit={handleSubmit}>
            <TextField
              fullWidth
              label="Current Password"
              name="oldPassword"
              type={showPasswords.old ? "text" : "password"}
              value={formData.oldPassword}
              onChange={handleChange}
              margin="normal"
              required
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton
                      onClick={() =>
                        setShowPasswords((prev) => ({
                          ...prev,
                          old: !prev.old,
                        }))
                      }
                    >
                      {showPasswords.old ? <VisibilityOff /> : <Visibility />}
                    </IconButton>
                  </InputAdornment>
                ),
              }}
            />
            <TextField
              fullWidth
              label="New Password"
              name="newPassword"
              type={showPasswords.new ? "text" : "password"}
              value={formData.newPassword}
              onChange={handleChange}
              margin="normal"
              required
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton
                      onClick={() =>
                        setShowPasswords((prev) => ({
                          ...prev,
                          new: !prev.new,
                        }))
                      }
                    >
                      {showPasswords.new ? <VisibilityOff /> : <Visibility />}
                    </IconButton>
                  </InputAdornment>
                ),
              }}
            />
            <TextField
              fullWidth
              label="Confirm New Password"
              name="confirmNewPassword"
              type={showPasswords.confirm ? "text" : "password"}
              value={formData.confirmNewPassword}
              onChange={handleChange}
              margin="normal"
              required
              error={
                formData.confirmNewPassword.length > 0 &&
                formData.newPassword !== formData.confirmNewPassword
              }
              helperText={
                formData.confirmNewPassword.length > 0 &&
                formData.newPassword !== formData.confirmNewPassword
                  ? "Passwords do not match"
                  : ""
              }
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton
                      onClick={() =>
                        setShowPasswords((prev) => ({
                          ...prev,
                          confirm: !prev.confirm,
                        }))
                      }
                    >
                      {showPasswords.confirm ? (
                        <VisibilityOff />
                      ) : (
                        <Visibility />
                      )}
                    </IconButton>
                  </InputAdornment>
                ),
              }}
            />
            <Button
              type="submit"
              variant="contained"
              fullWidth
              disabled={loading}
              sx={{
                mt: 2,
                borderRadius: 2,
                background: "linear-gradient(45deg, #8E44AD, #AF7AC5)",
                boxShadow: "0 3px 5px 2px rgba(142, 68, 173, .3)",
                textTransform: "none",
                fontWeight: 600,
              }}
            >
              {loading ? (
                <CircularProgress size={24} color="inherit" />
              ) : (
                "Update Password"
              )}
            </Button>
          </form>
        </CardContent>
      </Card>
    </Box>
  );
};
