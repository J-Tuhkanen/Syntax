
const monthStrings = [
    "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
]

/** Converts Date to clear date string (eg. 1st Jan 2019) */
export const DateFormatter = (date:Date) => {

    let dayString:string = date.getDate().toString();
    
    switch (date.getDate()){
        case 1: {
            dayString = dayString + "st";
            break;
        }
        case 2: {
            dayString = dayString + "nd";
            break;
        }
        case 3: {
            dayString = dayString + "rd";
            break;
        }
        default: {
            dayString = dayString + "th";
            break;
        }
    }

    return `${dayString} of ${monthStrings[date.getMonth()]} ${date.getFullYear()}`;
}