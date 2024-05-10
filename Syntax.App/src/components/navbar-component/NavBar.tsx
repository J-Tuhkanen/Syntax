import { Link } from 'react-router-dom';
import './NavBar.scss';
import { AuthenticationContext } from '../../App';
import { useContext, useEffect } from 'react';

export const NavBar: React.FC = () => {

    const authenticationContext = useContext(AuthenticationContext);

    return(<>
        <div className="navbar">
            {authenticationContext.isSignedIn 
                ? <> 
                    <Link to="#">Logged in as {authenticationContext.User?.displayName}</Link>
                    <Link to="/signout">Signout</Link>
                </> 
                : <>
                    <Link to='/login'>Login</Link>
                </>            
            }
        </div>
    </>);
};