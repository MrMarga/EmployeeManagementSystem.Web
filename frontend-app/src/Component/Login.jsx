import React, { useState } from "react";
import "./Signup.scss";
import TextField from "@mui/material/TextField";
import Radio from "@mui/material/Radio";
import RadioGroup from "@mui/material/RadioGroup";
import FormControlLabel from "@mui/material/FormControlLabel";
import Button from "@mui/material/Button";
import Stack from "@mui/material/Stack";
import { useNavigate } from "react-router-dom";
import AuthServices from "../Services/AuthServices";
import { jwtDecode } from "jwt-decode";

const authService = new AuthServices();

const Login = () => {
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    Email: "",
    Password: "",
    RoleValue: "User",
    EmailFlag: false,
    PasswordFlag: false,
  });

  const handleValues = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleChangeRole = (e) => {
    setFormData({ ...formData, RoleValue: e.target.value });
  };

  const checkValidity = () => {
    const emailFlag = formData.Email === "";
    const passwordFlag = formData.Password === "";
    setFormData((prevFormData) => ({
      ...prevFormData,
      EmailFlag: emailFlag,
      PasswordFlag: passwordFlag,
    }));
    return !emailFlag && !passwordFlag;
  };

  const AccountAlready = (e) => {
    navigate("/");
  };

  const handleForgotPassword = () => {
    navigate("/forgot-password");
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const isValid = checkValidity();
    const { Email, Password } = formData;
    if (isValid && Email !== "" && Password !== "") {
      try {
        const data = {
          email: formData.Email,
          password: formData.Password,
          Role: formData.RoleValue,
        };
        const response = await authService.Login(data);

        if (response.data.isSuccess) {
          console.log(response.data.tokens);

          const getTokenExpiration = (accessToken) => {
            try {
              const decoded = jwtDecode(accessToken);
              console.log(decoded);
              if (decoded && decoded.exp) {
                return decoded.exp;
              }
              return null;
            } catch (error) {
              console.error("Invalid token", error);
              return null;
            }
          };

          const accessToken = response.data.tokens.accessToken;
          const refreshToken = response.data.tokens.refreshToken;
          const deviceId = response.data.tokens.deviceID;
          const userInfo = jwtDecode(accessToken);
          const expTime = getTokenExpiration(accessToken);
          const decodeExpirationTime = (expTime) => {
            const date = new Date(expTime * 1000);
            return date.toLocaleString();
          };
          const expirationTime = decodeExpirationTime(expTime);
          console.log("Expiration Date:", expirationTime);

          localStorage.setItem("AccessToken", accessToken);
          localStorage.setItem("deviceId", deviceId);
          localStorage.setItem("RefreshToken", refreshToken);
          localStorage.setItem("Exp Time", expirationTime);
          localStorage.setItem("UserInfo", JSON.stringify(userInfo));
          localStorage.setItem("UserRole", userInfo.role);
          localStorage.setItem("UserId", userInfo.nameid);

          navigate("/homePage");
          console.log("Logged In successfully!");
        } else {
          console.log("response", response.data.message);
        }
      } catch (error) {
        console.error("Error during login:", error);
      }
    } else {
      console.log("Form submission failed. Please check your inputs.");
    }
  };

  return (
    <div className="signup-container">
      <div className="signup-subContainer">
        <div className="Header">Login</div>
        <div className="Body">
          <form onSubmit={handleSubmit}>
            <TextField
              error={formData.EmailFlag}
              name="Email"
              className="TextField"
              label="Email"
              variant="outlined"
              size="small"
              value={formData.Email}
              onChange={handleValues}
            />
            <TextField
              error={formData.PasswordFlag}
              name="Password"
              type="password"
              className="TextField"
              label="Password"
              variant="outlined"
              size="small"
              value={formData.Password}
              onChange={handleValues}
            />

            <RadioGroup
              row
              aria-labelledby="demo-row-radio-buttons-group-label"
              name="Role"
              className="Roles"
              value={formData.RoleValue}
              onChange={handleChangeRole}
            >
              <FormControlLabel
                className="RoleValue"
                value="Admin"
                control={<Radio />}
                label="Admin"
              />
              <FormControlLabel
                className="RoleValue"
                value="User"
                control={<Radio />}
                label="User"
              />
            </RadioGroup>
            <div>
              <Stack spacing={2} direction="row">
                <Button variant="text" onClick={AccountAlready}>
                  Create a New account
                </Button>
                <Button variant="text" onClick={handleForgotPassword}>
                  Forgot Password?
                </Button>
                <Button type="submit" variant="outlined">
                  Login
                </Button>
              </Stack>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default Login;
