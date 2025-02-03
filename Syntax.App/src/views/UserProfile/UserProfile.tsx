import "./UserProfile.scss";

import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { CommentDto, TopicDto, UserSettingsDto } from "dtos/Dtos";
import { ProfileEditComponent } from "views/UserProfile/components/Edit";
import { ProfileViewComponent } from "./components/View";

export const UserProfile: React.FC = () => {

    const [userDetails, setUserDetails] = useState<{comments:CommentDto[], topics:TopicDto[]}>();
    const navigate = useNavigate();
    const { userId } = useParams();    
    const [isInEditMode, setIsInEditMode] = useState<boolean>(false);
    const [userSettings, setUserSettings] = useState<UserSettingsDto>();
    
    const getUserDetails = async () => {
        const requestUri = `https://localhost:7181/api/user/details/${userId}`;
        const requestCredentials: RequestCredentials = 'include';
        const requestData = {
            method: "GET",
            credentials: requestCredentials       
        };

        var response = await fetch(requestUri, requestData);

        if (response.ok) {
            setUserDetails(await response.json());
        }
        else{
            navigate("/login");
        }
    }

    useEffect(() => {
 
        const getUserSettings = async () => {
            const requestUri = `https://localhost:7181/api/user/settings/${userId}`;
            const requestCredentials: RequestCredentials = 'include';
            const requestData = {
                method: "GET",
                credentials: requestCredentials       
            };
    
            var response = await fetch(requestUri, requestData);

            if (response.ok) {
                var json = await response.json();
                setUserSettings(json);
            }
        }

        getUserDetails();
        getUserSettings();
    }, []);
    
    if (userSettings !== undefined) {        
        return isInEditMode === false 
            ? <ProfileViewComponent 
                settings={userSettings} 
                comments={userDetails?.comments ?? []} 
                topics={userDetails?.topics ?? []} 
                toggleFunction={setIsInEditMode}/>
            : <ProfileEditComponent 
                userInfo={userSettings} 
                toggleFunction={setIsInEditMode}/>
    }
    else {
        return <h3>Error loading user profile</h3>;
    }
};
