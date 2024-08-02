const express = require('express');
const app = express();
const fs = require('fs');
const bodyParser = require('body-parser');
const cors = require('cors');
const uuid = require('uuid');

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
            res.status(500).send({message: 'Error reading data'});
        } else {
            res.json(JSON.parse(data));
        }
    });
});

app.post('/api', (req, res) => {
    fs.readFile('data.json', (err, data) => {
        if (err) {
            console.error(err);
            res.status(500).send({message: 'Error reading data'});
        } else {
            const jsonData = JSON.parse(data);
            const itemIndex = jsonData.items.findIndex(item => item.id === req.body.id);
            if (itemIndex !== -1) {
                jsonData.items[itemIndex] = {...jsonData.items[itemIndex], ...req.body};
                if (req.body.is_hunt_ready !== undefined) {
                    jsonData.items[itemIndex].is_hunt_ready = req.body.is_hunt_ready;
                }
            } else {
                jsonData.items.push(req.body);
            }
            fs.writeFile('data.json', JSON.stringify(jsonData), (err) => {
                if (err) {
                    console.error(err);
                    res.status(500).send({message: 'Error writing data'});
                } else {
                    res.send({message: 'Data updated successfully'});
                }
            });
        }
    });
});

app.post('/api/add-card', (req, res) => {
    const newCardData = {
        id: uuid.v4(),
        name: req.body.name,
        startTime: null,
        is_hunt_ready: false,
        order: req.body.order
    };

    fs.readFile('data.json', (err, data) => {
        if (err) {
            console.error(err);
            res.status(500).send({message: 'Error reading data'});
        } else {
            const jsonData = JSON.parse(data);
            jsonData.items.push(newCardData);
            fs.writeFile('data.json', JSON.stringify(jsonData), (err) => {
                if (err) {
                    console.error(err);
                    res.status(500).send({message: 'Error writing data'});
                } else {
                    res.send({message: 'Card added successfully'});
                }
            });
        }
    });
});

app.post('/api/remove-card', (req, res) => {
    const cardIdToRemove = req.body.id;
    fs.readFile('data.json', (err, data) => {
        if (err) {
            console.error(err);
            res.status(500).send({message: 'Error reading data'});
        } else {
            const jsonData = JSON.parse(data);
            const cardIndexToRemove = jsonData.items.findIndex(item => item.id === cardIdToRemove);
            if (cardIndexToRemove !== -1) {
                jsonData.items.splice(cardIndexToRemove, 1);
                fs.writeFile('data.json', JSON.stringify(jsonData), (err) => {
                    if (err) {
                        console.error(err);
                        res.status(500).send({message: 'Error writing data'});
                    } else {
                        res.send({message: 'Card removed successfully'});
                    }
                });
            } else {
                res.status(404).send({message: 'Card not found'});
            }
        }
    });
});

app.post('/api/edit-card', (req, res) => {
    const cardIdToEdit = req.body.id;
    const newCardName = req.body.name;
    const newCardOrder = req.body.order;
    fs.readFile('data.json', (err, data) => {
        if (err) {
            console.error(err);
            res.status(500).send({message: 'Error reading data'});
        } else {
            const jsonData = JSON.parse(data);
            const cardIndexToEdit = jsonData.items.findIndex(item => item.id === cardIdToEdit);
            if (cardIndexToEdit !== -1) {
                jsonData.items[cardIndexToEdit].name = newCardName;
                jsonData.items[cardIndexToEdit].order = newCardOrder;
                fs.writeFile('data.json', JSON.stringify(jsonData), (err) => {
                    if (err) {
                        console.error(err);
                        res.status(500).send({message: 'Error writing data'});
                    } else {
                        res.send({message: 'Card edited successfully'});
                    }
                });
            } else {
                res.status(404).send({message: 'Card not found'});
            }
        }
    });
});

app.post('/api/update-order', (req, res) => {
    const updatedOrder = req.body;
    fs.readFile('data.json', (err, data) => {
        if (err) {
            console.error(err);
            res.status(500).send({message: 'Error reading data'});
        } else {
            const jsonData = JSON.parse(data);
            jsonData.items.sort((a, b) => updatedOrder.indexOf(a.id) - updatedOrder.indexOf(b.id));
            jsonData.items.forEach((item, index) => {
                item.order = index + 1;
            });
            fs.writeFile('data.json', JSON.stringify(jsonData), (err) => {
                if (err) {
                    console.error(err);
                    res.status(500).send({message: 'Error writing data'});
                } else {
                    res.send({message: 'Order updated successfully'});
                }
            });
        }
    });
});

app.listen(8000, () => {
    console.log('Server listening on port 8000');
});