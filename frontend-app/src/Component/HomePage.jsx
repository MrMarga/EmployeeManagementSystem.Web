import React, { useState, useEffect } from "react";
import "./HomePage.scss";
import {
  Container,
  CircularProgress,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Button,
} from "@mui/material";
import { NavLink } from "react-router-dom";
import EmployeeServices from "../Services/EmployeeServices";
import Logout from "./Logout";

const HomePage = () => {
  const [employees, setEmployees] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      const employeeService = new EmployeeServices();
      try {
        const response = await employeeService.GetAllEmployee();
        if (response && response.data) {
          setEmployees(response.data);
        } else {
          console.error("Unexpected response structure:", response);
        }
      } catch (error) {
        console.error("Error fetching employees:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const handleDelete = async (id) => {
    if (!window.confirm("Are you sure you want to delete this employee?")) {
      return;
    }

    const employeeService = new EmployeeServices();
    try {
      await employeeService.DeleteEmployee(id);
      setEmployees(employees.filter((employee) => employee.id !== id));
    } catch (error) {
      console.error("Error deleting employee:", error);
    }
  };

  if (loading) {
    return (
      <div className="loading">
        <CircularProgress />
      </div>
    );
  }

  return (
    <>
      <Container>
        <h1>Employee Management System</h1>
        <h2>Employee List</h2>
        <div>
          <h3>
            <NavLink to="/addEmployee">Add Employee</NavLink>
          </h3>
        </div>
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Full Name</TableCell>
                <TableCell>Email</TableCell>
                <TableCell>Phone Number</TableCell>
                <TableCell>Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {employees.length > 0 ? (
                employees.map((employee) => (
                  <TableRow key={employee.id}>
                    <TableCell>{employee.fullName}</TableCell>
                    <TableCell>{employee.email}</TableCell>
                    <TableCell>{employee.phone}</TableCell>
                    <TableCell>
                      <Button>
                        <NavLink to={`/updateEmployee/${employee.id}`}>
                          Edit
                        </NavLink>
                      </Button>
                      {" | "}
                      <Button onClick={() => handleDelete(employee.id)}>
                        Delete
                      </Button>
                    </TableCell>
                  </TableRow>
                ))
              ) : (
                <TableRow>
                  <TableCell colSpan={4}>No employees found</TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>
      </Container>
      <Container>
        <Logout />
      </Container>
    </>
  );
};

export default HomePage;
