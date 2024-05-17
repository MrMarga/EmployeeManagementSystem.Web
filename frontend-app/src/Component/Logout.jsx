import { Button } from "@mui/material";
import React from "react";

const Logout = () => {
  const onLogout = () => {
    localStorage.clear();
    window.location.href = "/login";
  };

  return (
    <div>
      <Button onClick={onLogout}>Logout</Button>
    </div>
  );
};

export default Logout;
