import React, { ChangeEvent, FormEvent, useEffect, useState } from 'react'
import { sendHttpRequest } from 'utils/httpRequest'
import { CommentDto } from 'dtos/CommentDto'

type CommentAreaProps = {
    TopicId: string
}

export const CommentArea: React.FC<CommentAreaProps> = (props) => {
    
    const [commentSectionComments, setCommentSectionComments] = useState<CommentDto[]>();
    const [userComment, setUserComment] = useState<string>();
    
    useEffect(() => {

        const getComments = async() => {
            const response = await sendHttpRequest({method: "GET", endpoint: `comment/${props.TopicId}`});
        
            console.log(response);

            if(response.status === 200){

                var data = await response.json();
                console.log(data);
                setCommentSectionComments(data);
            }
        };

        getComments();

    }, []);
  
    const onSubmitCommentInput = async (event: FormEvent<HTMLInputElement>) => {
        
        console.log(event);
    }

    return (
        <>
            <input type="text" 
                value={userComment} 
                onChange={(e: ChangeEvent<HTMLInputElement>) => setUserComment(e.target.value)}
                onSubmit={onSubmitCommentInput}/>

            {commentSectionComments?.map((c, index) => 
                
                <div key={index} className='comment-container'>
                    
                    <p>{c.content}</p>
                    <p>{c.username}</p>
                </div>
            )}
        </>
    )
}
