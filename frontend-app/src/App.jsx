import React from "react";
import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
} from "react-router-dom";
import SignUp from "./Component/SignUp";
import Login from "./Component/Login";
import HomePage from "./Component/HomePage";
import AddEmployee from "./Component/AddEmployee";
import UpdateEmployee from "./Component/UpdateEmployee";
import ProtectedRoute from "./Component/ProtectedRoute";
import ProtectedLoginRoute from "./Component/ProtectedLoginRoute";
import ForgotPassword from "./Component/ForgetPassword";
import PasswordReset from "./Component/PasswordReset";
import ProtectedServiceRoute from "./Component/ProtectedServiceRoute";

function App() {
  return (
    <Router>
      <Routes>
        {/* Public Routes */}
        <Route path="/" element={<SignUp />} />
        <Route
          path="/login"
          element={<ProtectedLoginRoute element={<Login />} />}
        />
        <Route path="/forgot-password" element={<ForgotPassword />} />
        <Route
          path="/reset-password/:email/:token"
          element={<PasswordReset />}
        />

        {/* Protected Routes */}
        <Route
          path="/homePage"
          element={<ProtectedRoute element={<HomePage />} />}
        />
        <Route
          path="/addEmployee"
          element={<ProtectedServiceRoute element={<AddEmployee />} />}
        />
        <Route
          path="/updateEmployee/:id"
          element={<ProtectedServiceRoute element={<UpdateEmployee />} />}
        />

        {/* Fallback Route */}
        <Route path="*" element={<Navigate to="/" />} />
      </Routes>
    </Router>
  );
}

export default App;
