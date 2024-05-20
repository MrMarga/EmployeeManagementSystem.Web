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
import TrendsPage from "./Component/TrendsPage";
import ForgotPassword from "./Component/ForgetPassword";

function App() {
  return (
    <div>
      <Router>
        <Routes>
          <Route path="/" element={<SignUp />} />
          <Route path="/login" element={<Login />} />
          <Route path="/forgot-password" element={<ForgotPassword />} />
          <Route
            path="/homePage"
            element={<ProtectedRoute element={<HomePage />} />}
          />
          <Route
            path="/addEmployee"
            element={<ProtectedRoute element={<AddEmployee />} />}
          />
          <Route
            path="/updateEmployee/:id"
            element={<ProtectedRoute element={<UpdateEmployee />} />}
          />
          <Route
            path="/trendsPage"
            element={<ProtectedRoute element={<TrendsPage />} />}
          />
          <Route path="*" element={<Navigate to="/" />} />
        </Routes>
      </Router>
    </div>
  );
}

export default App;
