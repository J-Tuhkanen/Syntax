import ToggleSwitch from "components/general/switch";
import { UserInformationDto, UserSettingsDto as UserSettingsDto } from "dtos/Dtos";
import { ChangeEvent, Dispatch, SetStateAction, useRef, useState } from "react";
import { FaPen } from "react-icons/fa";
import { FormateDateToTopicTimestamp } from "utils/dateFormatter";

export const ProfileEditComponent: React.FC<{
        userInfo:UserInformationDto, 
        toggleFunction:Dispatch<SetStateAction<boolean>>
    }> = (props) => {
    
    const fileInputRef = useRef<HTMLInputElement>(null);
    const [userProfilePic, setUserProfilePic] = useState<File>();
    const [previewUrl, setPreviewUrl] = useState<string>();
    const [userSettings, setUserSettings] = useState<UserSettingsDto>(props.userInfo.settings);

    const uploadForPreviewFile = (e: ChangeEvent<HTMLInputElement>) => {
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
        formData.append("DisplayName", userSettings.displayName);
        formData.append("ShowTopics",  userSettings.showTopics.toString());
        formData.append("ShowComments", userSettings.showComments.toString());

        const requestUri = "https://localhost:7181/api/user/settings";
        const requestCredentials: RequestCredentials = 'include';
        const requestData = {
            method: "POST",
            credentials: requestCredentials,
            body: formData            
        };

        var response = await fetch(requestUri, requestData);

        if (response.status === 200) {
            props.toggleFunction(false);
        }
    };

    const updateUserSettings = (property:string, value:any) => {

        setUserSettings({
            ...(userSettings as UserSettingsDto),
            [property]: value
        });
    }

    return(<>
        <div className="container">
            <div id="details-row" className="row">        
                <div className="col-3 file-upload" onClick={handleImageClick}>
                    <div id="file-upload-element">
                        <img src={previewUrl} />
                        <p>Click to upload new profile picture</p>
                    </div>
                    <input type="file" onChange={uploadForPreviewFile} ref={fileInputRef} />
                </div>
                <div className="col-6">
                    <input value={userSettings.displayName} onChange={(e : ChangeEvent<HTMLInputElement>) => {                            
                        updateUserSettings("username", e.target.value); 
                        e.target.focus();
                    }}/>
                    <FaPen/>
                    <p>{props.userInfo.joinedDate ? `Joined on ${FormateDateToTopicTimestamp(new Date(props.userInfo.joinedDate))}` : "N/A"}</p>
                </div>
                <div className="col-3">
                    <button onClick={() => submitUserSettings()}>Save changes</button>
                    <button onClick={() => props.toggleFunction(false)}>Exit Edit Mode</button>
                </div>
            </div>
            <div id="activity-row" className="row">
                <h4>Activity options</h4>
                <ToggleSwitch label="Show topics" isOn={userSettings.showTopics ?? false} onToggle={() => updateUserSettings("showTopics", !userSettings.showTopics)} />
                <ToggleSwitch label="Show comments" isOn={userSettings.showComments ?? false} onToggle={() => updateUserSettings("showComments", !userSettings.showComments)} />
            </div>
        </div>
    </>);
};