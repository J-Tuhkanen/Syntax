import { SignupForm } from 'components/form/Signup'
import React, { useState } from 'react'

export const SignupView = () => {

  const [signUpSucceeded, setSignUpSucceeded] = useState<boolean>(false);

  return (
    <div className='container container-sm'>
        {signUpSucceeded 
          ? <p>Signup succeeded. Now login <a href="login">here</a></p>
          : <SignupForm/>}
        
    </div>
  )
}
