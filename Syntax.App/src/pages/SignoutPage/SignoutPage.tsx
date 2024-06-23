import React, { useEffect } from 'react'
import { useNavigate } from 'react-router-dom';
import { sendRequest } from '../../helpers/apiRequestHelpers';
import { AuthenticationState } from '../../models/AuthenticationState';

type SignoutPageProps = {
    setAuthStateFunction: React.Dispatch<React.SetStateAction<AuthenticationState>>
};

const SignoutPage: React.FC<SignoutPageProps> = ({setAuthStateFunction}) => {

    const navigate = useNavigate();
    
    useEffect(()=>{
        
        // Simple call to signout the cookie and navigate back to front as default.
        const signoutCall = async (): Promise<void> => {
            await sendRequest({ method: "POST", endpoint: "authentication/logout" });
            setAuthStateFunction({ isSignedIn: false, User: undefined })
            navigate("/");
        }

        signoutCall();
    }, [])

    return (
        <></>
    )
}

export default SignoutPage  