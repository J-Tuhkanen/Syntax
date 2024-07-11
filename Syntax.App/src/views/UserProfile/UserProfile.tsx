import { ChangeEvent, FormEvent, useRef, useState } from "react";
import { useParams } from "react-router-dom";
import "./UserProfile.scss";
import { sendHttpRequest } from "utils/httpRequest";

export const UserProfile: React.FC = () => {

    const { userId } = useParams();
    const [userProfilePic, setUserProfilePic] = useState<File>();
    const [previewUrl, setPreviewUrl] = useState<string>();
    const fileInputRef = useRef<HTMLInputElement>(null);

    const uploadFile = (e: ChangeEvent<HTMLInputElement>) => {
        const selectedFiles = e.target.files;
        if (selectedFiles && selectedFiles.length > 0) {
            setUserProfilePic(selectedFiles[0]);
            setPreviewUrl(URL.createObjectURL(selectedFiles[0]));
        }
    };

    const handleImageClick = () => fileInputRef.current?.click();

    const submitUserSettings = async (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const formData = new FormData();
        if (userProfilePic) {
            formData.append("File", userProfilePic);
        } else {
            console.error("No file selected");
            return;
        }
        // await sendHttpRequest({ endpoint: "profilesettings", method: "POST", requestBody: formData });
    
        const requestUri = "https://localhost:7181/api/profilesettings";
        const requestCredentials: RequestCredentials = 'include';
        const requestData = {
            method: "POST",
            credentials: requestCredentials,
            // headers: { 'Content-Type': 'application/json' },
            body: formData,
            
        };

        var response = await fetch(requestUri, requestData);
    };

    return (
        <form style={{ margin: "auto" }} onSubmit={submitUserSettings}>
            <div className="row">
                <div className="file-upload col-md-1" onClick={handleImageClick} style={{ cursor: 'pointer', margin: 'auto' }}>
                    <input
                        type="file"
                        onChange={uploadFile}
                        ref={fileInputRef}
                        style={{ display: 'none' }}
                    />
                    <img src={previewUrl} alt="Upload" />
                </div>
                <div className="col-md-11">
                    {userId}
                </div>
            </div>
            <input type="submit" value="Submit" />
        </form>
    );
};