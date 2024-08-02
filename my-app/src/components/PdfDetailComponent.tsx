import React, { useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { Button, Container, Typography, Box, TextField } from '@mui/material';
import { PuffLoader } from 'react-spinners';

interface PdfFile {
  id: number;
  fileName: string;
  filePath: string;
  summary: string;
}

const PdfDetailComponent: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const { pdfFile } = location.state as { pdfFile: PdfFile };

  const [fileName, setFileName] = useState(pdfFile.fileName);
  const [summary, setSummary] = useState(pdfFile.summary);
  const [loading, setLoading] = useState<boolean>(false);

  const handleDelete = async () => {
    setLoading(true);
    try {
      await axios.delete(`https://aifilestoragesystemapi.azurewebsites.net/PdfFiles/${pdfFile.id}`);
      alert('File deleted successfully');
      navigate('/');
    } catch (error) {
      console.error('Error deleting PDF file:', error);
      alert('Error deleting PDF file');
    } finally {
      setLoading(false);
    }
  };

  const handleUpdate = async () => {
    setLoading(true);
    try {
      const updatedFile = { ...pdfFile, fileName, summary };
      await axios.put(`https://aifilestoragesystemapi.azurewebsites.net/PdfFiles/${pdfFile.id}`, updatedFile);
      alert('File updated successfully');
    } catch (error) {
      console.error('Error updating PDF file:', error);
      alert('Error updating PDF file');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container>
      <Typography variant="h4" gutterBottom>
        PDF File Details
      </Typography>
      <Box my={2}>
        <Typography variant="h6">File ID: {pdfFile.id}</Typography> {/* Display File ID */}
        <TextField
          label="File Name"
          variant="outlined"
          value={fileName}
          onChange={(e) => setFileName(e.target.value)}
          fullWidth
          margin="normal"
        />
        <Typography variant="h6">Summary:</Typography>
        <TextField
          label="Summary"
          variant="outlined"
          value={summary}
          onChange={(e) => setSummary(e.target.value)}
          fullWidth
          margin="normal"
          multiline
        />
      </Box>
      <Box my={2}>
        <Button variant="contained" color="secondary" onClick={handleUpdate} style={{ marginRight: '10px' }} disabled={loading}>
          {loading ? 'Updating...' : 'Update'}
        </Button>
        <Button variant="contained" color="error" onClick={handleDelete} disabled={loading}>
          {loading ? 'Deleting...' : 'Delete'}
        </Button>
      </Box>
      {loading && (
        <Box my={2}>
          <PuffLoader color="#123abc" loading={loading} size={60} />
        </Box>
      )}
    </Container>
  );
};

export default PdfDetailComponent;
