import { ChangeEvent, MouseEvent, useRef, useState } from "react";
import { useParams } from "react-router-dom";
import "./UserProfile.scss";

export const UserProfile : React.FC = () => {

    const { userId } = useParams();
    const [userProfilePic, setUserProfilePic] = useState<File>();
    const [previewUrl, setPreviewUrl] = useState<string>();
    const fileInputRef = useRef<HTMLInputElement>(null);

    const uploadFile = async (e: ChangeEvent<HTMLInputElement>) => {
        
        const { files }:any = e.target;
        const selectedFiles = files as FileList;
        setUserProfilePic(selectedFiles[0]);

        setPreviewUrl(URL.createObjectURL(selectedFiles[0]));
    };

    const handleImageClick = () => fileInputRef.current?.click();

    return <>
        <div className="file-upload" onClick={handleImageClick} style={{ cursor: 'pointer' }}>
            <input
                type="file"
                onChange={uploadFile}
                ref={fileInputRef}
                style={{ display: 'none' }}
            />
            <img src={previewUrl} alt="Upload" style={{ width: '200px', height: '200px' }} />
        </div>
    </>
};