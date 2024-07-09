import { UUID } from "crypto";

export type CommentDto = {
    topicId: UUID,
    content: string;
    username: string;
};
