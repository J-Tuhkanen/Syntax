import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { sendRequest } from "../helpers/apiRequestHelpers";
import { TopicDto } from "../dtos/TopicDto";

const TopicViewPage : React.FC = () => {
    const { topicId } = useParams();
    const [topic, setTopic] = useState<TopicDto>();

    useEffect(() =>  {

        const getTopicData = async () => {
            
            const response = await sendRequest({method: "GET", endpoint: `topic/${topicId}`});
            setTopic(await response.json());
        };

        getTopicData();
    }, []);


    return(topic ? <>
        <h1>{topic.title}</h1>
        <p>{topic.body}</p>
    </> : null);
}

export default TopicViewPage;