import "./App.css";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import SignUp from "./Component/SignUp";
import Login from "./Component/Login";
import HomePage from "./Component/HomePage";
import AddEmployee from "./Component/AddEmployee";
import UpdateEmployee from "./Component/UpdateEmployee";

function App() {
  return (
    <>
      <div>
        <Router>
          <Routes>
            <Route exact path="/" element={<SignUp />} />
            <Route exact path="/Login" element={<Login />} />
            <Route exact path="/HomePage" element={<HomePage />} />
            <Route exact path="/AddEmployee" element={<AddEmployee />} />
            <Route
              exact
              path="/UpdateEmployee/:id"
              element={<UpdateEmployee />}
            />
          </Routes>
        </Router>
      </div>
    </>
  );
}

export default App;
