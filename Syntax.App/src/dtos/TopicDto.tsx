import { UUID } from 'crypto';

export type TopicDto = {
    id: UUID;
    title: string;
    body: string;
};
