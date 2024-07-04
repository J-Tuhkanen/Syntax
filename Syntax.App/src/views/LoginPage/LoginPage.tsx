import { Login } from 'components/login-component/Login';
import { sendHttpRequest } from 'services/httpRequest';
import { useNavigate } from "react-router-dom";
import { useState } from 'react';
import { AuthenticationState } from 'models/AuthenticationState';

type RequestSignInProps = {

    username: string;
    password: string;
}
type LoginPageProps = {

    setAuthStateFunction: React.Dispatch<React.SetStateAction<AuthenticationState>>
}

const LoginPage: React.FC<LoginPageProps> = ({setAuthStateFunction}) => {

    const navigate = useNavigate();
    const [errorModel, setErrorModel] = useState({ showError: false, message: "" });
    const [isAuthenticating, setIsAuthenticating] = useState(false);

    const onRequestSignIn = async (props: RequestSignInProps): Promise<void> => {

        if (isAuthenticating)
            return;

        setErrorModel({ showError: false, message: "" });
        setIsAuthenticating(true);

        const response = await sendHttpRequest({

            method: "POST",
            endpoint: "authentication/login",
            requestBody: {
                "username": props.username,
                "password": props.password
            },
            
        });

        if (response.status === 200) {

            setAuthStateFunction({ isSignedIn: true, User: await response.json() });
            navigate("/");
        }

        else if (response.status === 401){
            setErrorModel({ showError: true, message: "Authentication failed. Check email and password." });
        }

        setIsAuthenticating(false);

    };

    // if (isAuthenticating)
    //     return (<Loader />);

    return (
        <>
            <div className='container container-sm'>
                <Login onRequestSignIn={onRequestSignIn} />

            {/* {errorModel.showError
                ? <InfoMessage infoMessageType={InfoMessageType.Error} message={errorModel.message} />
            : null} */}
    
            </div>
        </>
    );

}

export default LoginPage;