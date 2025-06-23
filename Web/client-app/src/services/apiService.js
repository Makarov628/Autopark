import useAuthStore from '../stores/authStore';

const API_BASE_URL = '/api';

class ApiService {
  constructor() {
    this.isRefreshing = false;
    this.failedQueue = [];
  }

  // Обработка очереди запросов во время refresh
  processQueue(error, token = null) {
    this.failedQueue.forEach(({ resolve, reject }) => {
      if (error) {
        reject(error);
      } else {
        resolve(token);
      }
    });
    this.failedQueue = [];
  }

  // Получение заголовков с токеном
  getHeaders() {
    const { accessToken } = useAuthStore.getState();
    return {
      'Content-Type': 'application/json',
      ...(accessToken && { 'Authorization': `Bearer ${accessToken}` })
    };
  }

  // Refresh токена
  async refreshToken() {
    const { refreshToken, updateTokens, logout } = useAuthStore.getState();
    
    if (!refreshToken) {
      logout();
      throw new Error('No refresh token available');
    }

    try {
      const response = await fetch(`${API_BASE_URL}/user/refresh-token`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ refreshToken })
      });

      if (response.ok) {
        const data = await response.json();
        updateTokens(data.accessToken, data.refreshToken);
        return data.accessToken;
      } else {
        logout();
        throw new Error('Failed to refresh token');
      }
    } catch (error) {
      logout();
      throw error;
    }
  }

  // Основной метод для выполнения запросов
  async request(url, options = {}) {
    const { method = 'GET', body, ...restOptions } = options;

    // Проверяем, не истек ли токен
    const { isTokenExpired } = useAuthStore.getState();
    if (isTokenExpired()) {
      if (!this.isRefreshing) {
        this.isRefreshing = true;
        try {
          await this.refreshToken();
          this.processQueue(null, useAuthStore.getState().accessToken);
        } catch (error) {
          this.processQueue(error, null);
          throw error;
        } finally {
          this.isRefreshing = false;
        }
      } else {
        // Если refresh уже выполняется, добавляем в очередь
        return new Promise((resolve, reject) => {
          this.failedQueue.push({ resolve, reject });
        }).then(token => {
          return this.request(url, { ...options, token });
        });
      }
    }

    const requestOptions = {
      method,
      headers: this.getHeaders(),
      ...restOptions
    };

    if (body) {
      requestOptions.body = JSON.stringify(body);
    }

    const response = await fetch(`${API_BASE_URL}${url}`, requestOptions);

    // Обработка 401 ошибки
    if (response.status === 401) {
      if (!this.isRefreshing) {
        this.isRefreshing = true;
        try {
          await this.refreshToken();
          this.processQueue(null, useAuthStore.getState().accessToken);
          // Повторяем запрос с новым токеном
          return this.request(url, options);
        } catch (error) {
          this.processQueue(error, null);
          useAuthStore.getState().logout();
          window.location.href = '/login';
          throw error;
        } finally {
          this.isRefreshing = false;
        }
      } else {
        // Если refresh уже выполняется, добавляем в очередь
        return new Promise((resolve, reject) => {
          this.failedQueue.push({ resolve, reject });
        }).then(token => {
          return this.request(url, { ...options, token });
        });
      }
    }

    return response;
  }

  // Методы для разных типов запросов
  async get(url) {
    return this.request(url, { method: 'GET' });
  }

  async post(url, data) {
    return this.request(url, { method: 'POST', body: data });
  }

  async put(url, data) {
    return this.request(url, { method: 'PUT', body: data });
  }

  async delete(url) {
    return this.request(url, { method: 'DELETE' });
  }

  async patch(url, data) {
    return this.request(url, { method: 'PATCH', body: data });
  }

  // Получить пользователей без роли (например, Driver или Manager)
  async getUsersWithoutRole(role) {
    const response = await this.get(`/user/available?notHasRole=${role}`);
    if (!response.ok) throw new Error('Ошибка при загрузке пользователей');
    return response.json();
  }
}

// Создаем экземпляр сервиса
const apiService = new ApiService();

export default apiService; 