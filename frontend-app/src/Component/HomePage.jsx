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
  Pagination,
  Box,
  Typography,
} from "@mui/material";
import { NavLink } from "react-router-dom";
import EmployeeServices from "../Services/EmployeeServices";
import Logout from "./Logout";

const HomePage = () => {
  const [employees, setEmployees] = useState([]);
  const [loading, setLoading] = useState(true);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(5);
  const [totalEmployees, setTotalEmployees] = useState(0);

  useEffect(() => {
    const fetchData = async () => {
      const employeeService = new EmployeeServices();
      setLoading(true);
      try {
        const response = await employeeService.GetAllEmployee(
          pageNumber,
          pageSize
        );
        if (response && response.data) {
          setEmployees(response.data.employees);
          setTotalEmployees(response.data.totalCount);
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
  }, [pageNumber, pageSize]);

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

  const handlePageChange = (event, value) => {
    setPageNumber(value);
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
        <Typography variant="h3" component="h1" gutterBottom>
          Employee Management System
        </Typography>
        <Typography variant="h4" component="h2" gutterBottom>
          Employee List
        </Typography>
        <Box mb={3}>
          <Button
            variant="contained"
            color="primary"
            component={NavLink}
            to="/addEmployee"
          >
            Add Employee
          </Button>
        </Box>
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Emp.Id</TableCell>
                <TableCell>Full Name</TableCell>
                <TableCell>Email</TableCell>
                <TableCell>Phone Number</TableCell>
                <TableCell>Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {employees.length > 0 ? (
                employees.map((employee, index) => (
                  <TableRow key={index}>
                    <TableCell>{employee.id}</TableCell>
                    <TableCell>{employee.fullName}</TableCell>
                    <TableCell>{employee.email}</TableCell>
                    <TableCell>{employee.phone}</TableCell>
                    <TableCell>
                      <Button
                        variant="contained"
                        color="primary"
                        size="small"
                        component={NavLink}
                        to={`/updateEmployee/${employee.id}`}
                      >
                        Edit
                      </Button>
                      <Button
                        variant="contained"
                        color="secondary"
                        size="small"
                        onClick={() => handleDelete(employee.id)}
                        style={{ marginLeft: "10px" }}
                      >
                        Delete
                      </Button>
                    </TableCell>
                  </TableRow>
                ))
              ) : (
                <TableRow>
                  <TableCell colSpan={5}>No employees found</TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>
        <Box mt={3} display="flex" justifyContent="center">
          <Pagination
            count={Math.ceil(totalEmployees / pageSize)}
            page={pageNumber}
            onChange={handlePageChange}
            color="primary"
            size="large"
            showFirstButton
            showLastButton
          />
        </Box>
      </Container>
      <Container>
        <Logout />
      </Container>
    </>
  );
};

export default HomePage;
