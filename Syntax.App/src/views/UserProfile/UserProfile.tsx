import { ChangeEvent, FormEvent, useEffect, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { CommentDto, TopicDto, UserDto } from "dtos/Dtos";
import { FormateDateToTopicTimestamp } from "utils/dateFormatter";
import "./UserProfile.scss";

type UserDetails = {
    user: UserDto,
    comments: CommentDto[],
    topics: TopicDto[]
}

export const UserProfile: React.FC = () => {

    const { userId } = useParams();
    const [profilePicture, setProfilePicture] = useState<string>();
    const [userDetails, setUserDetails] = useState<UserDetails>();
    const [isInEditMode, setIsInEditMode] = useState<boolean>(true);
    const [userProfilePic, setUserProfilePic] = useState<File>();
    const [previewUrl, setPreviewUrl] = useState<string>();
    const fileInputRef = useRef<HTMLInputElement>(null);
    const navigate = useNavigate();

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
    
        const requestUri = "https://localhost:7181/api/profilesettings";
        const requestCredentials: RequestCredentials = 'include';
        const requestData = {
            method: "POST",
            credentials: requestCredentials,
            body: formData            
        };

        var response = await fetch(requestUri, requestData);
    };

    useEffect(() => {
        
        const getUserProfilePicture = async () => {
            try {
                const requestUri = `https://localhost:7181/api/user/avatar/${userId}`;
                const requestCredentials: RequestCredentials = 'include';
                const requestData = {
                    method: "GET",
                    credentials: requestCredentials       
                };
        
                var response = await fetch(requestUri, requestData);
                
                if (response.ok) {
                    const blob = await response.blob();
                    const url = URL.createObjectURL(blob);
                    setProfilePicture(url);
                } 
                else {
                    navigate("/login");
                }
            } catch (error) {
                console.error('Error fetching image:', error);
            }
        };

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
 
        getUserDetails();
        getUserProfilePicture();
    }, []);

    const getUserActivity = () => {
        if (userDetails) {
            // Sort by timestamp
            return [...userDetails.topics, ...userDetails.comments].sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime());
        }
    };
 
    return isInEditMode ? (
        <div className="container">        
            <button onClick={() => setIsInEditMode(false)}>Exit Edit Mode</button>
            <div className="row col-md-12">
                <div className="row col-md-4">
                    <img src={profilePicture} alt="Profile" />
                </div>
                <div className="row col-md-8">
                    <h4>{userDetails?.user?.displayName}</h4>
                    <p>
                        Joined on{" "}
                        {userDetails?.user?.joinedDate
                            ? FormateDateToTopicTimestamp(new Date(userDetails.user.joinedDate))
                            : "N/A"}
                    </p>
                </div>
            </div>
            <div>
                <h4>Recent activity:</h4>
                {getUserActivity()?.map((item, index) => (
                    <p key={index}>{item.content}</p>
                ))}
            </div>
        </div>
    ) : (
        <div className="container">
            <form style={{ margin: "auto" }} onSubmit={submitUserSettings}>
                <button onClick={() => setIsInEditMode(true)}>Enter Edit Mode</button>
                <div className="row">
                    <div
                        className="file-upload col-md-1"
                        onClick={handleImageClick}
                        style={{ cursor: "pointer", margin: "auto" }}
                        >
                        <input
                            type="file"
                            onChange={uploadFile}
                            ref={fileInputRef}
                            style={{ display: "none" }}
                            />
                        <img src={previewUrl} alt="Upload Preview" />
                    </div>
                </div>
                <input type="submit" value="Submit" />
            </form>
        </div>
    );
};
