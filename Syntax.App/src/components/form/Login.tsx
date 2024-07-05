import "./Form.scss";
import React, { ChangeEvent, useState } from "react";

type LoginProps = {

    onRequestSignIn: Function
}

export const LoginForm: React.FC<LoginProps> = ({ onRequestSignIn }) => {    

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
            {/* <div className="col-md-6 justify-content-md-center login-description">
                <h4>Welcome to Syntax!</h4>
                <p>This is a social network for programming. You can find all kinds of topics about different languages and 
                    projects on our platform.
                </p>

                <p>
                    You need to sign in or if you haven't, <a href="/register">sign up</a> and start browsing.
                </p>
            </div> */}
        </form>
    </>);
}