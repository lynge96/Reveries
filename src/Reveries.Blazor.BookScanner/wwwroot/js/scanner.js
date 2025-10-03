window.scanner = {
    startContinuousScan: function (dotNetObj) {
        const codeReader = new ZXing.BrowserMultiFormatReader();
        let lastIsbn = null;

        codeReader.listVideoInputDevices()
            .then(videoInputDevices => {
                const deviceId = videoInputDevices[0].deviceId;

                codeReader.decodeFromVideoDevice(deviceId, 'video', (result, err) => {
                    if (result) {
                        const text = result.text.replace(/-/g, '');
                        if (isValidIsbn(text) && text !== lastIsbn) {
                            lastIsbn = text;
                            dotNetObj.invokeMethodAsync('OnIsbnScanned', text);
                        }
                    }
                });
            })
            .catch(err => console.error(err));
    }
};

// Simpel ISBN check (kun længde og cifre)
function isValidIsbn(str) {
    return (/^\d{10}$/.test(str) || /^\d{13}$/.test(str));
}
