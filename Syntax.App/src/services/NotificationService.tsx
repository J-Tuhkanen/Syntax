import { HubConnection, HubConnectionBuilder, HubConnectionState } from "@microsoft/signalr";
import { CommentDto } from "dtos/Dtos";

export class SyntaxCommentClient {

    private connection: HubConnection;

    constructor(endpoint:string){

        this.connection = new HubConnectionBuilder().withUrl(endpoint).build();
    }

    public addHandler = (methodName: string, callback: Function) => {

        this.connection.on(methodName, (comment:CommentDto) => callback(comment))
    }

    public connectionState = (): HubConnectionState => { 
        return this.connection.state;
    }

    public connect = async (): Promise<void> => {

        await this.connection.start();
    }
}