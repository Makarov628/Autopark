import { useEffect, useState } from 'react';
import Alert from '../components/ui/Alert';
import LoadingSpinner from '../components/ui/LoadingSpinner';
import apiService from '../services/apiService';

const DashboardPage = () => {
  const [stats, setStats] = useState({
    vehicles: 0,
    drivers: 0,
    enterprises: 0,
    brandModels: 0
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchStats();
  }, []);

  const fetchStats = async () => {
    try {
      setLoading(true);
      
      // Загружаем данные параллельно
      const [vehiclesRes, driversRes, enterprisesRes, brandModelsRes] = await Promise.all([
        apiService.get('/vehicles'),
        apiService.get('/drivers'),
        apiService.get('/enterprises'),
        apiService.get('/brandmodels')
      ]);

      const vehicles = await vehiclesRes.json();
      const drivers = await driversRes.json();
      const enterprises = await enterprisesRes.json();
      const brandModels = await brandModelsRes.json();

      setStats({
        vehicles: vehicles.length,
        drivers: drivers.length,
        enterprises: enterprises.length,
        brandModels: brandModels.length
      });
    } catch (err) {
      setError('Ошибка при загрузке статистики');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <LoadingSpinner size="lg" text="Загрузка статистики..." />;
  }

  return (
    <div className="container mx-auto">
      <div className="mb-6">
        <h1 className="text-3xl font-bold text-white">Панель управления</h1>
        <p className="text-gray-400 mt-2">Обзор системы управления автопарком</p>
      </div>
      
      {error && (
        <Alert 
          type="error" 
          message={error} 
          onClose={() => setError('')}
          className="mb-6"
        />
      )}
      
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div className="bg-gray-800 rounded-lg shadow-lg p-6">
          <div className="flex items-center">
            <div className="p-3 rounded-full bg-blue-500 bg-opacity-20">
              <span className="text-2xl">🚗</span>
            </div>
            <div className="ml-4">
              <p className="text-sm font-medium text-gray-400">Транспорт</p>
              <p className="text-2xl font-bold text-white">{stats.vehicles}</p>
            </div>
          </div>
        </div>

        <div className="bg-gray-800 rounded-lg shadow-lg p-6">
          <div className="flex items-center">
            <div className="p-3 rounded-full bg-green-500 bg-opacity-20">
              <span className="text-2xl">👨‍💼</span>
            </div>
            <div className="ml-4">
              <p className="text-sm font-medium text-gray-400">Водители</p>
              <p className="text-2xl font-bold text-white">{stats.drivers}</p>
            </div>
          </div>
        </div>

        <div className="bg-gray-800 rounded-lg shadow-lg p-6">
          <div className="flex items-center">
            <div className="p-3 rounded-full bg-yellow-500 bg-opacity-20">
              <span className="text-2xl">🏢</span>
            </div>
            <div className="ml-4">
              <p className="text-sm font-medium text-gray-400">Предприятия</p>
              <p className="text-2xl font-bold text-white">{stats.enterprises}</p>
            </div>
          </div>
        </div>

        <div className="bg-gray-800 rounded-lg shadow-lg p-6">
          <div className="flex items-center">
            <div className="p-3 rounded-full bg-purple-500 bg-opacity-20">
              <span className="text-2xl">🏷️</span>
            </div>
            <div className="ml-4">
              <p className="text-sm font-medium text-gray-400">Модели</p>
              <p className="text-2xl font-bold text-white">{stats.brandModels}</p>
            </div>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="bg-gray-800 rounded-lg shadow-lg p-6">
          <h3 className="text-lg font-medium text-white mb-4">Быстрые действия</h3>
          <div className="space-y-3">
            <button className="w-full text-left p-3 bg-gray-700 rounded-lg hover:bg-gray-600 transition-colors">
              <span className="text-blue-400">➕</span>
              <span className="ml-2 text-white">Добавить транспорт</span>
            </button>
            <button className="w-full text-left p-3 bg-gray-700 rounded-lg hover:bg-gray-600 transition-colors">
              <span className="text-green-400">👨‍💼</span>
              <span className="ml-2 text-white">Добавить водителя</span>
            </button>
            <button className="w-full text-left p-3 bg-gray-700 rounded-lg hover:bg-gray-600 transition-colors">
              <span className="text-yellow-400">🏢</span>
              <span className="ml-2 text-white">Добавить предприятие</span>
            </button>
          </div>
        </div>

        <div className="bg-gray-800 rounded-lg shadow-lg p-6">
          <h3 className="text-lg font-medium text-white mb-4">Системная информация</h3>
          <div className="space-y-3 text-sm">
            <div className="flex justify-between">
              <span className="text-gray-400">Версия системы:</span>
              <span className="text-white">1.0.0</span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-400">Статус:</span>
              <span className="text-green-400">Активна</span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-400">Последнее обновление:</span>
              <span className="text-white">{new Date().toLocaleDateString('ru-RU')}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default DashboardPage; 