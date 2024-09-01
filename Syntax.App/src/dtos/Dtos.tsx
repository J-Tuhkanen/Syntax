import { UUID } from "crypto";

export type TopicDto = {
    userId: UUID,
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
    displayName: string,
    email: string,
    joinedDate: Date
}