import React, { useState } from "react";
import { TextField, Button, Stack } from "@mui/material";
import EmployeeServices from "../Services/EmployeeServices";

const employeeService = new EmployeeServices();

const AddEmployee = () => {
  const [formData, setFormData] = useState({
    FullName: "",
    Email: "",
    Phone: "",
    EmailFlag: false,
    FullNameFlag: false,
  });

  const handleValues = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const checkValidity = () => {
    setFormData({
      ...formData,
      FullNameFlag: formData.FullName === "",
      EmailFlag: formData.Email === "",
    });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    checkValidity();
    const { FullNameFlag, EmailFlag, Email, FullName } = formData;
    if (!FullNameFlag && !EmailFlag && Email !== "" && FullName !== "") {
      console.log("Form submitted successfully!");
      let data = {
        FullName: formData.FullName,
        email: formData.Email,
        Phone: formData.Phone,
      };
      employeeService
        .AddEmployee(data)
        .then((data) => {
          setFormData({
            FullName: "",
            Email: "",
            Phone: "",
            EmailFlag: false,
            FullNameFlag: false,
            PasswordFlag: false,
          });
          window.location.href = "/HomePage";
          console.log("data:", data);
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
              name="FullName"
              label="FullName"
              variant="outlined"
              size="small"
              value={formData.FullName}
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
              name="Phone"
              type="text"
              className="TextField"
              label="Phone"
              variant="outlined"
              size="small"
              value={formData.Phone}
              onChange={handleValues}
            />
            <div>
              <Stack direction="row">
                <Button type="submit" variant="outlined">
                  Add
                </Button>
              </Stack>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default AddEmployee;
