import React, { useEffect } from "react";
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

function App() {
  useEffect(() => {
    // Clear the cookie from local storage after 5 minutes
    const timeoutId = setTimeout(() => {
      localStorage.removeItem("userData");
      console.log("Cookie cleared");
      window.location.href = "/login";
    }, 2 * 60 * 1000); // 5 minutes in milliseconds

    return () => {
      clearTimeout(timeoutId);
    };
  }, []);

  return (
    <div>
      <Router>
        <Routes>
          <Route path="/" element={<SignUp />} />
          <Route path="/login" element={<Login />} />
          <Route path="/homePage" element={<HomePage />} />
          <Route path="/addEmployee" element={<AddEmployee />} />
          <Route path="/updateEmployee/:id" element={<UpdateEmployee />} />
          <Route path="*" element={<Navigate to="/" />} />
        </Routes>
      </Router>
    </div>
  );
}

export default App;
