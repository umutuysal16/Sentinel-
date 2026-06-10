import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'providers/app_provider.dart';
import 'services/api_service.dart';
import 'services/signalr_service.dart';
import 'theme/app_theme.dart';
import 'screens/dashboard_screen.dart';
import 'screens/alerts_screen.dart';
import 'screens/agents_screen.dart';

void main() {
  runApp(const SentinelApp());
}

class SentinelApp extends StatelessWidget {
  const SentinelApp({super.key});

  @override
  Widget build(BuildContext context) {
    // Change this to your API URL
    // For Android emulator use 10.0.2.2 instead of localhost
    // For physical device use your PC's IP address
    // 10.0.2.2 = host machine's localhost from Android emulator
    const apiBaseUrl = 'http://localhost:5052';
    const signalRUrl = 'http://localhost:5052/hubs/alerts';

    final apiService = ApiService(baseUrl: apiBaseUrl);
    final signalRService = SignalRService(hubUrl: signalRUrl);

    return ChangeNotifierProvider(
      create: (_) => AppProvider(apiService: apiService, signalRService: signalRService),
      child: MaterialApp(
        title: 'Sentinel',
        debugShowCheckedModeBanner: false,
        theme: AppTheme.darkTheme,
        home: const MainScreen(),
      ),
    );
  }
}

class MainScreen extends StatefulWidget {
  const MainScreen({super.key});

  @override
  State<MainScreen> createState() => _MainScreenState();
}

class _MainScreenState extends State<MainScreen> {
  int _currentIndex = 0;

  final _screens = const [
    DashboardScreen(),
    AlertsScreen(),
    AgentsScreen(),
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SafeArea(child: _screens[_currentIndex]),
      bottomNavigationBar: BottomNavigationBar(
        currentIndex: _currentIndex,
        onTap: (i) => setState(() => _currentIndex = i),
        items: const [
          BottomNavigationBarItem(icon: Icon(Icons.dashboard), label: 'Dashboard'),
          BottomNavigationBarItem(icon: Icon(Icons.warning_amber), label: 'Alerts'),
          BottomNavigationBarItem(icon: Icon(Icons.dns), label: 'Agents'),
        ],
      ),
    );
  }
}
