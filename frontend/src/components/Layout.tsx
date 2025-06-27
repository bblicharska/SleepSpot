import React, { forwardRef, useEffect, useState } from "react";
import {
  AppBar,
  Toolbar,
  Typography,
  Box,
  Drawer,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  CssBaseline,
  IconButton,
  Tooltip,
  Divider,
  Menu,
  MenuItem,
  Button,
  Snackbar,
} from "@mui/material";
import HomeIcon from "@mui/icons-material/Home";
import ApartmentIcon from "@mui/icons-material/Apartment";
import PersonIcon from "@mui/icons-material/Person";
import MenuOpenIcon from "@mui/icons-material/MenuOpen";
import MenuIcon from "@mui/icons-material/Menu";
import ListItemButton from "@mui/material/ListItemButton";
import MuiAlert, { AlertProps } from "@mui/material/Alert";
import { useLocation, useNavigate } from "react-router-dom";
import { jwtDecode } from "jwt-decode";
import { useAuth } from "./AuthContext";

const drawerWidth = 240;
const collapsedWidth = 72;

type JwtPayload = {
  firstName: string;
  lastName: string;
  exp?: number;
};

export default function Layout({ children }: { children: React.ReactNode }) {
  const [collapsed, setCollapsed] = useState(false);
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const navigate = useNavigate();
  const { pathname } = useLocation();
  const { token, user, logout, loggedOutDueToExpiry } = useAuth();
  const [showExpiryToast, setShowExpiryToast] = useState(loggedOutDueToExpiry);

  useEffect(() => {
    if (loggedOutDueToExpiry) {
      setShowExpiryToast(true);
    }
  }, [loggedOutDueToExpiry]);
  const toggleDrawer = () => setCollapsed(!collapsed);

  const isLoggedIn = !!token;

  const userFullName = user ? `${user.firstName} ${user.lastName}` : "User";

  const isLoginPage = pathname.toString().includes("login");

  const handleMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  const handleLogout = () => {
    logout(); // from AuthContext
    handleMenuClose();
  };

  return (
    <Box sx={{ display: "flex" }}>
      <CssBaseline />

      {/* AppBar */}
      <AppBar
        position="fixed"
        sx={{
          zIndex: (theme) => theme.zIndex.drawer + 1,
          whiteSpace: "nowrap",
          backgroundColor: "#fff2cc",
        }}
      >
        <Toolbar>
          <Box onClick={() => navigate("/main")} sx={{ cursor: "pointer" }}>
            <Typography variant="h6" noWrap sx={{ color: "#8E44AD" }}>
              SleepSpot
            </Typography>
          </Box>
          <Box sx={{ flexGrow: 1 }} />
          {isLoggedIn ? (
            <>
              <Box
                onClick={handleMenuOpen}
                sx={{
                  display: "flex",
                  alignItems: "center",
                  cursor: "pointer",
                  backgroundColor: "#f3e5f5", // very light purple background
                  borderRadius: 2,
                  px: 1.5,
                  py: 0.5,
                  gap: 1,
                  userSelect: "none",
                  minWidth: 180,
                  boxShadow: "0 1px 3px rgb(0 0 0 / 0.1)",
                  transition: "background-color 0.3s ease",
                  "&:hover": {
                    backgroundColor: "#e1bee7", // slightly darker purple on hover
                  },
                }}
              >
                <PersonIcon sx={{ color: "#6a1b9a", fontSize: 28 }} />

                <Box
                  sx={{
                    display: "flex",
                    flexDirection: "column",
                    lineHeight: 1,
                  }}
                >
                  <Typography
                    variant="subtitle1"
                    sx={{
                      fontWeight: 600,
                      color: "#4a148c",
                      whiteSpace: "nowrap",
                      textOverflow: "ellipsis",
                      overflow: "hidden",
                      maxWidth: 130,
                    }}
                  >
                    {userFullName}
                  </Typography>

                  <Typography
                    variant="caption"
                    color="text.secondary"
                    sx={{ fontStyle: "italic" }}
                  >
                    {user?.role || "User"}
                  </Typography>
                </Box>
              </Box>
              <Menu
                anchorEl={anchorEl}
                open={Boolean(anchorEl)}
                onClose={handleMenuClose}
                anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
                transformOrigin={{ vertical: "top", horizontal: "right" }}
                PaperProps={{
                  sx: {
                    width: 180,
                    bgcolor: "#f3e5f5", // Light lilac
                    borderRadius: 2,
                    boxShadow: "0 4px 10px rgba(0,0,0,0.15)",
                    mt: 1,
                  },
                }}
              >
                <MenuItem
                  onClick={handleLogout}
                  sx={{
                    color: "#4a148c",
                    "&:hover": {
                      backgroundColor: "#fce4ec", // Soft light pink/lilac
                    },
                    "&.Mui-focusVisible": {
                      backgroundColor: "#fce4ec",
                    },
                  }}
                >
                  Logout
                </MenuItem>
              </Menu>
            </>
          ) : (
            <Box
              onClick={() => navigate("/login")}
              sx={{
                display: "flex",
                alignItems: "center",
                cursor: "pointer",
                backgroundColor: "#f3e5f5", // same light purple
                borderRadius: 2,
                px: 1.5,
                py: 0.5,
                gap: 1,
                userSelect: "none",
                boxShadow: "0 1px 3px rgb(0 0 0 / 0.1)",
                transition: "background-color 0.3s ease",
                "&:hover": {
                  backgroundColor: "#e1bee7", // slightly darker on hover
                },
              }}
            >
              <PersonIcon sx={{ color: "#6a1b9a", fontSize: 28 }} />
              <Typography
                variant="subtitle1"
                sx={{
                  fontWeight: 600,
                  color: "#4a148c",
                  whiteSpace: "nowrap",
                }}
              >
                SIGN IN / SIGN UP
              </Typography>
            </Box>
          )}
        </Toolbar>
      </AppBar>

      {/* Drawer - Sidebar */}
      {!isLoginPage && (
        <Drawer
          variant="permanent"
          sx={{
            width: collapsed ? collapsedWidth : drawerWidth,
            flexShrink: 0,
            whiteSpace: "nowrap",
            [`& .MuiDrawer-paper`]: {
              width: collapsed ? collapsedWidth : drawerWidth,
              transition: "width 0.3s",
              overflowX: "hidden",
              boxSizing: "border-box",
              display: "flex",
              flexDirection: "column",
              justifyContent: "space-between",
            },
          }}
        >
          <div>
            <Toolbar />
            <List>
              <ListItem disablePadding onClick={() => navigate("/main")}>
                <ListItemButton>
                  <Tooltip
                    title="Main Page"
                    placement="right"
                    disableHoverListener={!collapsed}
                  >
                    <ListItemIcon>
                      <HomeIcon />
                    </ListItemIcon>
                  </Tooltip>
                  {!collapsed && <ListItemText primary="Main Page" />}
                </ListItemButton>
              </ListItem>
              <ListItem disablePadding onClick={() => navigate("/properties")}>
                <ListItemButton>
                  <Tooltip
                    title="Apartments"
                    placement="right"
                    disableHoverListener={!collapsed}
                  >
                    <ListItemIcon>
                      <ApartmentIcon />
                    </ListItemIcon>
                  </Tooltip>
                  {!collapsed && <ListItemText primary="Apartments" />}
                </ListItemButton>
              </ListItem>
            </List>
          </div>

          {/* Collapse/Expand toggle at bottom */}
          <Box sx={{ textAlign: "center", py: 1 }}>
            <Divider />
            <IconButton onClick={toggleDrawer}>
              {collapsed ? <MenuIcon /> : <MenuOpenIcon />}
            </IconButton>
          </Box>
        </Drawer>
      )}

      {/* Main content */}
      <Box component="main" sx={{ flexGrow: 1, p: 3, overflowX: "hidden" }}>
        <Toolbar />
        {children}
      </Box>
      <Snackbar
        open={showExpiryToast}
        autoHideDuration={6000}
        onClose={() => setShowExpiryToast(false)}
        anchorOrigin={{ vertical: "top", horizontal: "center" }}
      >
        <Alert
          onClose={() => setShowExpiryToast(false)}
          severity="warning"
          sx={{ width: "100%" }}
          action={
            <Button
              color="inherit"
              size="small"
              onClick={() => {
                setShowExpiryToast(false);
                navigate("/login");
              }}
            >
              LOGIN
            </Button>
          }
        >
          Your session expired. Please log in again.
        </Alert>
      </Snackbar>
    </Box>
  );
}

const Alert = forwardRef<HTMLDivElement, AlertProps>(function Alert(
  props,
  ref
) {
  return <MuiAlert elevation={6} ref={ref} variant="filled" {...props} />;
});
