const fs = require('fs');
const jsonData = require('./data.json');

function updateOrder() {
    jsonData.items.forEach((item, index) => {
        item.order = index;
    });

    fs.writeFileSync('data.json', JSON.stringify(jsonData, null, 2));

    console.log('Data file updated!');
}

const replaceOrder_row_col = () => {
    jsonData.items.forEach((item, index) => {
        item.col = index % 3; // assign column (0, 1, or 2)
        item.row = Math.floor(index / 3); // assign row
        delete item.order; // remove order property
    });

    fs.writeFile('data.json', JSON.stringify(jsonData, null, 2), (err) => {
        if (err) {
            console.error(err);
        } else {
            console.log('Data updated successfully');
        }
    });
};




fs.writeFile('data.json', JSON.stringify(jsonData, null, 2), (err) => {
    if (err) {
        console.error(err);
    } else {
        console.log('Data updated successfully');
    }
});

function updateDataFile() {
    updateOrder()
    replaceOrder_row_col()
}

// Call the function to update the json file with new code updates
updateDataFile();