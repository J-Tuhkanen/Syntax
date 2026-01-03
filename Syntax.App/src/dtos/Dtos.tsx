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
    timestamp: Date,
};

export type UserInformationDto = {
    email: string,
    joinedDate: Date
    activity: {        
        comments: CommentDto[],
        topics: TopicDto[],
    }
    settings: UserSettingsDto
}

export type UserSettingsDto = {
    displayName: string,
    profilePicture: string,
    showTopics: boolean,
    showComments: boolean,
}
    