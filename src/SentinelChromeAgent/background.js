let agentId = null;

async function registerAgent() {
    try {
        const res = await fetch('http://localhost:5052/api/BrowserLogs/register', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Name: 'Sentinel-Chrome-Extension', IpAddress: '0.0.0.0' })
        });
        const data = await res.json();
        agentId = data.agentId;
    } catch (e) {
        console.error("Sentinel Registration failed", e);
    }
}

// Eklenti yüklendiğinde kendini kaydet
registerAgent();

// Saniyede bir heartbeat gönder
setInterval(() => {
    if (agentId) {
        fetch('http://localhost:5052/api/BrowserLogs/heartbeat', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ AgentId: agentId, Hostname: 'chrome-extension' })
        }).catch(() => {});
    }
}, 30000);

// Sayfalardaki content script'ten gelen (hiçbir log ayrımı yapmadan bütün logları) API'ye yolla
chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
    if (!agentId) return;

    fetch('http://localhost:5052/api/BrowserLogs', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            AgentId: agentId,
            Level: message.level,
            Message: message.message,
            Source: 'Plugin:' + message.source,
            Properties: { Url: sender.tab ? sender.tab.url : 'unknown' }
        })
    }).catch(e => console.error(e));
});
