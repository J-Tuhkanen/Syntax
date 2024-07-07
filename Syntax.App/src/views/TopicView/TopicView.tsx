import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { sendHttpRequest } from "utils/httpRequest";
import { TopicDto } from "dtos/TopicDto";
import { ViewTopic } from "components/view-topic-component/ViewTopic";
import './TopicView.scss';

const TopicViewView : React.FC = () => {

    const { topicId } = useParams();
    const [topic, setTopic] = useState<TopicDto>();
    const [isFetching, setIsFetching] = useState<Boolean>(true);
    const navigate = useNavigate();

    useEffect(() =>  {

        const getTopicData = async () => {
            
            const response = await sendHttpRequest({method: "GET", endpoint: `topic/${topicId}`});
            
            if(response.status === 401){
                navigate("/login");
            }

            setIsFetching(false);
            const topic = await response.json();
            
            if(topic){

                setTopic(topic);
            }
        };

        getTopicData();
    }, []);


    return(topic ? 
        <div className="view-topic-container">
            <ViewTopic id={topic.id} title={topic.title} content={topic.content} timestamp={topic.timestamp} username={topic.username} userId={topic.userId}/>
        </div> : isFetching ? null : <p>Post not found...</p>);
}

export default TopicViewView;