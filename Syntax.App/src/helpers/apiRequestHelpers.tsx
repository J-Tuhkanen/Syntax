const API_BASE_URI: string = "https://localhost:7181/api/";

type RequestArguments = {

    method: string,
    endpoint: string,
    requestBody?: object
};

export const sendRequest = async(requestArgs: RequestArguments):  Promise<Response> => {

    const requestUri = `${API_BASE_URI}${requestArgs.endpoint}`;
    const requestCredentials: RequestCredentials = 'include';
    const requestData = {
        method: requestArgs.method,
        credentials: requestCredentials,
        headers: { 'Content-Type': 'application/json' },
        body: requestArgs.requestBody ? JSON.stringify(requestArgs.requestBody) : null
    };

    return await fetch(requestUri, requestData);
};