import { UUID } from "crypto";

export type TopicDto = {
    userId: `${string}-${string}-${string}-${string}-${string}`;
    id: UUID;
    title: string;
    content: string;
    timestamp: Date,
    username: string
};


export type CommentDto = {
    topicId: UUID,
    content: string;
    username: string;
};

export type UserDto = {
    DisplayName: string,
    Email: string,
    JoinedDate: Date
}