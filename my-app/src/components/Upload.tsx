import React, { useState } from 'react';
import axios from 'axios';
import { Button, TextField, Typography, Container, Box, CircularProgress } from '@mui/material';

const Upload: React.FC = () => {
  const [file, setFile] = useState<File | null>(null);
  const [fileName, setFileName] = useState<string>('');
  const [uploadStatus, setUploadStatus] = useState<string>('');
  const [loading, setLoading] = useState<boolean>(false);

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFile = event.target.files?.[0] || null;
    setFile(selectedFile);
    setFileName(selectedFile ? selectedFile.name : '');
  };

  const handleUpload = async () => {
    if (!file) {
      alert('Please select a file to upload');
      return;
    }

    const formData = new FormData();
    formData.append('file', file);
    setLoading(true);

    try {
      const response = await axios.post('https://aifilestoragesystemapi.azurewebsites.net/PdfFiles/PostPdfFile', formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      });
      setUploadStatus('File uploaded successfully');
      console.log('Upload response:', response.data);
    } catch (error) {
      setUploadStatus('Error uploading file');
      console.error('Error uploading file:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container>
      <Typography variant="h4" gutterBottom>
        Upload PDF File
      </Typography>
      <Box my={2}>
        <TextField
          variant="outlined"
          fullWidth
          type="file"
          onChange={handleFileChange}
          helperText={fileName}
        />
      </Box>
      <Box my={2}>
        <Button variant="contained" color="primary" onClick={handleUpload} disabled={loading}>
          Upload
        </Button>
      </Box>
      {loading && <CircularProgress />}
      {uploadStatus && (
        <Typography variant="body1" color="error">
          {uploadStatus}
        </Typography>
      )}
    </Container>
  );
};

export default Upload;
