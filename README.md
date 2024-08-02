# AI Cloud File Storage System

Welcome to the AI Cloud File Storage System, developed as part of the MSA Phase 2 Software Development project. This project showcases a comprehensive web application leveraging modern technologies to provide an AI-powered PDF summarization and cloud storage solution.

## Project Overview

The AI Cloud File Storage System is a full-stack web application with the following features:

- **Frontend**: Developed using React and TypeScript, styled with Material-UI (MUI) for a visually appealing and responsive design on both computer and mobile devices.
- **Backend**: Built with .NET6 C# and Entity Framework Core (EFCore) for efficient database operations.
- **Database**: Utilizes Microsoft SQL Server for data storage.
- **AI Integration**: Integrates OpenAI API to summarize PDF content.
- **State Management**: Uses Redux for global state management.
- **Theming**: Supports dark and light themes with a switcher.
- **Deployment**: Deployed on Microsoft Azure and containerized using Docker.

## Features

- **CRUD Operations**: Basic Create, Read, Update, and Delete operations for PDF files.
- **File Upload**: Upload PDF files (less than two pages less than 1000 tokens) to Microsoft Blob Storage.
- **PDF Summarization**: Automatically generate summaries for uploaded PDF files using OpenAI API.
- **Search Functionality**: Search for PDF files by name or ID.
- **Theming**: Switch between dark and light themes.
- **Unit Testing**: Includes a comprehensive unit test suite.
- **React Router**: Implemented for efficient client-side routing.

## Deployed Application

You can access the deployed application [here](https://aifilestoragesystemweb.azurewebsites.net). Please note that the web application may take a minute to respond due to cold start, as Azure supports a shared plan for free.

## Usage Instructions

1. **Upload PDF File**: On the main page, select a PDF file (less than two pages) and click the "Upload" button. Wait for the "successfully uploaded" message.
2. **Search PDF File**: Use the search bar to find your uploaded PDF by name or ID (case and space sensitive).
3. **View and Update PDF File**: Once found, you can view the file name and summary. Modify the details and click "Update" to save changes.
4. **Delete PDF File**: Click the "Delete" button to remove the file from the system.

## Technologies Used

- **Frontend**: React, TypeScript, Material-UI (MUI)
- **Backend**: .NET C#, Entity Framework Core (EFCore)
- **Database**: Microsoft SQL Server
- **State Management**: Redux
- **Theming**: Dark and Light theme switcher
- **Containerization**: Docker
- **AI Integration**: OpenAI API
- **Deployment**: Microsoft Azure

## Getting Started

### Prerequisites

- Node.js
- .NET SDK
- Docker

### Notice you may not able to run this project by clone this repository

The connection string and openai key are ignored, so you may try the functionalities by accessing the website [here](https://aifilestoragesystemweb.azurewebsites.net).
If the MSA team requires to have all the information please contanct me with this email heatherpluto@gmail.com or songyvze2021@outlook.com

### There are two test pdf files, you may use to test the functionality of my app

Resume.pdf
academic transcript.pdf
