import React from "react";
import { Navigate } from "react-router-dom";
import { jwtDecode } from "jwt-decode";

const ProtectedServiceRoute = ({ element }) => {
  const token = localStorage.getItem("token");
  let isAdmin = false;

  if (token) {
    try {
      const decodedToken = jwtDecode(token);
      isAdmin = decodedToken.role === "Admin";
    } catch (error) {
      console.error("Error decoding token:", error);
    }
  }

  return isAdmin ? element : <Navigate to="/homePage" />;
};

export default ProtectedServiceRoute;
