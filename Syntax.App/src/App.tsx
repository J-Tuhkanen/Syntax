import React from 'react';
import './App.css';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import LoginPage from './pages/LoginPage';


const App = () => {
  return (
    <BrowserRouter>
      <div className="content">
        <Routes>
          <Route path="/login" element={<LoginPage/>}/>
        </Routes>
      </div>
    </BrowserRouter>
  );
}

export default App;
