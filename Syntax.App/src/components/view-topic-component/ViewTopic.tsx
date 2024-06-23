import { UUID } from 'crypto'

type TopicProps = {
    id: UUID,
    title: string,
    content: string
}

export const ViewTopic: React.FC<TopicProps>  = (props) => {
  return (
    <></>
    // <div>VIewTopic</div>
  )
}
