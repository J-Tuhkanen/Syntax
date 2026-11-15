import { sendHttpRequest } from 'utils/httpRequest';
import { useNavigate } from "react-router-dom";
import { CSSProperties, useEffect, useState } from 'react';
import { FeedTopic } from 'components/feed-topic-component/FeedTopic';
import { TopicDto } from 'dtos/Dtos';

const MainFeedPage: React.FC = () => {

    const navigate = useNavigate();
    const [topics, setTopics] = useState<TopicDto[]>();
    useEffect(() => {
        
        const getTopics = async () => {
            const response = await sendHttpRequest({method: "GET", endpoint: "topic"});

            if(response.status === 200){                
                setTopics(await response.json());
            }
            else if(response.status === 401){
                navigate("/login");
            }
        };

        getTopics();
    }, []);

    return(
            <>
            {topics?.map((value, index) => 
                <FeedTopic 
                key={index}
                id={value.id} 
                content={value.content} 
                title={value.title}
                timestamp={value.timestamp}
                username={value.username}
                userId={value.userId}/>
                )}
            </>
        );
}

export default MainFeedPage;