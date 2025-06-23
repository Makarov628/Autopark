import { jwtDecode } from 'jwt-decode';
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

const useAuthStore = create(
  persist(
    (set, get) => ({
      // Состояние
      user: null,
      accessToken: null,
      refreshToken: null,
      isAuthenticated: false,
      isLoading: false,

      // Действия
      login: (accessToken, refreshToken, user) => {
        set({
          accessToken,
          refreshToken,
          user,
          isAuthenticated: true,
          isLoading: false
        });
      },

      logout: () => {
        set({
          user: null,
          accessToken: null,
          refreshToken: null,
          isAuthenticated: false,
          isLoading: false
        });
        // Очищаем localStorage
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
      },

      setLoading: (isLoading) => {
        set({ isLoading });
      },

      updateTokens: (accessToken, refreshToken) => {
        set({
          accessToken,
          refreshToken,
          isAuthenticated: true
        });
      },

      // Проверка срока действия токена
      isTokenExpired: () => {
        const { accessToken } = get();
        if (!accessToken) return true;

        try {
          const decoded = jwtDecode(accessToken);
          const currentTime = Date.now() / 1000;
          return decoded.exp < currentTime;
        } catch (error) {
          console.error('Error decoding token:', error);
          return true;
        }
      },

      // Получение информации о пользователе из токена
      getUserFromToken: () => {
        const { accessToken } = get();
        if (!accessToken) return null;

        try {
          const decoded = jwtDecode(accessToken);
          return {
            id: decoded.userId,
            email: decoded.email,
            roles: decoded.role ? [decoded.role] : []
          };
        } catch (error) {
          console.error('Error decoding user from token:', error);
          return null;
        }
      }
    }),
    {
      name: 'auth-storage',
      partialize: (state) => ({
        accessToken: state.accessToken,
        refreshToken: state.refreshToken,
        user: state.user
      })
    }
  )
);

export default useAuthStore; 