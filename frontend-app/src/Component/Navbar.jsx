import React, { useState, useEffect } from "react";
import {
  AppBar,
  Toolbar,
  Typography,
  Avatar,
  Box,
  Button,
} from "@mui/material";
import AuthServices from "../Services/AuthServices";
import Logout from "./Logout";

const Navbar = () => {
  const [user, setUser] = useState(null);
  const authServices = new AuthServices();

  useEffect(() => {
    const fetchUserData = async () => {
      try {
        const userId = localStorage.getItem("UserId");
        if (userId) {
          const response = await authServices.GetUserById(userId);
          const userData = response.data;

          if (userData) {
            setUser(userData);
          }
        }
      } catch (error) {
        console.error("Error fetching user data:", error);
      }
    };

    fetchUserData();
  }, []);

  return (
    <AppBar position="static">
      <Toolbar>
        <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
          Employee Management System
        </Typography>
        {user && (
          <Box display="flex" alignItems="center" ml={2}>
            <Avatar alt={user.name} src={user.profileImagePath} />
            <Box ml={2}>
              <Typography variant="body1">{user.name}</Typography>
              <Typography variant="body2">{user.role}</Typography>
            </Box>
          </Box>
        )}
        <Box ml={2}>
          <Button
            sx={{
              backgroundColor: "white",
              color: "#1976D2",
              "&:hover": {
                backgroundColor: "Black",
                color: "White",
              },
            }}
          >
            <Logout />
          </Button>
        </Box>
      </Toolbar>
    </AppBar>
  );
};

export default Navbar;
