import { UUID } from 'crypto';

export type TopicDto = {
    userId: `${string}-${string}-${string}-${string}-${string}`;
    id: UUID;
    title: string;
    content: string;
    timestamp: Date,
    username: string
};
