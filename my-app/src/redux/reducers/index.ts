// src/redux/reducers/index.ts

import { combineReducers } from '@reduxjs/toolkit';
import pdfReducer from '../pdfSlice';

const rootReducer = combineReducers({
  pdf: pdfReducer,
});

export default rootReducer;

