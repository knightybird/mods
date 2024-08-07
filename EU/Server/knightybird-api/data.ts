export interface Card {
    id: string;
    name: string;
    startTime: number | null;
    isHuntReady: boolean;
    col: number;
    row: number;
}

interface Data {
    items: Card[];
}

export default Data;
