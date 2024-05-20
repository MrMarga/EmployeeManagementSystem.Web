import React, { useState } from "react";
import { TextField, Button, Stack, Typography } from "@mui/material";
import AuthServices from "../Services/AuthServices";
import { useNavigate } from "react-router-dom";

const authService = new AuthServices();

const ForgotPassword = () => {
  const [email, setEmail] = useState("");
  const [emailFlag, setEmailFlag] = useState(false);
  const [resetToken, setResetToken] = useState("");
  const [message, setMessage] = useState("");

  const handleEmailChange = (e) => {
    setEmail(e.target.value);
  };

  const handleResetPassword = async (e) => {
    e.preventDefault();
    setEmailFlag(email === "");
    if (email !== "") {
      try {
        const response = await authService.ForgotPassword({ email });
        if (response.data.resetToken) {
          console.log("Password reset email sent successfully!");
          setResetToken(response.data.resetToken);
          setMessage(response.data.message);
        } else {
          console.log("Error:", response.data.message);
        }
      } catch (error) {
        console.error("Error during password reset:", error);
      }
    }
  };

  return (
    <div className="forgot-password-container">
      <div className="forgot-password-subContainer">
        <div className="Header">Forgot Password</div>
        <div className="Body">
          <form onSubmit={handleResetPassword}>
            <TextField
              error={emailFlag}
              name="Email"
              className="TextField"
              label="Email"
              variant="outlined"
              size="small"
              value={email}
              onChange={handleEmailChange}
            />
            <Stack spacing={2} direction="row">
              <Button type="submit" variant="outlined">
                Reset Password
              </Button>
            </Stack>
          </form>
          {resetToken && (
            <div>
              <Typography variant="body1">Reset Token: {resetToken}</Typography>
              <Typography variant="body1">{message}</Typography>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default ForgotPassword;
