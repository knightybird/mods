const express = require('express');
const app = express();
const fs = require('fs');
const bodyParser = require('body-parser');
const cors = require('cors');

app.use(cors({
    origin: '*',
    methods: ['GET', 'POST', 'PUT', 'DELETE'],
    allowedHeaders: ['Content-Type', 'Authorization']
}));

app.use(bodyParser.json());

app.get('/api', (req, res) => {
    fs.readFile('data.json', (err, data) => {
        if (err) {
            console.error(err);
            res.status(500).send({ message: 'Error reading data' });
        } else {
            res.json(JSON.parse(data));
        }
    });
});

app.post('/api', (req, res) => {
    fs.readFile('data.json', (err, data) => {
        if (err) {
            console.error(err);
            res.status(500).send({ message: 'Error reading data' });
        } else {
            const jsonData = JSON.parse(data);
            const itemIndex = jsonData.items.findIndex(item => item.id === req.body.id);
            if (itemIndex!== -1) {
                jsonData.items[itemIndex] = req.body;
            } else {
                jsonData.items.push(req.body);
            }
            fs.writeFile('data.json', JSON.stringify(jsonData), (err) => {
                if (err) {
                    console.error(err);
                    res.status(500).send({ message: 'Error writing data' });
                } else {
                    res.send({ message: 'Data updated successfully' });
                }
            });
        }
    });
});

app.listen(8000, () => {
    console.log('Server listening on port 8000');
});
