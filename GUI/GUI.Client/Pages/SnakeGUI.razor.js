
window.initRenderJS = (instance) => {
    window.theInstance = instance;
};

document.addEventListener('keydown', function (event) {
    // Optionally log the key for testing
    console.log('Key pressed:', event.key);

    // Convert the key to lowercase and check if it's a valid movement key
    let key = event.key.toLowerCase();

    if (["w", "a", "s", "d"].includes(key)) {
        // Call the C# method and pass the key pressed
        theInstance.invokeMethodAsync('HandleKeyPress', key);
    }

});