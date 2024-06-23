import { TopicDto } from '../../dtos/TopicDto'

export const ViewTopic: React.FC<TopicDto>  = (props) => {
  return (
    <>
      <h1>{props.title}</h1>
      <p>{props.body}</p>
    </>
  )
}
