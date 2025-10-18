window.scanner = {
    codeReader: null,
    currentDeviceId: null,
    lastIsbn: null,

    startContinuousScan: function (dotNetObj) {
        if (this.codeReader) {
            console.warn("Scanner is already running");
            return;
        }
        
        const hints = new Map();
        hints.set(ZXing.DecodeHintType.POSSIBLE_FORMATS, [
            ZXing.BarcodeFormat.EAN_13
        ]);
        hints.set(ZXing.DecodeHintType.TRY_HARDER, false);

        this.codeReader = new ZXing.BrowserMultiFormatReader(hints);

        this.codeReader.listVideoInputDevices()
            .then(videoInputDevices => {
                if (videoInputDevices.length === 0) {
                    console.error("No video input devices found");
                    return;
                }

                const preferredDevice = videoInputDevices.find(d => d.label.toLowerCase().includes("back")) || videoInputDevices[0];
                this.currentDeviceId = preferredDevice.deviceId;
                
                const constraints = {
                    video: {
                        deviceId: this.currentDeviceId,
                        facingMode: 'environment',
                        width: { ideal: 1920 },
                        height: { ideal: 1080 },
                        focusMode: 'continuous',
                        zoom: { ideal: 1.5 }
                    }
                };

                this.codeReader.decodeFromConstraints(constraints, 'video', (result, err) => {
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
    if (/^\d{13}$/.test(str)) {
        return str.startsWith("978") || str.startsWith("979");
    }
    return /^\d{10}$/.test(str);
}