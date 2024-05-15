import React, { useState } from "react";
import "./Signup.scss";
import TextField from "@mui/material/TextField";
import Radio from "@mui/material/Radio";
import RadioGroup from "@mui/material/RadioGroup";
import FormControlLabel from "@mui/material/FormControlLabel";
import Button from "@mui/material/Button";
import Stack from "@mui/material/Stack";
import AuthServices from "../Services/AuthServices";

const authService = new AuthServices();

const Login = () => {
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
    setFormData({
      ...formData,
      EmailFlag: formData.Email === "",
      PasswordFlag: formData.Password === "",
    });
  };

  const AccountAlready = (e) => {
    window.location.href = "/";
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    checkValidity();
    const { EmailFlag, PasswordFlag, Email, Password } = formData;
    if (!EmailFlag && !PasswordFlag && Email !== "" && Password !== "") {
      console.log("Form submitted successfully!");
      try {
        let data = {
          email: formData.Email,
          password: formData.Password,
          Role: formData.RoleValue,
        };
        const response = await authService.Login(data);
        if (response.data.isSuccess) {
          console.log("Login successful!", response);
          window.location.href = "/HomePage";
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
