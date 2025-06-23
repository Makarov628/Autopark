import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import InitialSetupPage from '../pages/InitialSetupPage';
import systemService from '../services/systemService';
import LoadingSpinner from './ui/LoadingSpinner';

const InitialSetupCheck = ({ children }) => {
  const [isSetupComplete, setIsSetupComplete] = useState(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    checkInitialSetup();
  }, []);

  const checkInitialSetup = async () => {
    try {
      const data = await systemService.checkSetupStatus();
      setIsSetupComplete(data.isSetupComplete);
      
      if (!data.isSetupComplete) {
      }
    } catch (error) {
      console.warn('Ошибка при проверке настройки системы:', error);
      // При ошибке предполагаем что система настроена
      setIsSetupComplete(true);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-900">
        <LoadingSpinner size="lg" text="Проверка системы..." />
      </div>
    );
  }

  // Если система настроена, показываем дочерние компоненты
  if (isSetupComplete) {
    return children;
  }

  // Если система не настроена, показываем загрузку (будет редирект)
  return <InitialSetupPage />;
};

export default InitialSetupCheck; 