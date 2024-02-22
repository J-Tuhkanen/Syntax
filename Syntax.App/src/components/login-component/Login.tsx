import "./Login.scss";
import React, { ChangeEvent, useState } from "react";

type LoginProps = {

    onRequestSignIn: Function
}

export const Login: React.FC<LoginProps> = ({ onRequestSignIn }) => {    

    const [email, setEmail] = useState("");    
    const [emailError, setEmailError] = useState("");

    const [password, setPassword] = useState("");
    const [passwordError, setPasswordError] = useState("");
    
    const formHasErrors = (): boolean => {

        let hasErrors = false;

        if (email.trim().length < 1) {
            setEmailError("Email field cannot be empty.");
            hasErrors = true;
        }

        if (password.trim().length < 1) {

            setPasswordError("Password field cannot be empty");
            hasErrors = true;
        }

        return hasErrors;
    }

    const onLoginSubmit = async (e:React.FormEvent<HTMLFormElement>) => {
        
        e.preventDefault();
        
        if (formHasErrors() === false)
            await onRequestSignIn({ email, password });        
    }

    const onEmailchange = (e: ChangeEvent<HTMLInputElement>) => {

        setEmailError("");
        setEmail(e.target.value);
    }

    const onPasswordChange = (e: ChangeEvent<HTMLInputElement>) => {

        setPasswordError("");
        setPassword(e.target.value);
    } 

    return (<>
        <form className="login-form " onSubmit={onLoginSubmit}>
            <div className="row col-md-4 justify-content-md-center">
                <div className="form-field-group col-md-10">
                    <input type="text" onChange={onEmailchange}/>
                </div>

                <div className="form-field-group col-md-10">
                    <input type="password" onChange={onPasswordChange}/>
                </div>

                <div className="form-field-group col-md-10">
                    <button type="submit" className="btn btn-primary">Login</button>
                </div>
            </div>
        </form>
    </>);
}