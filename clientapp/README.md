# Email Aliases React Frontend

This is the React frontend for the Email Aliases API.

## Setup

1. Install dependencies:
```bash
npm install
```

2. Start the development server:
```bash
npm run dev
```

The app will run on `http://localhost:5173` and proxy API requests to `http://localhost:5000` (or your API port).

## Building for Production

To build the React app for production:

```bash
npm run build
```

This will create a `wwwroot` directory in the parent folder with the production build. The .NET API will serve these static files.

## Development Workflow

1. Start the .NET API server (usually on port 5000 or 5267)
2. Start the React dev server: `npm run dev` (runs on port 5173)
3. The React dev server will proxy `/api` requests to the .NET API

## Production Deployment

In production, the React app is built and served as static files by the .NET API:
- Build the React app: `npm run build`
- The .NET API serves files from `wwwroot`
- All non-API routes are handled by the React app (SPA routing)

