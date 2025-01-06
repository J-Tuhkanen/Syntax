import { ChangeEvent, FormEvent, useEffect, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { CommentDto, TopicDto, UserDto } from "dtos/Dtos";
import { FormateDateToTopicTimestamp } from "utils/dateFormatter";
import Tabs from 'components/general/tabs';
import ToggleSwitch from 'components/general/switch';
import "./UserProfile.scss";

type UserDetails = {
    user: UserDto,
    comments: CommentDto[],
    topics: TopicDto[]
}

type UserSettings = {
    username: string,
    profilePicture: string,
    showTopics: boolean,
    showComments: boolean,
}

export const UserProfile: React.FC = () => {

    const { userId } = useParams();
    const fileInputRef = useRef<HTMLInputElement>(null);
    
    const [userDetails, setUserDetails] = useState<UserDetails>();
    const [userProfilePic, setUserProfilePic] = useState<File>();
    const [previewUrl, setPreviewUrl] = useState<string>();
    const [isInEditMode, setIsInEditMode] = useState<boolean>(false);
    const [userSettings, setUserSettings] = useState<UserSettings>();
    
    const navigate = useNavigate();

    const uploadFile = (e: ChangeEvent<HTMLInputElement>) => {
        const selectedFiles = e.target.files;
        if (selectedFiles && selectedFiles.length > 0) {
            setUserProfilePic(selectedFiles[0]);
            setPreviewUrl(URL.createObjectURL(selectedFiles[0]));
        }
    };

    const handleImageClick = () => fileInputRef.current?.click();

    const submitUserSettings = async () => {

        const formData = new FormData();
        if (userProfilePic) {
            formData.append("File", userProfilePic);
        } 
        formData.append("UserName", "terve");
        formData.append("ShowTopics", "false");
        formData.append("ShowComments", "false");

        const requestUri = "https://localhost:7181/api/user/settings";
        const requestCredentials: RequestCredentials = 'include';
        const requestData = {
            method: "POST",
            credentials: requestCredentials,
            body: formData            
        };

        var response = await fetch(requestUri, requestData);
    };

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
                setUserDetails(await response.json());
            }
            else{
                navigate("/login");
            }
        }
 
        const getUserSettings = async () => {
            const requestUri = `https://localhost:7181/api/user/settings/${userId}`;
            const requestCredentials: RequestCredentials = 'include';
            const requestData = {
                method: "GET",
                credentials: requestCredentials       
            };
    
            var response = await fetch(requestUri, requestData);

            if (response.ok) {
                setUserSettings(await response.json());
            }
        }

        getUserDetails();
        getUserSettings();
    }, []);

    const getUserTopicActity = () => {
        if (userDetails) {
            // Sort by timestamp
            return [...userDetails.topics].sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime());
        }
    }

    const getUserCommentActity = () => {
        if (userDetails) {
            // Sort by timestamp
            return [...userDetails.comments].sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime());
        }
    }

    const updateUserSettings = (property:string, value:any) => {

        setUserSettings({
            ...(userSettings as UserSettings),
            [property]: value
        });
    }

    const tabsContent = [
        {label: "Topics", content: getUserTopicActity()?.map((item, index) => (<p key={index}>{item.content}</p> ))}, 
        {label: "Comments", content: getUserCommentActity()?.map((item, index) => (<p key={index}>{item.content}</p> ))}
    ];
 
    return isInEditMode === false ? (
        <div className="container">
            <div id="details-row" className="row">        
                <div className="col-4">
                    <img src={`https://localhost:7181/users/${userId}/avatar.png`} alt="Profile picture"/>
                </div>
                <div className="col-6">
                    <h4>{userDetails?.user?.displayName}</h4>
                    <p>{userDetails?.user?.joinedDate ? "Joined on " + FormateDateToTopicTimestamp(new Date(userDetails.user.joinedDate)) : "N/A"}</p>
                </div>
                <div className="col-2">
                    <button onClick={() => setIsInEditMode(true)}>Enter Edit Mode</button>
                </div>
            </div>
            <div id="activity-row" className="row">
                <Tabs tabs={tabsContent}/>
            </div>
        </div>
    ) : (
        <div className="container">
            <div id="details-row" className="row">        
                <div className="col-4 file-upload" onClick={handleImageClick}>
                    <div id="file-upload-element">
                        <img src={previewUrl} />
                        <p>
                            Click to upload new profile picture
                        </p>
                    </div>
                    <input type="file" onChange={uploadFile} ref={fileInputRef} />
                </div>
                <div className="col-5">
                    <h4>{userDetails?.user?.displayName}</h4>
                    <p>{userDetails?.user?.joinedDate ? "Joined on " + FormateDateToTopicTimestamp(new Date(userDetails.user.joinedDate)) : "N/A"}</p>
                </div>
                <div className="col-3">
                    <button onClick={() => submitUserSettings()}>Save changes</button>
                    <button onClick={() => setIsInEditMode(false)}>Exit Edit Mode</button>
                </div>
            </div>
            <div id="activity-row" className="row">
                <h4>Activity options</h4>
                <ToggleSwitch label="Show topics" isOn={userSettings?.showTopics ?? false} onToggle={() => updateUserSettings("showTopics", !userSettings?.showTopics)} />
                <ToggleSwitch label="Show comments" isOn={userSettings?.showComments ?? false} onToggle={() => updateUserSettings("showComments", !userSettings?.showComments)} />
            </div>
        </div>
    );
};
