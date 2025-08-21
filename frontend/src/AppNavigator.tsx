import { Route, Routes } from "react-router-dom";
import { AuthForm } from "./components/AuthForm";
import { LandingPage } from "./components/LandingPage";
import { PropertyDetailsPage } from "./components/PropertyDetailsPage";
import { PropertiesAndRoomsPage } from "./components/PropertiesAndRoomsPage";
import { RoomDetailsPage } from "./components/RoomDetailsPage";
import { PropertyCreationPage } from "./components/CreateProperty/PropertyCreationPage";
import { ChangePasswordPage } from "./components/ChangePasswordPage";
import { GroupListingsPage } from "./components/GroupListingsPage";
import { GroupListingDetailsPage } from "./components/GroupListingDetailsPage";
import { GroupListingCreationPage } from "./components/CreateListing/CreateGroupListingPage";
import { MyGroupsPage } from "./components/MyGroupsPage";
import { RentalRequests } from "./components/RentalRequests";
import { MyRequests } from "./components/MyRequests";

export const AppNavigator: React.FC = () => {
  return (
    <Routes>
      <Route path="/main" element={<LandingPage />} />
      <Route path="/change-password" element={<ChangePasswordPage />} />
      <Route path="/login" element={<AuthForm />} />
      <Route path="/properties" element={<PropertiesAndRoomsPage />} />
      <Route path="/rooms" element={<PropertiesAndRoomsPage />} />
      <Route path="/property/:id" element={<PropertyDetailsPage />} />
      <Route path="/room/:id" element={<RoomDetailsPage />} />
      <Route path="/property/create" element={<PropertyCreationPage />} />
      <Route path="/group-listings" element={<GroupListingsPage />} />
      <Route path="/group-listings/:id" element={<GroupListingDetailsPage />} />
      <Route
        path="/group-listing/create"
        element={<GroupListingCreationPage />}
      />
      <Route path="/my-groups" element={<MyGroupsPage />} />
      <Route path="/my-rental-requests" element={<RentalRequests />} />
      <Route path="/my-applications-rentals" element={<MyRequests />} />
    </Routes>
  );
};
