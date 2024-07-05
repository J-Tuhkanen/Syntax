import { sendHttpRequest } from "services/httpRequest";
import "./Form.scss";
import React, { ChangeEvent, useState } from "react";
import { SyntaxFormProps } from "./Form";

export const SignupForm: React.FC<SyntaxFormProps<number>> = (formProps) => {    

    const [signUpFormData, setLoginFormData] = useState({
        username: "",
        email: "",
        password: ""
    });

    const formHasErrors = (): boolean => {

        let hasErrors = false;

        if (signUpFormData.username.trim().length < 1) {
            //setEmailError("Username field cannot be empty.");
            hasErrors = true;
        }

        if (signUpFormData.email.trim().length < 1) {
            //setEmailError("Email field cannot be empty.");
            hasErrors = true;
        }

        if (signUpFormData.password.trim().length < 1) {
            //setPasswordError("Password field cannot be empty");
            hasErrors = true;
        }

        return hasErrors;
    }

    const onLoginSubmit = async (e:React.FormEvent<HTMLFormElement>) => {
        
        e.preventDefault();
        
        if (formHasErrors() === false) {
            
            const response = await sendHttpRequest({ method: "POST", endpoint: "authentication/register", requestBody: signUpFormData});
            formProps.submitResult(response.status)
        }
    }

    const onFormFieldChange = (e: ChangeEvent<HTMLInputElement>) => {

        setLoginFormData({
            ...signUpFormData,
            [e.target.name]: e.target.value
        });
    }

    return (<>
        <form className="login-form row" onSubmit={onLoginSubmit}>
            <div className="col-md-12 justify-content-md-center">

                <h3 className="col-md-12"><b>Create a new account</b></h3>

                <div className="form-field-group merged-inputs col-md-10">
                    <input className="top-input" name="username" type="text" placeholder="Username" onChange={onFormFieldChange}/>
                    <input name="email" type="text" placeholder="Email" onChange={onFormFieldChange}/>
                    <input className="bottom-input" name="password" type="password" placeholder="Password" onChange={onFormFieldChange}/>
                    {/* <label>Username</label> */}
                </div>

                <div className="form-field-group merged-inputs col-md-10">
                    <p>By signing up and loggin in, you agree with the <a href="rules">rules</a> of Syntax website</p>
                </div>

                <div className="form-field-group col-md-12">
                    <button type="submit" className="btn btn-primary">Create account</button>
                </div>
            </div>
        </form>
    </>);
}