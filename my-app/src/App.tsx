import React from 'react';
import logo from './logo.svg';
import './App.css';
import Upload from './components/Upload';
import { BrowserRouter as Router, Route, Routes, useNavigate } from 'react-router-dom';
import { ThemeProvider, createTheme, CssBaseline, Button, Box } from '@mui/material';
import { SettingsProvider, useSettings } from './Theme';
import SearchComponent from './components/SearchComponent';
import PdfDetailComponent from './components/PdfDetailComponent';

function BackToHomeButton() {
  const navigate = useNavigate();

  const handleBackToHome = () => {
    navigate('/');
  };

  return (
    <Button variant="contained" color="secondary" onClick={handleBackToHome} style={{ marginTop: '10px' }}>
      Back to Home
    </Button>
  );
}

function AppContent() {
  const { isDarkTheme, toggleDarkTheme } = useSettings();

  const theme = createTheme({
    palette: {
      mode: isDarkTheme ? 'dark' : 'light',
      primary: {
        main: isDarkTheme ? '#ffffff' : '#000000',
      },
    },
  });

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Router>
        <div className="App">
          <header className="App-header">
            <div className="App-logo-container">
              <img src={logo} className="App-logo" alt="logo" />
            </div>
            <div className="theme-switcher">
              <Button variant="contained" onClick={toggleDarkTheme}>
                Switch to {isDarkTheme ? 'Light' : 'Dark'} Theme
              </Button>
            </div>
            <Routes>
              <Route path="/" element={
                <>
                  <Upload />
                  <SearchComponent />
                </>
              } />
              <Route path="/pdf/:id" element={
                <>
                  <PdfDetailComponent />
                  <BackToHomeButton />
                </>
              } />
            </Routes>
          </header>
        </div>
      </Router>
    </ThemeProvider>
  );
}

function App() {
  return (
    <SettingsProvider>
      <AppContent />
    </SettingsProvider>
  );
}

export default App;
