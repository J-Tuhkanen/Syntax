import { HubConnection, HubConnectionBuilder, HubConnectionState } from "@microsoft/signalr";
import { CommentDto } from "dtos/CommentDto";

export class SyntaxCommentClient {

    connection: HubConnection;

    constructor(endpoint:string){

        this.connection = new HubConnectionBuilder().withUrl(endpoint).build();
    }

    addHandler = (methodName: string, callback: Function) => {

        this.connection.on(methodName, (comment:CommentDto) => callback(comment))
    }

    connectionState = (): HubConnectionState => this.connection.state;

    connect = async (): Promise<void> => {

        await this.connection.start();
    }
}