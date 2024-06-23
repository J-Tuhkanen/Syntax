import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { sendRequest } from "../helpers/apiRequestHelpers";
import { TopicDto } from "../dtos/TopicDto";

const TopicViewPage : React.FC = () => {

    const { topicId } = useParams();
    const [topic, setTopic] = useState<TopicDto>();
    const [isFetching, setIsFetching] = useState<Boolean>(true);
    const navigate = useNavigate();

    useEffect(() =>  {

        const getTopicData = async () => {
            
            const response = await sendRequest({method: "GET", endpoint: `topic/${topicId}`});
            
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


    return(topic ? <>
        <h1>{topic.title}</h1>
        <p>{topic.body}</p>
    </> : isFetching ? null : 
        <p>Post not found...</p>);
}

export default TopicViewPage;