import React from "react";
import { useNavigate } from "react-router-dom";
import {
  AppBar,
  Toolbar,
  Typography,
  Button,
  Container,
  Box,
  Grid,
  Card,
  CardContent,
  CardActions,
  ThemeProvider,
  createTheme,
  CssBaseline,
} from "@mui/material";
import { Home, Group, Add, Search } from "@mui/icons-material";
import { useAuth } from "../components/AuthContext";

const theme = createTheme({
  palette: {
    primary: {
      main: "#8E44AD",
    },
    secondary: {
      main: "#fff2cc",
    },
    background: {
      default: "#ffffff",
    },
  },
  typography: {
    h2: {
      fontWeight: 600,
      color: "#8E44AD",
    },
    h4: {
      fontWeight: 500,
      color: "#8E44AD",
    },
  },
});

export const LandingPage: React.FC = () => {
  const navigate = useNavigate();
  const { user } = useAuth();

  const navigationCards = [
    {
      title: "View Properties",
      description: "Browse available long-term rental properties",
      icon: <Search sx={{ fontSize: 40, color: "#8E44AD" }} />,
      path: "/properties",
      buttonText: "Browse Properties",
    },
    {
      title: "Find Roommates",
      description:
        "Discover group listings and connect with potential roommates",
      icon: <Group sx={{ fontSize: 40, color: "#8E44AD" }} />,
      path: "/group-listings",
      buttonText: "Find Groups",
    },
    {
      title: "List Your Property",
      description:
        "Create a listing for your rental property (only for registered users)",
      icon: <Home sx={{ fontSize: 40, color: "#8E44AD" }} />,
      path: "/property/create",
      buttonText: "Create Listing",
      needRegistered: true,
    },
    {
      title: "Create Group Listing",
      description:
        "Start a group to find roommates for shared accommodation (only for registered users)",
      icon: <Add sx={{ fontSize: 40, color: "#8E44AD" }} />,
      path: "/group-listing/create",
      buttonText: "Create Group",
      needRegistered: true,
    },
  ];

  const handleNavigation = (path: string) => {
    navigate(path);
  };

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <AppBar position="static" sx={{ backgroundColor: "#8E44AD" }}>
        <Toolbar>
          <Home sx={{ mr: 2 }} />
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            SleepSpot
          </Typography>
        </Toolbar>
      </AppBar>
      <Box
        sx={{
          background: "linear-gradient(135deg, #fff2cc 0%, #ffffff 100%)",
          py: 8,
          textAlign: "center",
        }}
      >
        <Container maxWidth="md">
          <Typography variant="h2" component="h1" gutterBottom>
            Find Your Perfect Long-Term Rental
          </Typography>
          <Typography variant="h6" color="text.secondary" paragraph>
            Connect with property owners and potential roommates for hassle-free
            long-term housing solutions
          </Typography>
        </Container>
      </Box>
      <Container maxWidth="lg" sx={{ py: 6 }}>
        <Typography
          variant="h4"
          component="h2"
          textAlign="center"
          gutterBottom
          sx={{ mb: 4 }}
        >
          What would you like to do?
        </Typography>
        <Grid container spacing={4}>
          {navigationCards.map((card, index) => (
            <Grid size={{ xs: 12, sm: 6, md: 3 }} key={index}>
              <Card
                sx={{
                  height: "100%",
                  display: "flex",
                  flexDirection: "column",
                  transition:
                    "transform 0.2s ease-in-out, box-shadow 0.2s ease-in-out",
                  "&:hover": {
                    transform: "translateY(-4px)",
                    boxShadow: "0 8px 25px rgba(142, 68, 173, 0.15)",
                  },
                }}
              >
                <CardContent sx={{ flexGrow: 1, textAlign: "center", py: 3 }}>
                  <Box sx={{ mb: 2 }}>{card.icon}</Box>
                  <Typography
                    variant="h6"
                    component="h3"
                    gutterBottom
                    sx={{ color: "#8E44AD", fontWeight: 600 }}
                  >
                    {card.title}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    {card.description}
                  </Typography>
                </CardContent>
                <CardActions sx={{ justifyContent: "center", pb: 3 }}>
                  <Button
                    disabled={card.needRegistered && !user}
                    variant="contained"
                    onClick={() => handleNavigation(card.path)}
                    sx={{
                      backgroundColor: "#8E44AD",
                      "&:hover": {
                        backgroundColor: "#7D3C98",
                      },
                      borderRadius: 2,
                      px: 3,
                    }}
                  >
                    {card.buttonText}
                  </Button>
                </CardActions>
              </Card>
            </Grid>
          ))}
        </Grid>
      </Container>
      <Box
        sx={{
          backgroundColor: "#8E44AD",
          color: "white",
          py: 3,
          mt: 6,
        }}
      >
        <Container>
          <Typography variant="body2" textAlign="center">
            © 2025 SleepSpot. Making long-term rentals simple and connected.
          </Typography>
        </Container>
      </Box>
    </ThemeProvider>
  );
};
