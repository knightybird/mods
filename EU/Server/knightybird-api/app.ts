import express, { Request, Response } from 'express';
import bodyParser from 'body-parser';
import cors from 'cors';
import { v4 as uuidv4 } from 'uuid';
import fs from "fs";

import Data, { Card } from './data';

const app = express();

app.use(cors({
    origin: '*',
    methods: ['GET', 'POST', 'PUT', 'DELETE'],
    allowedHeaders: ['Content-Type', 'Authorization']
}));

app.use(bodyParser.json());

function readData(): Promise<Data> {
    return new Promise((resolve, reject) => {
        fs.readFile('data.json', (err, data) => {
            if (err) {
                reject(err);
            } else {
                resolve(JSON.parse(data.toString()));
            }
        });
    });
}

function writeData(data: Data): Promise<void> {
    return new Promise((resolve, reject) => {
        fs.writeFile('data.json', JSON.stringify(data), (err) => {
            if (err) {
                reject(err);
            } else {
                resolve();
            }
        });
    });
}
// Endpoint to get data
app.get('/api', async (req: Request, res: Response) => {
    try {
        const data = await readData();
        res.json(data);
    } catch (err) {
        console.error(err);
        res.status(500).send({ message: 'Error reading data' });
    }
});

// Endpoint to update data
app.post('/api', async (req: Request, res: Response) => {
    try {
        const data = await readData();
        const itemIndex = data.items.findIndex((item: Card) => item.id === req.body.id);
        if (itemIndex!== -1) {
            Object.assign(data.items[itemIndex], req.body);
        } else {
            data.items.push(req.body);
        }
        await writeData(data);
        res.send({ message: 'Data updated successfully' });
    } catch (err) {
        console.error(err);
        res.status(500).send({ message: 'Error writing data' });
    }
});

// Endpoint to add card
app.post('/api/add-card', async (req: Request, res: Response) => {
    try {
        const data = await readData();
        const lastRow = Math.max(...data.items.filter((item: Card) => item.col === 0).map((item: Card) => item.row));
        const newCardData = {
            id: uuidv4(),
            name: req.body.name,
            startTime: null,
            isHuntReady: false,
            col: req.body.col,
            row: lastRow + 1,
        };
        data.items.push(newCardData);
        await writeData(data);
        res.send({ message: 'Card added successfully' });
    } catch (err) {
        console.error(err);
        res.status(500).send({ message: 'Error writing data' });
    }
});

// Endpoint to remove card
app.post('/api/remove-card', async (req: Request, res: Response) => {
    try {
        const data = await readData();
        const cardIndexToRemove = data.items.findIndex((item: Card) => item.id === req.body.id);
        if (cardIndexToRemove!== -1) {
            data.items.splice(cardIndexToRemove, 1);
            await writeData(data);
            res.send({ message: 'Card removed successfully' });
        } else {
            res.status(404).send({ message: 'Card not found' });
        }
    } catch (err) {
        console.error(err);
        res.status(500).send({ message: 'Error writing data' });
    }
});

// Endpoint to edit card
app.post('/api/edit-card', async (req: Request, res: Response) => {
    try {
        const data = await readData();
        const cardIndexToEdit = data.items.findIndex((item: Card) => item.id === req.body.id);
        if (cardIndexToEdit!== -1) {
            data.items[cardIndexToEdit].name = req.body.name;
            await writeData(data);
            res.send({ message: 'Card edited successfully' });
        } else {
            res.status(404).send({ message: 'Card not found' });
        }
    } catch (err) {
        console.error(err);
        res.status(500).send({ message: 'Error writing data' });
    }
});

// Endpoint to update order
app.post('/api/update-order', async (req: Request, res: Response) => {
    try {
        const data = await readData();
        req.body.forEach((item: Card) => {
            const existingItem = data.items.find((i: Card) => i.id === item.id);
            if (existingItem) {
                existingItem.col = item.col;
                existingItem.row = item.row;
            }
        });
        await writeData(data);
        res.send({ message: 'Order updated successfully' });
    } catch (err) {
        console.error(err);
        res.status(500).send({ message: 'Error writing data' });
    }
});

app.listen(8000, () => {
    console.log('Server listening on port 8000');
});