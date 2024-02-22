import { Login } from '../components/login-component/Login';
import { sendRequest } from '../helpers/apiRequestHelpers';
// import { InfoMessage, InfoMessageType } from "../components/info-message-component/InfoMessage";
import { useNavigate } from "react-router-dom";
import { useState } from 'react';

type RequestSignInProps = {

    email: string;
    password: string;
}
type LoginPageProps = {

    // setAuthStateFunction: Function
}

const LoginPage: React.FC<LoginPageProps> = () => {

    const navigate = useNavigate();
    const [errorModel, setErrorModel] = useState({ showError: false, message: "" });
    const [isAuthenticating, setIsAuthenticating] = useState(false);

    const onRequestSignIn = async (props: RequestSignInProps): Promise<void> => {

        if (isAuthenticating)
            return;

        setErrorModel({ showError: false, message: "" });
        setIsAuthenticating(true);
        
        const response = await sendRequest({

            method: "POST",
            endpoint: "authentication/login",
            requestBody: {
                "username": "asdasd",
                "password": "strasdasding"
              }
        });

        if (response.status === 200) {

            // setAuthStateFunction({ isSignedIn: true, user: await response.json() });
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