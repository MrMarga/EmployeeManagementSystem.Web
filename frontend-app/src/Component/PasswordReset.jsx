import React, { useState } from "react";
import { useParams } from "react-router-dom";
import { TextField, Button, Typography } from "@mui/material";
import AuthServices from "../Services/AuthServices";

const authService = new AuthServices();

const PasswordReset = () => {
  const { email, token } = useParams(); // Extract email from URL params
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [message, setMessage] = useState("");

  const handleResetPassword = async (e) => {
    e.preventDefault();
    if (password !== confirmPassword) {
      setMessage("Passwords do not match");
      return;
    }
    try {
      const response = await authService.ResetPassword({
        token,
        email,
        newPassword: password,
      });
      window.location.href = "/";
      setMessage(response.data.message);
    } catch (error) {
      console.error("Error resetting password:", error);
      setMessage("Failed to reset password");
    }
  };

  return (
    <div>
      <Typography variant="h4">Reset Password</Typography>
      <form onSubmit={handleResetPassword}>
        <TextField
          label="New Password"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        <TextField
          label="Confirm Password"
          type="password"
          value={confirmPassword}
          onChange={(e) => setConfirmPassword(e.target.value)}
          required
        />
        <Button type="submit" variant="contained" color="primary">
          Reset Password
        </Button>
      </form>
      {message && <Typography>{message}</Typography>}
    </div>
  );
};

export default PasswordReset;
