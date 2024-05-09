import "./Topic.scss";

type TopicProps = {

    title: string,
    content: string
}

export const Topic: React.FC<TopicProps> = (props) => {

    return (
    <>
        <div className="topic-element">
            <h3>{props.title}</h3>
            <p>{props.content}</p>
        </div>
    </>);
} 