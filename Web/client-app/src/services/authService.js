import useAuthStore from '../stores/authStore';
import apiService from './apiService';

class AuthService {
  // Вход в систему
  async login(email, password) {
    try {
      const response = await fetch('/api/user/login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          email,
          password,
          deviceName: 'Web Browser',
          deviceType: 0
        })
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Ошибка авторизации');
      }

      const data = await response.json();
      
      // Сохраняем токены и информацию о пользователе
      const { login } = useAuthStore.getState();
      login(data.accessToken, data.refreshToken, {
        id: data.userId,
        email: data.email,
        firstName: data.firstName,
        lastName: data.lastName
      });

      return data;
    } catch (error) {
      throw error;
    }
  }

  // Регистрация
  async register(userData) {
    try {
      const response = await fetch('/api/user/register', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(userData)
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Ошибка регистрации');
      }

      // return await response.json();
    } catch (error) {
      throw error;
    }
  }

  // Активация аккаунта
  async activate(token, password, repeatPassword) {
    try {
      const response = await fetch('/api/user/activate', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          token,
          password,
          repeatPassword
        })
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Ошибка активации');
      }

      return true;
    } catch (error) {
      throw error;
    }
  }

  // Выход из системы
  async logout() {
    try {
      const { refreshToken, logout: logoutStore } = useAuthStore.getState();
      
      if (refreshToken) {
        await apiService.post('/user/logout', { refreshToken });
      }
    } catch (error) {
      console.warn('Ошибка при выходе:', error);
    } finally {
      // Всегда очищаем локальное состояние
      logoutStore();
    }
  }

  // Проверка авторизации
  async checkAuth() {
    try {
      const { isAuthenticated, isTokenExpired } = useAuthStore.getState();
      
      if (!isAuthenticated || isTokenExpired()) {
        return false;
      }

      // Проверяем токен на сервере
      const response = await apiService.get('/user/me');
      return response.ok;
    } catch (error) {
      return false;
    }
  }

  // Получение информации о текущем пользователе
  async getCurrentUser() {
    try {
      const response = await apiService.get('/user/me');
      if (response.ok) {
        return await response.json();
      }
      throw new Error('Не удалось получить информацию о пользователе');
    } catch (error) {
      throw error;
    }
  }

  // Установка push токена
  async setPushToken(token) {
    try {
      const response = await apiService.post('/user/push-token', { token });
      if (!response.ok) {
        throw new Error('Не удалось установить push токен');
      }
      return true;
    } catch (error) {
      throw error;
    }
  }
}

// Создаем экземпляр сервиса
const authService = new AuthService();

export default authService; 