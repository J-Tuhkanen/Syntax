import { ChangeEvent, FormEvent, useEffect, useRef, useState } from "react";
import { useParams } from "react-router-dom";
import "./UserProfile.scss";
import { sendHttpRequest } from "utils/httpRequest";
import { CommentDto, TopicDto, UserDto } from "dtos/Dtos";
import { FormateDateToTopicTimestamp } from "utils/dateFormatter";
import { type } from "os";

type UserDetails = {
    user: UserDto,
    comments: CommentDto[],
    topics: TopicDto[]
}

export const UserProfile: React.FC = () => {

    const { userId } = useParams();
    const [userDetails, setUserDetails] = useState<UserDetails>();
    // const [userProfilePic, setUserProfilePic] = useState<File>();
    // const [previewUrl, setPreviewUrl] = useState<string>();
    // const fileInputRef = useRef<HTMLInputElement>(null);
    const [profilePicture, setProfilePicture] = useState<string>();

    // const uploadFile = (e: ChangeEvent<HTMLInputElement>) => {
    //     const selectedFiles = e.target.files;
    //     if (selectedFiles && selectedFiles.length > 0) {
    //         setUserProfilePic(selectedFiles[0]);
    //         setPreviewUrl(URL.createObjectURL(selectedFiles[0]));
    //     }
    // };

    // const handleImageClick = () => fileInputRef.current?.click();

    // const submitUserSettings = async (event: FormEvent<HTMLFormElement>) => {
    //     event.preventDefault();
    //     const formData = new FormData();
    //     if (userProfilePic) {
    //         formData.append("File", userProfilePic);
    //     } else {
    //         console.error("No file selected");
    //         return;
    //     }
    
    //     const requestUri = "https://localhost:7181/api/profilesettings";
    //     const requestCredentials: RequestCredentials = 'include';
    //     const requestData = {
    //         method: "POST",
    //         credentials: requestCredentials,
    //         body: formData            
    //     };

    //     var response = await fetch(requestUri, requestData);
    // };

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

                    // console.log(url);
                } else {
                    console.error('Failed to fetch image');
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
 
    return (
        <>
        <div className="row col-md-12">
            <div className="row col-md-4">
                <img src={profilePicture}/>
            </div>
            <div className="row col-md-8">
                <h4>{userDetails?.user?.displayName}</h4>
                <p>Joined on {userDetails?.user?.joinedDate != null ? FormateDateToTopicTimestamp(new Date(userDetails.user.joinedDate)) : null}</p>
            </div>
        </div>
        <div>
            <h4>Recent activity:</h4>
            {getUserActivity()?.map((item, index) => {
                return(<p key={index}>{item.content}</p>)
            })}
        </div>
        </>
        // <form style={{ margin: "auto" }} onSubmit={submitUserSettings}>
        //     <div className="row">
        //         <div className="file-upload col-md-1" onClick={handleImageClick} style={{ cursor: 'pointer', margin: 'auto' }}>
        //             <input
        //                 type="file"
        //                 onChange={uploadFile}
        //                 ref={fileInputRef}
        //                 style={{ display: 'none' }} />
        //             <img src={previewUrl} alt="Upload" />
        //         </div>
        //         <div className="col-md-11">
        //             {userId}

        //             <img src={loadedImageUrl}/>
        //         </div>
        //     </div>
        //     <input type="submit" value="Submit" />
        // </form>
    );
};
