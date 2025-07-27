import { Route, Routes } from "react-router-dom";
import { AuthForm } from "./components/AuthForm";
import { LandingPage } from "./components/LandingPage";
import { PropertyDetailsPage } from "./components/PropertyDetailsPage";
import { PropertiesAndRoomsPage } from "./components/PropertiesAndRoomsPage";
import { RoomDetailsPage } from "./components/RoomDetailsPage";
import { PropertyCreationPage } from "./components/CreateProperty/PropertyCreationPage";

export const AppNavigator: React.FC = () => {
  return (
    <Routes>
      <Route path="/main" element={<LandingPage />} />
      <Route path="/login" element={<AuthForm />} />
      <Route path="/properties" element={<PropertiesAndRoomsPage />} />
      <Route path="/rooms" element={<PropertiesAndRoomsPage />} />
      <Route path="/property/:id" element={<PropertyDetailsPage />} />
      <Route path="/room/:id" element={<RoomDetailsPage />} />
      <Route path="/property/create" element={<PropertyCreationPage />} />
    </Routes>
  );
};
