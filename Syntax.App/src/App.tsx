import './App.css';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import LoginPage from 'views/LoginPage/LoginPage';
import MainFeedPage from 'views/MainFeedPage/MainFeedPage';
import { NavBar } from 'components/navbar-component/NavBar';
import { createContext, useEffect, useState } from 'react';
import { sendHttpRequest } from 'services/httpRequest';
import SignoutPage from 'views/SignoutPage/SignoutPage';
import { AuthenticationState } from 'models/AuthenticationState';
import TopicViewPage from 'views/TopicViewPage/TopicViewPage';

export const AuthenticationContext = createContext<AuthenticationState>({ isSignedIn: false});

const App = () => {
  const [authState, setAuthState] = useState<AuthenticationState>({ isSignedIn: false })

  useEffect(() => {
    const getCurrentSession = async (): Promise<void> => {

      const response = await sendHttpRequest({ method: "GET", endpoint: "authentication/session" });

      if(response.status === 200){
        setAuthState({ isSignedIn: true, User: await response.json()});
      }
    };
    
    getCurrentSession();
  }, []);

  return (
    <>
    <BrowserRouter>
      <AuthenticationContext.Provider value={authState}>
        <NavBar/>
        <div className="content">
          <Routes>
            <Route path="/" element={<MainFeedPage/>}/>
            <Route path="/login" element={<LoginPage setAuthStateFunction={setAuthState}/>}/>
            <Route path="/signout" element={<SignoutPage setAuthStateFunction={setAuthState}/>}/>
            <Route path="/topic/:topicId" element={<TopicViewPage/>}/>
          </Routes>
        </div>
      </AuthenticationContext.Provider>
    </BrowserRouter>
    </>
  );
}

export default App;
