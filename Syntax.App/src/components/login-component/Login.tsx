import "./Login.scss";
import React, { ChangeEvent, useState } from "react";

type LoginProps = {

    onRequestSignIn: Function
}

export const Login: React.FC<LoginProps> = ({ onRequestSignIn }) => {    

    const [loginFormData, setLoginFormData] = useState({
        username: "",
        password: ""
    });

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
        <form className="login-form " onSubmit={onLoginSubmit}>
            <div className="row col-md-4 justify-content-md-center">
                <div className="form-field-group col-md-10">
                    <input name="username" type="text" onChange={onFormFieldChange}/>
                </div>

                <div className="form-field-group col-md-10">
                    <input name="password" type="password" onChange={onFormFieldChange}/>
                </div>

                <div className="form-field-group col-md-10">
                    <button type="submit" className="btn btn-primary">Login</button>
                </div>
            </div>
        </form>
    </>);
}