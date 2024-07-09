import './NavBar.scss';
import { Link } from 'react-router-dom';
import { AuthenticationContext } from 'App';
import { useContext } from 'react';

export const NavBar: React.FC = () => {

    const authenticationContext = useContext(AuthenticationContext);
    const homeButtonText = '<Syntax/>';    

    return(<>
        <div className="navbar">
            {authenticationContext.isSignedIn 
                ? <> 
                    <Link to="/" style={{
                        width: '55%'
                    }}>{homeButtonText}</Link>
                    <Link to="#"style={{
                        width: '35%', textAlign: 'right'
                    }}>Logged in as {authenticationContext.User?.displayName}</Link>
                    <Link to="/signout" style={{
                        width: '10%', textAlign: 'right'
                    }}>Signout</Link>
                </> 
                : <>
                    <Link to='/login' style={{
                        width: '100%', textAlign: 'right'
                    }}>Login</Link>
                </>            
            }
        </div>
    </>);
};