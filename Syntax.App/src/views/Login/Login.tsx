import { LoginForm } from 'components/form/Login';
import { sendHttpRequest } from 'services/httpRequest';
import { useNavigate } from "react-router-dom";
import { useState } from 'react';
import { AuthenticationState } from 'models/AuthenticationState';
import { ApplicationUser } from 'models/ApplicationUser';

type LoginPageProps = {
    setAuthStateFunction: React.Dispatch<React.SetStateAction<AuthenticationState>>
}

const LoginPage: React.FC<LoginPageProps> = (pageProps:LoginPageProps) => {

    const OnSubmitResult = (user: ApplicationUser) => {
        
        pageProps.setAuthStateFunction({ isSignedIn: true, User: user });
    }

    return (
        <div className='container container-sm'>
            <LoginForm submitResult={OnSubmitResult}/>
        </div>
    );

}

export default LoginPage;