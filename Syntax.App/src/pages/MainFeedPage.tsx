import { sendRequest } from '../helpers/apiRequestHelpers';
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from 'react';

type TopicDto = {

    title: string,
    body: string
};

const MainFeedPage: React.FC = () => {

    const navigate = useNavigate();
    const [topics, setTopics] = useState<TopicDto[]>();
    useEffect(() => {
        
        const getTopics = async () => {
            const response = await sendRequest({method: "GET", endpoint: "topic"});

            if(response.status === 200){
                const responseData: TopicDto[] = await response.json();
                setTopics(responseData);
            }
            else if(response.status === 401){
                navigate("/login");
            }
        };

        getTopics();
    }, []);

    return(
        <div>
            <div>
                {topics?.map((value, index) => 
                    <div>
                        <h1>{value.title}</h1>                        
                        <h1>{value.body}</h1>
                    </div>
                )}
            </div>
        </div>);
}

export default MainFeedPage;