import React, { useState } from "react";
import "./Signup.scss";
import TextField from "@mui/material/TextField";
import Radio from "@mui/material/Radio";
import RadioGroup from "@mui/material/RadioGroup";
import FormControlLabel from "@mui/material/FormControlLabel";
import Button from "@mui/material/Button";
import Stack from "@mui/material/Stack";
import AuthServices from "../Services/AuthServices";
import Input from "@mui/material/Input";
import { FormLabel } from "@mui/material";

const authService = new AuthServices();

const SignUp = () => {
  const [formData, setFormData] = useState({
    Username: "",
    Name: "",
    Password: "",
    ConfirmPassword: "",
    Email: "",
    RoleValue: "User",
    ProfilePicture: null,
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

  const handleFileChange = (e) => {
    setFormData({ ...formData, ProfilePicture: e.target.files[0] });
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

  const handleSignInClick = () => {
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
      ProfilePicture,
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

      let data = new FormData();
      data.append("username", formData.Username);
      data.append("name", formData.Name);
      data.append("email", formData.Email);
      data.append("password", formData.Password);
      data.append("confirmPassword", formData.ConfirmPassword);
      data.append("role", formData.RoleValue);
      data.append("ImageFile", formData.ProfilePicture);

      authService
        .SignUp(data)
        .then((response) => {
          setFormData({
            Name: "",
            Username: "",
            Password: "",
            ConfirmPassword: "",
            Email: "",
            RoleValue: "User",
            ProfilePicture: "",
            EmailFlag: false,
            UsernameFlag: false,
            PasswordFlag: false,
            ConfirmPasswordFlag: false,
          });
          console.log("data:", response.data);
          // Redirect to login after successful signup
          window.location.href = "/Login";
        })
        .catch((error) => {
          console.log("Error:", error);
        });
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

            <FormLabel>Profile Picture : </FormLabel>
            <Input
              type="file"
              id="profile-picture"
              onChange={handleFileChange}
            />
            {/* Profile picture preview */}
            {formData.ProfilePicture && (
              <img
                src={URL.createObjectURL(formData.ProfilePicture)}
                alt="Profile Preview"
                style={{ maxWidth: "100px", marginTop: "10px" }}
              />
            )}

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
                  Already has an account? Sign In
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
