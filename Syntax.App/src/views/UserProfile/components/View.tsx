import Tabs from "components/general/tabs";
import { CommentDto, TopicDto, UserInformationDto} from "dtos/Dtos";
import { Dispatch, SetStateAction } from "react";
import { FormateDateToTopicTimestamp } from "utils/dateFormatter";

export const ProfileViewComponent: React.FC<{
    userInfo:UserInformationDto, 
    comments: CommentDto[],
    topics: TopicDto[]
    toggleFunction:Dispatch<SetStateAction<boolean>>}> = (props) => {
        
    const getUserTopicActity = () => {
        return props.topics.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime());
    }

    const getUserCommentActity = () => {
        return props.comments.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime());
    }

    const tabsContent = [
        {label: "Topics", content: getUserTopicActity()?.map((item, index) => (<p key={index}>{item.content}</p> ))}, 
        {label: "Comments", content: getUserCommentActity()?.map((item, index) => (<p key={index}>{item.content}</p> ))}
    ];

    return(<>
        <div className="container">
            <div id="details-row" className="row">        
                <div className="col-3">
                    <img src={props.userInfo.profilePicture != null 
                        ? `https://localhost:7181/users/${userId}/${userSettings.profilePicture}` 
                        : "https://localhost:7181/default-avatar.png"} 
                        alt="Profile picture"/>
                </div>
                <div className="col-6">
                    <h4>{props.userInfo.displayName}</h4>
                    <p>{props.userInfo.joinedDate ? "Joined on " + FormateDateToTopicTimestamp(new Date(props.userInfo.joinedDate)) : "N/A"}</p>
                </div>
                <div className="col-2">
                    <button onClick={() => props.toggleFunction(true)}>Enter Edit Mode</button>
                </div>
            </div>
            <div id="activity-row" className="row">
                <Tabs tabs={tabsContent}/>
            </div>
        </div>
    </>)
};