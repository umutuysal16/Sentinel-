const script = document.createElement('script');
script.src = chrome.runtime.getURL('inject.js');
script.onload = function() {
    this.remove();
};
(document.head || document.documentElement).appendChild(script);

window.addEventListener('message', function(event) {
    if (event.source !== window) return;
    if (event.data && event.data.type === 'SENTINEL_LOG') {
        chrome.runtime.sendMessage(event.data.payload);
    }
});
