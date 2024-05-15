import React, { useState, useEffect } from "react";
import { TextField, Button, Stack } from "@mui/material";
import { useParams } from "react-router-dom";
import EmployeeServices from "../Services/EmployeeServices";

const UpdateEmployee = () => {
  const { id } = useParams();
  const [formData, setFormData] = useState({
    FullName: "",
    Email: "",
    Phone: "",
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);

  useEffect(() => {
    const fetchEmployeeData = async () => {
      const employeeService = new EmployeeServices();
      try {
        const response = await employeeService.GetEmployeeById(id);
        if (response && response.data) {
          console.log("Employee data:", response.data);
          setFormData({
            FullName: response.data.fullName || "",
            Email: response.data.email || "",
            Phone: response.data.phone || "",
          });
        }
      } catch (error) {
        setError("Error fetching employee data");
      }
    };
    fetchEmployeeData();
  }, [id]);

  const handleValues = (e) => {
    const { name, value } = e.target;
    setFormData((prevData) => ({
      ...prevData,
      [name]: value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    const employeeService = new EmployeeServices();
    try {
      await employeeService.UpdateEmployee(id, formData);
      setSuccess(true);
      window.location.href = "/HomePage";
    } catch (error) {
      setError("Error updating employee");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="signup-container">
      <div className="signup-subContainer">
        <div className="Header">Update Employee</div>
        <div className="Body">
          <form onSubmit={handleSubmit}>
            <TextField
              className="TextField"
              name="FullName"
              label="Full Name"
              variant="outlined"
              size="small"
              value={formData.FullName}
              onChange={handleValues}
            />
            <TextField
              className="TextField"
              name="Email"
              label="Email"
              variant="outlined"
              size="small"
              value={formData.Email}
              onChange={handleValues}
            />
            <TextField
              className="TextField"
              name="Phone"
              label="Phone"
              variant="outlined"
              size="small"
              value={formData.Phone}
              onChange={handleValues}
            />
            <div>
              <Stack direction="row">
                <Button type="submit" variant="outlined" disabled={loading}>
                  {loading ? "Updating..." : "Update"}
                </Button>
              </Stack>
            </div>
            {error && <div>Error: {error}</div>}
            {success && <div>Employee updated successfully!</div>}
          </form>
        </div>
      </div>
    </div>
  );
};

export default UpdateEmployee;
