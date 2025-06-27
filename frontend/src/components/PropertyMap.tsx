import { useState, useEffect } from 'react';
import { MapContainer, TileLayer, Marker, Popup } from 'react-leaflet';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import { CircularProgress, Paper, Typography } from '@mui/material';
import LocationOnIcon from '@mui/icons-material/LocationOn';

// Create a properly positioned custom icon
const createCustomIcon = () => {
  return L.divIcon({
    html: `<div style="
      position: relative;
      width: 30px;
      height: 30px;
      background: #8E44AD;
      border-radius: 50% 50% 50% 0;
      transform: rotate(-45deg);
    ">
      <div style="
        position: absolute;
        width: 18px;
        height: 18px;
        background: white;
        border-radius: 50%;
        left: 6px;
        top: 6px;
        transform: rotate(45deg);
      "></div>
    </div>`,
    className: '',
    iconSize: [30, 30],
    iconAnchor: [15, 30],  // This is critical - point at bottom center of pin
    popupAnchor: [0, -30]  // This positions popup above the pin
  });
};

export const PropertyMap = ({ address }: { address: string }) => {
  const [coordinates, setCoordinates] = useState<[number, number] | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const geocodeAddress = async () => {
      setLoading(true);
      setError(null);
      
      try {
        const nominatimUrl = `https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(address)}&limit=1&addressdetails=1`;
        const proxyUrl = `https://api.allorigins.win/get?url=${encodeURIComponent(nominatimUrl)}`;
        
        const response = await fetch(proxyUrl);
        const data = await response.json();
        const results = JSON.parse(data.contents);
        
        if (results && results.length > 0) {
          const coords: [number, number] = [
            parseFloat(results[0].lat), 
            parseFloat(results[0].lon)
          ];
          setCoordinates(coords);
        } else {
          setError('Address not found');
        }
      } catch (error) {
        console.error('Geocoding error:', error);
        setError('Failed to geocode address');
      } finally {
        setLoading(false);
      }
    };

    if (address && address.trim()) {
      geocodeAddress();
    } else {
      setLoading(false);
      setError('No address provided');
    }
  }, [address]);

  if (loading) return (
    <Paper elevation={3} style={{ padding: '20px', textAlign: 'center' }}>
      <CircularProgress />
      <Typography variant="body1" style={{ marginTop: '10px' }}>
        Loading map...
      </Typography>
    </Paper>
  );

  if (error) return (
    <Paper elevation={3} style={{ padding: '20px', textAlign: 'center' }}>
      <LocationOnIcon color="error" fontSize="large" />
      <Typography variant="h6" color="error" style={{ marginTop: '10px' }}>
        {error}
      </Typography>
      <Typography variant="body1" style={{ marginTop: '5px' }}>
        Address: {address}
      </Typography>
    </Paper>
  );

  if (!coordinates) return (
    <Paper elevation={3} style={{ padding: '20px', textAlign: 'center' }}>
      <Typography variant="h6">
        No coordinates found for this address
      </Typography>
    </Paper>
  );

  return (
    <Paper elevation={3} style={{ height: '400px', width: '100%' }}>
      <MapContainer 
        center={coordinates} 
        zoom={15} 
        style={{ height: '100%', width: '100%' }}
        scrollWheelZoom={false}
      >
        <TileLayer 
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        />
        <Marker position={coordinates} icon={createCustomIcon()}>
          <Popup>
            <Typography variant="subtitle2">{address}</Typography>
          </Popup>
        </Marker>
      </MapContainer>
    </Paper>
  );
};