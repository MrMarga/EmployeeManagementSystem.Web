import React from "react";
import { Navigate } from "react-router-dom";

const ProtectedLoginRoute = ({ element }) => {
  const isAuthenticated = !!localStorage.getItem("UserRole");
  return isAuthenticated ? <Navigate to="/homePage" /> : element;
};

export default ProtectedLoginRoute;
