export enum PopupType {
    Info = 'info-popup',
    Error = 'error-popup',
}

export const PopupInfo: React.FC<{
    message:string
    type:PopupType    
}> = ({message, type}) => {

    return(<div className={type}>
      <p>{message}</p>  
    </div>);
}