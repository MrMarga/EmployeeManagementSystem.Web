import React from "react";

const Logout = () => {
  const onLogout = () => {
    localStorage.clear();
    window.location.href = "/login";
  };

  return (
    <div>
      <div onClick={onLogout}> Logout</div>
    </div>
  );
};

export default Logout;
