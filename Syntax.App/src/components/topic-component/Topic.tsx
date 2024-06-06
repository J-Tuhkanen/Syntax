import "./Topic.scss";
import { useNavigate } from 'react-router-dom';
import { UUID } from "crypto";

type TopicProps = {
    id: UUID,
    title: string,
    content: string
}

export const Topic: React.FC<TopicProps> = (props) => {

    const navigate = useNavigate();
    const onClickTopic = () => {
        navigate(`Topic/${props.id}`);
    };

    return (
    <>
        {/* TODO: scss */}
        <div className="topic-element" onClick={onClickTopic} style={{ cursor: 'pointer' }}>
            <h3>{props.title}</h3>
            <p>{props.content}</p>
        </div>
    </>);
} 