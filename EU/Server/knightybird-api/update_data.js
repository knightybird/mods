const fs = require('fs');
const data = require('./data.json');

function updateOrder() {
    data.items.forEach((item, index) => {
        item.order = index;
    });

    fs.writeFileSync('data.json', JSON.stringify(data, null, 2));

    console.log('Data file updated!');
}

function updateDataFile() {
    updateOrder()
}

// Call the function to update the json file with new code updates
updateDataFile();