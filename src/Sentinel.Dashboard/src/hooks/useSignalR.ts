import { useEffect, useRef } from 'react';
import { HubConnectionBuilder, LogLevel, HubConnection } from '@microsoft/signalr';
import { useQueryClient } from '@tanstack/react-query';

export function useSignalR() {
  const queryClient = useQueryClient();
  const connectionRef = useRef<HubConnection | null>(null);

  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl('/hubs/alerts')
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .configureLogging(LogLevel.Information)
      .build();

    connection.on('ReceiveAlert', () => {
      queryClient.invalidateQueries({ queryKey: ['alerts'] });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });
    });

    connection.on('ReceiveAgentStatus', () => {
      queryClient.invalidateQueries({ queryKey: ['agents'] });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });
    });

    connection.on('ReceiveLog', () => {
      queryClient.invalidateQueries({ queryKey: ['logs'] });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });
    });

    connection.start().catch(err => console.error('SignalR connection error:', err));
    connectionRef.current = connection;

    return () => {
      connection.stop();
    };
  }, [queryClient]);

  return connectionRef;
}
