import "./UserProfile.scss";

import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { CommentDto, TopicDto, UserInformationDto, UserSettingsDto } from "dtos/Dtos";
import { ProfileEditComponent } from "views/UserProfile/components/Edit";
import { ProfileViewComponent } from "./components/View";

export const UserProfile: React.FC = () => {

    const { userId } = useParams();
    const navigate = useNavigate();  
    const [isInEditMode, setIsInEditMode] = useState<boolean>(false);
    const [userInformation, setUserInformation] = useState<UserInformationDto>();
    
    useEffect(() => {
 
        const getUserDetails = async () => {
            const requestUri = `https://localhost:7181/api/user/details/${userId}`;
            const requestCredentials: RequestCredentials = 'include';
            const requestData = {
                method: "GET",
                credentials: requestCredentials       
            };
    
            var response = await fetch(requestUri, requestData);
    
            if (response.ok) {
                setUserInformation(await response.json());
            }
            else{
                navigate("/login");
            }
        }

        getUserDetails();
    }, []);
    
    if (userInformation !== undefined) {        
        return isInEditMode === false 
            ? <ProfileViewComponent 
                userInfo={userInformation} 
                comments={userInformation.activity?.comments ?? []} 
                topics={userInformation.activity?.topics ?? []} 
                toggleFunction={setIsInEditMode}/>
            : <ProfileEditComponent 
                userInfo={userInformation} 
                toggleFunction={setIsInEditMode}/>
    }
    else {
        return <h3>Error loading user profile</h3>;
    }
};
