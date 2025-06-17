import { useEffect, useState } from 'react'
import { initializeCsrf, logout } from './utils/api'

// Импортируем компоненты
import BrandModelPage from './BrandModel'
import Driver from './Driver'
import Enterprise from './Enterprise'
import VehiclePage from './Vehicle'
import Login from './components/Login'

function App() {
  // Состояние авторизации
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  
  // Определяем состояние, которое говорит, что сейчас показываем
  const [activeTab, setActiveTab] = useState('vehicle')
  // Возможные значения: 'vehicle', 'brandModel', 'enterprise', 'driver'

  // Инициализируем CSRF токен и проверяем авторизацию при загрузке приложения
  useEffect(() => {
    const initializeApp = async () => {
      try {
        // Инициализируем CSRF токен
        await initializeCsrf();
        
        // Проверяем авторизацию
        await checkAuthStatus();
      } catch (error) {
        console.warn('Ошибка при инициализации приложения:', error);
      } finally {
        setIsLoading(false);
      }
    };

    initializeApp();
  }, []);

  // Функция для проверки статуса авторизации
  const checkAuthStatus = async () => {
    try {
      const response = await fetch('/api/auth/me', {
        credentials: 'include'
      });
      
      if (response.ok) {
        setIsAuthenticated(true);
      } else {
        setIsAuthenticated(false);
      }
    } catch (error) {
      console.warn('Ошибка при проверке авторизации:', error);
      setIsAuthenticated(false);
    }
  };

  // Обработчик успешной авторизации
  const handleLoginSuccess = () => {
    setIsAuthenticated(true);
  };

  // Обработчик выхода
  const handleLogout = async () => {
    const success = await logout();
    if (success) {
      setIsAuthenticated(false);
    } else {
      // Даже если logout не удался, сбрасываем состояние авторизации
      setIsAuthenticated(false);
    }
  };

  // Показываем загрузку
  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-900">
        <div className="text-white text-xl">Загрузка...</div>
      </div>
    );
  }

  // Показываем форму входа, если не авторизован
  if (!isAuthenticated) {
    return <Login onLoginSuccess={handleLoginSuccess} />;
  }

  // Основной интерфейс приложения
  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-3xl font-bold">Autopark Management</h1>
        <button
          onClick={handleLogout}
          className="bg-red-500 hover:bg-red-700 text-white font-bold py-2 px-4 rounded"
        >
          Выйти
        </button>
      </div>

      {/* Кнопки-переключатели */}
      <div className="mb-4">
        <button
          onClick={() => setActiveTab('vehicle')}
          className={`mr-2 px-4 py-2 border rounded ${
            activeTab === 'vehicle' ? 'bg-blue-500 text-white' : 'bg-black-300 text-white'
          }`}
        >
          Транспорт
        </button>
        <button
          onClick={() => setActiveTab('brandModel')}
          className={`mr-2 px-4 py-2 border rounded ${
            activeTab === 'brandModel' ? 'bg-blue-500 text-white' : 'bg-black-300 text-white'
          }`}
        >
          Модели
        </button>
        <button
          onClick={() => setActiveTab('enterprise')}
          className={`mr-2 px-4 py-2 border rounded ${
            activeTab === 'enterprise' ? 'bg-blue-500 text-white' : 'bg-black-300 text-white'
          }`}
        >
          Предприятия
        </button>
        <button
          onClick={() => setActiveTab('driver')}
          className={`px-4 py-2 border rounded ${
            activeTab === 'driver' ? 'bg-blue-500 text-white' : 'bg-black-300 text-white'
          }`}
        >
          Водители
        </button>
      </div>

      {/* Рендерим компонент в зависимости от состояния */}
      {activeTab === 'vehicle' && <VehiclePage />}
      {activeTab === 'brandModel' && <BrandModelPage />}
      {activeTab === 'enterprise' && <Enterprise />}
      {activeTab === 'driver' && <Driver />}
    </div>
  )
}

export default App