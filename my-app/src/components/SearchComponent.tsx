import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import { TextField, Button, Container, Typography, Box } from '@mui/material';
import { PuffLoader } from 'react-spinners';

const SearchComponent: React.FC = () => {
  const [searchTerm, setSearchTerm] = useState<string>('');
  const [searchBy, setSearchBy] = useState<'id' | 'filename'>('filename');
  const [loading, setLoading] = useState<boolean>(false);
  const navigate = useNavigate();

  const handleSearch = async () => {
    setLoading(true);
    try {
      console.log('Searching for PDF file with:', searchTerm); // Added logging
      const endpoint = searchBy === 'id'
        ? `https://aifilestoragesystemapi.azurewebsites.net/PdfFiles/${searchTerm}`
        : `https://aifilestoragesystemapi.azurewebsites.net/PdfFiles/SearchByFilename?filename=${searchTerm}`;
      
      const response = await axios.get(endpoint);
      console.log('Response from server:', response.data); // Added logging

      if (response.data) {
        if (searchBy === 'id') {
          navigate(`/pdf/${response.data.id}`, { state: { pdfFile: response.data } });
        } else {
          navigate(`/pdf/${response.data[0].id}`, { state: { pdfFile: response.data[0] } }); // Navigate to the first result for simplicity
        }
      } else {
        alert('File not found');
      }
    } catch (error) {
      console.error('Error searching for PDF file:', error); // Enhanced error logging
      if (axios.isAxiosError(error)) {
        console.error('Axios error response:', error.response); // Log the response from the server
      }
      alert('Error searching for PDF file');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container>
      <Typography variant="h4" gutterBottom>
        Search PDF Files
      </Typography>
      <div>
        <Button
          variant="contained"
          onClick={() => setSearchBy('filename')}
          color={searchBy === 'filename' ? 'primary' : 'inherit'}
          style={{ marginRight: '10px' }}
        >
          Search by Filename
        </Button>
        <Button
          variant="contained"
          onClick={() => setSearchBy('id')}
          color={searchBy === 'id' ? 'primary' : 'inherit'}
        >
          Search by ID
        </Button>
      </div>
      <TextField
        label={searchBy === 'id' ? 'Enter PDF File ID' : 'Enter PDF File Name'}
        variant="outlined"
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
        fullWidth
        margin="normal"
      />
      <Box my={2} display="flex" justifyContent="center" alignItems="center">
        <Button variant="contained" color="primary" onClick={handleSearch} disabled={loading}>
          {loading ? 'Searching...' : 'Search'}
        </Button>
        {loading && (
          <Box ml={2}>
            <PuffLoader color="#123abc" loading={loading} size={24} />
          </Box>
        )}
      </Box>
    </Container>
  );
};

export default SearchComponent;
