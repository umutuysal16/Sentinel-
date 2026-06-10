export class GlobalLogger {
  private static agentId: string | null = null;
  private static isInitialized = false;

  public static async init() {
    if (this.isInitialized) return;
    this.isInitialized = true;

    try {
      // 1. Sisteme kayıt ol
      const regRes = await fetch('/api/BrowserLogs/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ Name: 'Sentinel-Dashboard-Client', IpAddress: '127.0.0.1' })
      });

      if (regRes.ok) {
        const data = await regRes.json();
        this.agentId = data.agentId;
        
        // Düzenli Heartbeat gönder
        setInterval(() => this.sendHeartbeat(), 30000);

        // Olayları hijack et (ele geçir)
        this.hijackConsole();
        this.hijackWindowErrors();
      }
    } catch (e) {
      console.error("Sentinel Global Logger başlatılamadı:", e);
    }
  }

  private static async sendHeartbeat() {
    if (!this.agentId) return;
    try {
      await fetch('/api/BrowserLogs/heartbeat', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ AgentId: this.agentId, Hostname: window.location.hostname })
      });
    } catch {
      // sessizce yut
    }
  }

  private static async sendLog(level: string, message: string, source: string, stack?: string) {
    if (!this.agentId) return;
    try {
      await fetch('/api/BrowserLogs', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          AgentId: this.agentId,
          Level: level,
          Message: message,
          Source: source,
          StackTrace: stack,
          Properties: { Url: window.location.href, UserAgent: navigator.userAgent }
        })
      });
    } catch {
      // sessizce yut
    }
  }

  private static hijackConsole() {
    const originalError = console.error;
    const originalWarn = console.warn;
    const originalLog = console.log;
    const originalInfo = console.info;
    const originalDebug = console.debug;

    console.error = (...args: any[]) => {
      originalError.apply(console, args);
      this.sendLog('Error', args.map(a => String(a)).join(' '), 'Console.Error');
    };

    console.warn = (...args: any[]) => {
      originalWarn.apply(console, args);
      this.sendLog('Warning', args.map(a => String(a)).join(' '), 'Console.Warn');
    };

    console.log = (...args: any[]) => {
      originalLog.apply(console, args);
      this.sendLog('Information', args.map(a => String(a)).join(' '), 'Console.Log');
    };

    console.info = (...args: any[]) => {
      originalInfo.apply(console, args);
      this.sendLog('Information', args.map(a => String(a)).join(' '), 'Console.Info');
    };

    console.debug = (...args: any[]) => {
      originalDebug.apply(console, args);
      this.sendLog('Debug', args.map(a => String(a)).join(' '), 'Console.Debug');
    };
  }

  private static hijackWindowErrors() {
    window.onerror = (message, _source, _lineno, _colno, error) => {
      this.sendLog('Critical', String(message), 'Window.OnError', error?.stack);
      return false; // Hatanın default tarayıcı davranışını bozma
    };

    window.onunhandledrejection = (event) => {
      this.sendLog('Critical', `Unhandled Promise Rejection: ${event.reason}`, 'Window.UnhandledRejection', event.reason?.stack);
    };
  }
}
