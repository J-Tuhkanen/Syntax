import { useNavigate } from "react-router-dom";
import "./Form.scss";
import React, { ChangeEvent, useState } from "react";
import { sendHttpRequest } from "utils/httpRequest";
import { ApplicationUser } from "models/ApplicationUser";
import { AuthenticationState } from "models/AuthenticationState";
import { PopupInfo, PopupType } from "components/popup-info-component/PopupInfo";

type RequestSignInProps = {

    username: string;
    password: string;
}

export const LoginForm: React.FC<{ setAuthStateFunction: React.Dispatch<React.SetStateAction<AuthenticationState>> }> = ({ setAuthStateFunction }) => {    

    const [loginFormData, setLoginFormData] = useState({
        username: "",
        password: ""
    });
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
            
            setAuthStateFunction({ isSignedIn: true, User: await response.json()});
            navigate("/");
        }

        else if (response.status === 401){
            setErrorModel({ showError: true, message: "Authentication failed. Check email and password." });
        }

        setIsAuthenticating(false);
    };

    const formHasErrors = (): boolean => {

        let hasErrors = false;

        if (loginFormData.username.trim().length < 1) {
            //setEmailError("Email field cannot be empty.");
            hasErrors = true;
        }

        if (loginFormData.password.trim().length < 1) {
            //setPasswordError("Password field cannot be empty");
            hasErrors = true;
        }

        return hasErrors;
    }

    const onLoginSubmit = async (e:React.FormEvent<HTMLFormElement>) => {
        
        e.preventDefault();
        
        if (formHasErrors() === false) {
            await onRequestSignIn({
                username: loginFormData.username,
                password: loginFormData.password
            });        
        }
    }

    const onFormFieldChange = (e: ChangeEvent<HTMLInputElement>) => {

        setLoginFormData({
            ...loginFormData,
            [e.target.name]: e.target.value
        });
    }

    return (<>
        {errorModel.showError 
            ? <PopupInfo message={errorModel.message} type={PopupType.Error}/> 
            : null}

        <form className="login-form row" onSubmit={onLoginSubmit}>
            <div className="col-md-12 justify-content-md-center">

                <h3 className="col-md-12"><b>Sign in to your account</b></h3>

                <div className="form-field-group col-md-10">
                    <input className="top-input" name="username" type="text" placeholder="Username" onChange={onFormFieldChange}/>
                    <input className="bottom-input" name="password" type="password" placeholder="Password" onChange={onFormFieldChange}/>
                    {/* <label>Username</label> */}
                </div>
                
                <div className="form-field-group col-md-10">
                    <p>Don't have an account yet? <a href="signup">Sign up here</a></p>
                </div>

                <div className="form-field-group col-md-12">
                    <button type="submit" className="btn btn-primary">Login</button>
                </div>
            </div>
        </form>
    </>);
}