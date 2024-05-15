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

const SignUp = () => {
  const [formData, setFormData] = useState({
    Name: "",
    Username: "",
    Password: "",
    ConfirmPassword: "",
    Email: "",
    RoleValue: "User",
    NameFlag: false,
    EmailFlag: false,
    UsernameFlag: false,
    PasswordFlag: false,
    ConfirmPasswordFlag: false,
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

      UsernameFlag: formData.Username === "",
      EmailFlag: formData.Email === "",
      PasswordFlag: formData.Password === "",
      ConfirmPasswordFlag:
        formData.ConfirmPassword === "" ||
        formData.ConfirmPassword !== formData.Password,
    });
  };

  const handleSignInClick = (e) => {
    window.location.href = "/Login";
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    checkValidity();
    const {
      UsernameFlag,
      EmailFlag,
      PasswordFlag,
      ConfirmPasswordFlag,
      Email,
      Password,
      ConfirmPassword,
    } = formData;
    if (
      !UsernameFlag &&
      !EmailFlag &&
      !PasswordFlag &&
      !ConfirmPasswordFlag &&
      Email !== "" &&
      Password !== "" &&
      ConfirmPassword !== "" &&
      Password === ConfirmPassword
    ) {
      console.log("Form submitted successfully!");
      let data = {
        username: formData.Username,
        name: formData.Name,
        email: formData.Email,
        password: formData.Password,
        confirmPassword: formData.ConfirmPassword,
        Role: formData.RoleValue,
      };
      authService
        .SignUp(data)
        .then((data) => {
          setFormData({
            Name: "",
            Username: "",
            Password: "",
            ConfirmPassword: "",
            Email: "",
            RoleValue: "User",
            EmailFlag: false,
            UsernameFlag: false,
            PasswordFlag: false,
            ConfirmPasswordFlag: false,
          });
          console.log("data:", data);
        })
        .catch((error) => {
          console.log("Error:", error);
        });
      window.location.href = "/Login";
    } else {
      console.log("Form submission failed. Please check your inputs.");
    }
  };

  return (
    <div className="signup-container">
      <div className="signup-subContainer">
        <div className="Header">Sign Up</div>
        <div className="Body">
          <form onSubmit={handleSubmit}>
            <TextField
              className="TextField"
              name="Name"
              label="Name"
              variant="outlined"
              size="small"
              onChange={handleValues}
            />
            <TextField
              error={formData.UsernameFlag}
              name="Username"
              className="TextField"
              label="Username"
              variant="outlined"
              size="small"
              value={formData.Username}
              onChange={handleValues}
            />
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
            <TextField
              error={formData.ConfirmPasswordFlag}
              name="ConfirmPassword"
              type="password"
              className="TextField"
              label="Confirm Password"
              variant="outlined"
              size="small"
              value={formData.ConfirmPassword}
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
              <Stack spacing={10} direction="row">
                <Button onClick={handleSignInClick}>
                  Already has an account Sign In
                </Button>
                <Button type="submit" variant="outlined">
                  Sign Up
                </Button>
              </Stack>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default SignUp;
