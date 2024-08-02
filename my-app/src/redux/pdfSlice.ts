// src/redux/pdfSlice.ts

import { createSlice, PayloadAction } from '@reduxjs/toolkit';

interface PdfState {
  files: Array<{ id: number; fileName: string; filePath: string; summary: string }>;
}

const initialState: PdfState = {
  files: [],
};

const pdfSlice = createSlice({
  name: 'pdf',
  initialState,
  reducers: {
    addPdfFile(state, action: PayloadAction<{ id: number; fileName: string; filePath: string; summary: string }>) {
      state.files.push(action.payload);
    },
    removePdfFile(state, action: PayloadAction<number>) {
      state.files = state.files.filter((file) => file.id !== action.payload);
    },
  },
});

export const { addPdfFile, removePdfFile } = pdfSlice.actions;

export default pdfSlice.reducer;
