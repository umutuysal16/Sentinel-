import { useEffect, useRef, useState } from 'react';
import { useQueryClient } from '@tanstack/react-query';

// Simple beep sound as base64 WAV
const BEEP_BASE64 = 'data:audio/wav;base64,UklGRnoGAABXQVZFZm10IBAAAAABAAEAQB8AAEAfAAABAAgAZGF0YQoGAACBhYqFbF1fdH2JkZqTi3x0dX+Ij5SRiYB3dHl/hoyQkIqDe3h5fIKHi46LhoF8eXl7f4OGiYmGg398eXl7fYCDhYaFg4B9e3p6e31/gYOEhIOBf317ent8fX+BgoODgYB+fHt7fH1+gIGCgoKBf358e3t8fX5/gIGBgYGAf358fHx8fX5/gICBgYCAf358fHx8fX5/gICAgICAf358fHx9fn5/gICAgIB/f358fHx9fn5/f4CAgIB/f358fHx9fn5/f4CAgIB/f35+fX19fn5/f4B/gIB/f35+fX19fn5/f39/gIB/f35+fX19fn5/f39/f4B/f35+fn19fn5/f39/f39/f35+fn5+fn5/f39/f39/f35+fn5+fn5/f39/f39/f39+fn5+fn5+f39/f39/f39/fn5+fn5+fn9/f39/f39/f35+fn5+fn5/f39/f39/f39+fn5+fn5+f39/f39/f39/';

export function useAlertSound() {
  const [soundEnabled, setSoundEnabled] = useState(() => {
    const saved = localStorage.getItem('sentinel-sound');
    return saved !== 'false';
  });
  const audioRef = useRef<HTMLAudioElement | null>(null);
  const queryClient = useQueryClient();
  const prevAlertCountRef = useRef<number>(0);

  useEffect(() => {
    localStorage.setItem('sentinel-sound', String(soundEnabled));
  }, [soundEnabled]);

  useEffect(() => {
    audioRef.current = new Audio(BEEP_BASE64);
    audioRef.current.volume = 0.5;
  }, []);

  useEffect(() => {
    if (!soundEnabled) return;

    const unsubscribe = queryClient.getQueryCache().subscribe((event) => {
      if (event.type === 'updated' && event.query.queryKey[0] === 'alerts') {
        const data = event.query.state.data as { totalCount?: number } | undefined;
        if (data?.totalCount && data.totalCount > prevAlertCountRef.current && prevAlertCountRef.current > 0) {
          audioRef.current?.play().catch(() => {});
        }
        if (data?.totalCount !== undefined) {
          prevAlertCountRef.current = data.totalCount;
        }
      }
    });

    return () => unsubscribe();
  }, [soundEnabled, queryClient]);

  const toggleSound = () => setSoundEnabled(prev => !prev);

  return { soundEnabled, toggleSound };
}
