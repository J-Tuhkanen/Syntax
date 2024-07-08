import "./FeedTopic.scss";
import { useNavigate } from 'react-router-dom';
import { FormateDateToTopicTimestamp } from "utils/dateFormatter";
import { TopicDto } from "dtos/TopicDto";

export const FeedTopic: React.FC<TopicDto> = (props) => {

    const navigate = useNavigate();

    const onClickTopic = () => {
        navigate(`topic/${props.id}`);
    };

    function openCreatorProfile(event:React.MouseEvent<HTMLElement>): void {
        event.stopPropagation();
        navigate(`/user/${props.userId}`)
    }

    return (
    <>
        <div className="topic-element" style={{ cursor: 'pointer' }} onClick={onClickTopic}>
            <div className="feed-topic-header row">
                <p className="col-8" >{FormateDateToTopicTimestamp(new Date(props.timestamp))}</p>
                <p className="col-4">By <a onClick={openCreatorProfile}>{props.username}</a></p>
            </div>
            <hr className="solid"></hr>
            <div className="feed-topic-body">
                <h3>{props.title}</h3>
                <p>{props.content}</p>
            </div>
        </div>
    </>);
} 