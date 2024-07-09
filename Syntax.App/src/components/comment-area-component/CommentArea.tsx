import React, { ChangeEvent, FormEvent, useEffect, useState } from 'react'
import { sendHttpRequest } from 'utils/httpRequest'
import { CommentDto } from 'dtos/CommentDto'
import { SyntaxCommentClient } from 'services/NotificationService'
import { HubConnectionState } from '@microsoft/signalr'

type CommentAreaProps = {
    TopicId: string
}

export const CommentArea: React.FC<CommentAreaProps> = (props) => {
    
    const [commentSectionComments, setCommentSectionComments] = useState<CommentDto[]>([]);
    const [userComment, setUserComment] = useState<string>();
    const commentClient = new SyntaxCommentClient(`https://localhost:7181/notification/${props.TopicId}`);

    const connectToTopicCommentHub = async () => {
        
        if (commentClient.connectionState() !== HubConnectionState.Disconnected) {
            return;
        }

        commentClient.addHandler("messageReceived", (comment: CommentDto) => {

            setCommentSectionComments((prev) => {

                if(!prev){
                    return prev;
                }

                return [...prev, comment];
            })
        });

        await commentClient.connect().catch((err) => alert(err));
    }

    const SubmitComment = async (event: FormEvent<HTMLFormElement>) => {
        
        event.preventDefault();
        
        const response = await sendHttpRequest({endpoint: "comment", method: "POST", requestBody: {
            content: userComment,
            topicId: props.TopicId 
        }});

        if (response.status === 200){
            setUserComment("");
        }
    }
    
    useEffect(() => {

        const getComments = async() => {
            const response = await sendHttpRequest({method: "GET", endpoint: `comment/${props.TopicId}`});
        
            if(response.status === 200){

                var data = await response.json();
                console.log(data);
                setCommentSectionComments(data);
            }
        };

        getComments();
        connectToTopicCommentHub();
    }, []);

    return (
        <>
            <form onSubmit={SubmitComment}>
                <div className='row col-md-12'>                    
                    <textarea value={userComment} onChange={(e: ChangeEvent<HTMLTextAreaElement>) => setUserComment(e.target.value)}/>
                </div>
                <div className='row col-md-12'>
                    <div className='col-md-11'>
                        {/* Reserved for editing stuff */}
                    </div>
                    <button type="submit" className='col-md-1'>Jutksa</button>
                </div>
            </form>

            {commentSectionComments?.map((c, index) => 
                
                <div key={index} className='comment-container'>
                    
                    <p>{c.content}</p>
                    <p>{c.username}</p>
                </div>
            )}
        </>
    )
}
