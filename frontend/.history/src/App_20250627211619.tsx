import React from "react";
import "./App.css";
import Layout from "./components/Layout";
import { AppNavigator } from "./AppNavigator";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

export const App = () => {
  return (
    <Layout>
      <ToastContainer position="bottom-left" autoClose={3000} />
      <AppNavigator />
      {/* tu dodasz swoje komponenty, np. listę nieruchomości */}
    </Layout>
  );
};
