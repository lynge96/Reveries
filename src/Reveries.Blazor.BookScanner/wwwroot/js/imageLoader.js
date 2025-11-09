window.preloadImage = (url) => {
    if (!url) return;
    const img = new Image();
    img.src = url;  // Browser caching image
};
