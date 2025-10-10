window.scanner = {
    codeReader: null,
    currentDeviceId: null,
    lastIsbn: null,

    startContinuousScan: function (dotNetObj) {
        if (this.codeReader) {
            console.warn("Scanner is already running");
            return;
        }

        this.codeReader = new ZXing.BrowserMultiFormatReader();

        this.codeReader.listVideoInputDevices()
            .then(videoInputDevices => {
                if (videoInputDevices.length === 0) {
                    console.error("No video input devices found");
                    return;
                }

                const preferredDevice = videoInputDevices.find(d => d.label.toLowerCase().includes("back")) || videoInputDevices[0];
                this.currentDeviceId = preferredDevice.deviceId;

                this.codeReader.decodeFromVideoDevice(this.currentDeviceId, 'video', (result, err) => {
                    if (result) {
                        const text = result.text.replace(/-/g, '');
                        if (isValidIsbn(text) && text !== this.lastIsbn) {
                            this.lastIsbn = text;
                            dotNetObj.invokeMethodAsync('OnIsbnScanned', text);
                        }
                    }
                });
            })
            .catch(err => console.error("Error initializing scanner:", err));
    },

    stopContinuousScan: function () {
        if (this.codeReader) {
            try {
                this.codeReader.reset();
            } catch (e) {
                console.warn("Error stopping scanner:", e);
            }
            this.codeReader = null;
        }

        const video = document.getElementById('video');
        if (video && video.srcObject) {
            video.srcObject.getTracks().forEach(track => track.stop());
            video.srcObject = null;
        }

        this.lastIsbn = null;
    },

    resetLastIsbn: function () {
        this.lastIsbn = null;
    }
};

function isValidIsbn(str) {
    if (!(/^\d{10}$/.test(str) || /^\d{13}$/.test(str))) return false;
    return !(str.length === 13 && !str.startsWith("978") && !str.startsWith("979"));
}
