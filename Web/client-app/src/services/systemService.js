class SystemService {
  // Проверка статуса первоначальной настройки
  async checkSetupStatus() {
    try {
      const response = await fetch('/api/system/check-setup');
      if (response.ok) {
        return await response.json();
      }
      throw new Error('Ошибка при проверке статуса настройки');
    } catch (error) {
      console.warn('Ошибка при проверке настройки:', error);
      // При ошибке предполагаем что система настроена
      return { isSetupComplete: true };
    }
  }

  // Первоначальная настройка системы
  async performInitialSetup(adminData) {
    try {
      const response = await fetch('/api/system/initial-setup', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ adminData })
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Ошибка при настройке системы');
      }

      return await response.json();
    } catch (error) {
      throw error;
    }
  }

  // Получение информации о системе
  async getSystemInfo() {
    try {
      const response = await fetch('/api/system/info');
      if (response.ok) {
        return await response.json();
      }
      throw new Error('Ошибка при получении информации о системе');
    } catch (error) {
      throw error;
    }
  }

  // Проверка здоровья системы
  async checkHealth() {
    try {
      const response = await fetch('/api/system/health');
      if (response.ok) {
        return await response.json();
      }
      throw new Error('Система недоступна');
    } catch (error) {
      throw error;
    }
  }
}

// Создаем экземпляр сервиса
const systemService = new SystemService();

export default systemService; 