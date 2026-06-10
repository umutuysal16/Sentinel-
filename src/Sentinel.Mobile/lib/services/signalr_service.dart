import 'package:signalr_netcore/signalr_client.dart';

class SignalRService {
  late HubConnection _hubConnection;
  final String hubUrl;
  Function? onAlertReceived;
  Function? onAgentStatusReceived;

  SignalRService({required this.hubUrl});

  Future<void> connect() async {
    _hubConnection = HubConnectionBuilder()
        .withUrl(hubUrl)
        .withAutomaticReconnect()
        .build();

    _hubConnection.on('ReceiveAlert', (args) {
      onAlertReceived?.call(args);
    });

    _hubConnection.on('ReceiveAgentStatus', (args) {
      onAgentStatusReceived?.call(args);
    });

    _hubConnection.on('ReceiveLog', (args) {
      // Handled by polling in mobile for simplicity
    });

    try {
      await _hubConnection.start();
    } catch (e) {
      // Will auto-reconnect
    }
  }

  Future<void> disconnect() async {
    await _hubConnection.stop();
  }
}
