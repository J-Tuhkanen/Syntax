import React, { useEffect } from 'react'
import { useNavigate } from 'react-router-dom';
import { sendHttpRequest } from 'utils/httpRequest';
import { AuthenticationState } from 'models/AuthenticationState';

type SignoutViewProps = {
    setAuthStateFunction: React.Dispatch<React.SetStateAction<AuthenticationState>>
};

const SignoutView: React.FC<SignoutViewProps> = ({setAuthStateFunction}) => {

    const navigate = useNavigate();
    
    useEffect(()=>{
        
        // Simple call to signout the cookie and navigate back to front as default.
        const signoutCall = async (): Promise<void> => {
            await sendHttpRequest({ method: "POST", endpoint: "authentication/logout" });
            setAuthStateFunction({ isSignedIn: false, User: undefined })
            navigate("/");
        }

        signoutCall();
    }, [])

    return (
        <></>
    )
}

export default SignoutView  