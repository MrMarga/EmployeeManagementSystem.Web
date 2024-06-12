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
import AuthServices from "../Services/AuthServices";
import Logout from "./Logout";

const HomePage = () => {
  const [employees, setEmployees] = useState([]);
  const [loading, setLoading] = useState(true);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(5);
  const [totalEmployees, setTotalEmployees] = useState(0);
  const [user, setUser] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      const employeeService = new EmployeeServices();
      const authServices = new AuthServices();
      setLoading(true);
      try {
        const employeeResponse = await employeeService.GetAllEmployee(
          pageNumber,
          pageSize
        );
        if (employeeResponse && employeeResponse.data) {
          setEmployees(employeeResponse.data.employees);
          setTotalEmployees(employeeResponse.data.totalCount);
        } else {
          console.error("Unexpected response structure:", employeeResponse);
        }

        const userId = localStorage.getItem("UserId");
        if (userId) {
          const userResponse = await authServices.GetUserById(userId);
          if (userResponse) {
            setUser(userResponse);
          } else {
            console.error("Unexpected response structure:", userResponse);
          }
        }
      } catch (error) {
        console.error("Error fetching data:", error);
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

  const isAdmin = () => {
    const role = localStorage.getItem("UserRole");
    if (role) {
      const userRole = role;
      return userRole === "Admin";
    }
    return false;
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
        <Typography variant="h4" component="h2" gutterBottom>
          Employees List
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
                {isAdmin() && <TableCell>Actions</TableCell>}
              </TableRow>
            </TableHead>
            <TableBody>
              {employees.length > 0 ? (
                employees.map((employee) => (
                  <TableRow key={employee.id}>
                    <TableCell>{employee.id}</TableCell>
                    <TableCell>{employee.fullName}</TableCell>
                    <TableCell>{employee.email}</TableCell>
                    <TableCell>{employee.phone}</TableCell>
                    {isAdmin() && (
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
                    )}
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
    </>
  );
};

export default HomePage;
