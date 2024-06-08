import { sendRequest } from '../helpers/apiRequestHelpers';
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from 'react';
import { Topic } from '../components/topic-component/Topic';
import { TopicDto } from '../dtos/TopicDto';

const MainFeedPage: React.FC = () => {

    const navigate = useNavigate();
    const [topics, setTopics] = useState<TopicDto[]>();
    useEffect(() => {
        
        const getTopics = async () => {
            const response = await sendRequest({method: "GET", endpoint: "topic"});

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
        <div>
            <div className="row justify-content-center">
                <div className='col-6'>

                {topics?.map((value, index) => 
                    <Topic 
                    key={index}
                    id={value.id} 
                    content={value.body} 
                    title={value.title}/>
                    )}
                </div>
            </div>
        </div>);
}

export default MainFeedPage;