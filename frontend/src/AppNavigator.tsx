import { Route, Routes } from "react-router-dom";
import { PropertiesPage } from "./components/PropertiesPage";
import { AuthForm } from "./components/AuthForm";
import { LandingPage } from "./components/LandingPage";
import { PropertyDetailsPage } from "./components/PropertyDetailsPage";

export const AppNavigator: React.FC = () => {
  return (
    <Routes>
      <Route path="/main" element={<LandingPage />} />
      <Route path="/login" element={<AuthForm />} />
      <Route path="/properties" element={<PropertiesPage />} />
      <Route path="/property/:id" element={<PropertyDetailsPage />} />
    </Routes>
  );
};
