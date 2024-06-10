import React from "react";
import { Navigate } from "react-router-dom";

const ProtectedServiceRoute = ({ element }) => {
  const role = localStorage.getItem("UserRole");
  let isAdmin = false;

  if (role) {
    try {
      isAdmin = role === "Admin";
    } catch (error) {
      console.error("Error decoding token:", error);
    }
  }

  return isAdmin ? element : <Navigate to="/homePage" />;
};

export default ProtectedServiceRoute;
