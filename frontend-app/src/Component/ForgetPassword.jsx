import React, { useState } from "react";
import { TextField, Button, Stack, Typography } from "@mui/material";
import AuthServices from "../Services/AuthServices";
import { useNavigate } from "react-router-dom";
import "./ForgotPassword.scss";

const authService = new AuthServices();

const ForgotPassword = () => {
  const [email, setEmail] = useState("");
  const [emailFlag, setEmailFlag] = useState(false);
  const [message, setMessage] = useState("");
  const navigate = useNavigate();

  const handleEmailChange = (e) => {
    setEmail(e.target.value);
  };

  const handleResetPassword = async (e) => {
    e.preventDefault();
    setEmailFlag(email === "");
    if (email !== "") {
      try {
        const response = await authService.ForgotPassword({ email });
        if (response.data.resetLink) {
          console.log("Password reset email sent successfully!");
          console.log(response.data);
          let email = response.data.email;
          let token = response.data.resetToken;
          console.log(
            "Navigating to:",
            `/reset-password/${email}/${encodeURIComponent(token)}`
          );
          setMessage(response.data.message);
          navigate(`/reset-password/${email}/${encodeURIComponent(token)}`);
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
          <Typography variant="body1">{message}</Typography>
        </div>
      </div>
    </div>
  );
};

export default ForgotPassword;
