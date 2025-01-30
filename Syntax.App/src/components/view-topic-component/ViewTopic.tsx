import { TopicDto } from 'dtos/Dtos'
import { CommentArea } from 'components/comment-area-component/CommentArea'

export const ViewTopic: React.FC<TopicDto>  = (props) => {
  return (
    <>
      <h1>{props.title}</h1>
      <p>{props.content}</p>
      <CommentArea TopicId={props.id}/>
    </>
  )
}
