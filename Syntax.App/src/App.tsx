import './App.css';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import LoginView from 'views/Login/Login';
import MainFeedView from 'views/MainFeed/MainFeedView';
import { NavBar } from 'components/navbar-component/NavBar';
import { createContext, useEffect, useState } from 'react';
import { sendHttpRequest } from 'utils/httpRequest';
import SignoutView from 'views/Signout/SignoutView';
import { AuthenticationState } from 'models/AuthenticationState';
import TopicView from 'views/TopicView/TopicView';
import { SignupView } from 'views/Signup/Signup';

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
            <Route path="/" element={<MainFeedView/>}/>
            <Route path="/login" element={<LoginView setAuthStateFunction={setAuthState}/>}/>
            <Route path="/signout" element={<SignoutView setAuthStateFunction={setAuthState}/>}/>
            <Route path="/signup" element={<SignupView/>}/>
            <Route path="/topic/:topicId" element={<TopicView/>}/>
          </Routes>
        </div>
      </AuthenticationContext.Provider>
    </BrowserRouter>
    </>
  );
}

export default App;
