(function() {
    const originalLog = console.log;
    const originalInfo = console.info;
    const originalWarn = console.warn;
    const originalError = console.error;
    const originalDebug = console.debug;

    function sendToSentinel(level, args) {
        try {
            const message = Array.from(args).map(a => 
                typeof a === 'object' ? JSON.stringify(a) : String(a)
            ).join(' ');

            window.postMessage({
                type: 'SENTINEL_LOG',
                payload: {
                    level: level,
                    message: message,
                    source: window.location.hostname
                }
            }, '*');
        } catch (e) {
            // Sessizce yut
        }
    }

    console.log = function() { sendToSentinel('Information', arguments); originalLog.apply(console, arguments); };
    console.info = function() { sendToSentinel('Information', arguments); originalInfo.apply(console, arguments); };
    console.debug = function() { sendToSentinel('Debug', arguments); originalDebug.apply(console, arguments); };
    console.warn = function() { sendToSentinel('Warning', arguments); originalWarn.apply(console, arguments); };
    console.error = function() { sendToSentinel('Error', arguments); originalError.apply(console, arguments); };

    window.onerror = function(message, source, lineno, colno, error) {
        sendToSentinel('Critical', [message, error?.stack]);
        return false;
    };
})();
