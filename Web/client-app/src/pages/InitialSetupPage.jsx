import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Alert from '../components/ui/Alert';
import Button from '../components/ui/Button';
import Input from '../components/ui/Input';
import LoadingSpinner from '../components/ui/LoadingSpinner';
import systemService from '../services/systemService';

const InitialSetupPage = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    email: '',
    firstName: '',
    lastName: '',
    phone: '',
    password: '',
    confirmPassword: ''
  });
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);
  const [checkingSetup, setCheckingSetup] = useState(true);

  // Проверяем, нужна ли первоначальная настройка
  useEffect(() => {
    checkInitialSetup();
  }, []);

  const checkInitialSetup = async () => {
    try {
      const data = await systemService.checkSetupStatus();
      
      if (data.isSetupComplete) {
        // Система уже настроена, перенаправляем на логин
        navigate('/login');
      } else {
        setCheckingSetup(false);
      }
    } catch (error) {
      console.warn('Ошибка при проверке настройки:', error);
      setCheckingSetup(false);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const validateForm = () => {
    if (!formData.email.trim()) {
      setError('Email обязателен');
      return false;
    }
    
    if (!formData.email.includes('@')) {
      setError('Введите корректный email');
      return false;
    }
    
    if (!formData.firstName.trim()) {
      setError('Имя обязательно');
      return false;
    }
    
    if (!formData.lastName.trim()) {
      setError('Фамилия обязательна');
      return false;
    }
    
    if (!formData.phone.trim()) {
      setError('Телефон обязателен');
      return false;
    }
    
    if (!formData.password) {
      setError('Пароль обязателен');
      return false;
    }
    
    if (formData.password.length < 8) {
      setError('Пароль должен содержать минимум 8 символов');
      return false;
    }
    
    // Проверяем сложность пароля
    const hasUpperCase = /[A-Z]/.test(formData.password);
    const hasLowerCase = /[a-z]/.test(formData.password);
    const hasNumbers = /\d/.test(formData.password);
    const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(formData.password);
    
    if (!hasUpperCase || !hasLowerCase || !hasNumbers || !hasSpecialChar) {
      setError('Пароль должен содержать заглавные и строчные буквы, цифры и специальные символы');
      return false;
    }
    
    if (formData.password !== formData.confirmPassword) {
      setError('Пароли не совпадают');
      return false;
    }
    
    return true;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    if (!validateForm()) {
      return;
    }

    try {
      setLoading(true);
      await systemService.performInitialSetup({
        email: formData.email,
        firstName: formData.firstName,
        lastName: formData.lastName,
        phone: formData.phone,
        password: formData.password
      });

      setSuccess('Система успешно настроена! Перенаправление на страницу входа...');
      
      // Перенаправляем на страницу входа через 3 секунды
      setTimeout(() => {
        navigate('/login');
      }, 3000);
    } catch (err) {
      setError(err.message || 'Ошибка при настройке системы');
    } finally {
      setLoading(false);
    }
  };

  if (checkingSetup) {
    return (
      <div className="min-w-screen min-h-screen flex items-center justify-center bg-gray-900">
        <LoadingSpinner size="lg" text="Проверка настройки системы..." />
      </div>
    );
  }

  return (
    <div className="min-w-lg min-h-screen flex items-center justify-center bg-gray-900">
      <div className="max-w-md w-full space-y-8">
        <div>
          <h2 className="mt-6 text-center text-3xl font-extrabold text-white">
            Первоначальная настройка
          </h2>
          <p className="mt-2 text-center text-sm text-gray-400">
            Создание администратора системы
          </p>
          <div className="mt-4 p-4 bg-blue-900 bg-opacity-20 border border-blue-500 rounded-lg">
            <p className="text-sm text-blue-300">
              ⚠️ Это первый запуск системы. Создайте учетную запись администратора для управления автопарком.
            </p>
          </div>
        </div>

        {error && (
          <Alert 
            type="error" 
            message={error} 
            onClose={() => setError('')}
          />
        )}

        {success && (
          <Alert 
            type="success" 
            message={success} 
            onClose={() => setSuccess('')}
          />
        )}

        <form className="mt-8 space-y-6" onSubmit={handleSubmit}>
          <Input
            label="Email администратора"
            name="email"
            type="email"
            value={formData.email}
            onChange={handleInputChange}
            placeholder="admin@company.com"
            required
          />

          <Input
            label="Имя"
            name="firstName"
            value={formData.firstName}
            onChange={handleInputChange}
            placeholder="Имя администратора"
            required
          />

          <Input
            label="Фамилия"
            name="lastName"
            value={formData.lastName}
            onChange={handleInputChange}
            placeholder="Фамилия администратора"
            required
          />

          <Input
            label="Телефон"
            name="phone"
            type="tel"
            value={formData.phone}
            onChange={handleInputChange}
            placeholder="+7 (999) 123-45-67"
            required
          />

          <Input
            label="Пароль администратора"
            name="password"
            type="password"
            value={formData.password}
            onChange={handleInputChange}
            placeholder="Минимум 8 символов"
            required
          />

          <Input
            label="Подтвердите пароль"
            name="confirmPassword"
            type="password"
            value={formData.confirmPassword}
            onChange={handleInputChange}
            placeholder="Повторите пароль"
            required
          />

          <div className="bg-yellow-900 bg-opacity-20 border border-yellow-500 rounded-lg p-4">
            <h4 className="text-yellow-300 font-medium mb-2">Требования к паролю:</h4>
            <ul className="text-sm text-yellow-200 space-y-1">
              <li>• Минимум 8 символов</li>
              <li>• Заглавные и строчные буквы</li>
              <li>• Цифры</li>
              <li>• Специальные символы (!@#$%^&*)</li>
            </ul>
          </div>

          <Button
            type="submit"
            loading={loading}
            className="w-full"
          >
            Настроить систему
          </Button>
        </form>

        <div className="text-center">
          <p className="text-sm text-gray-400">
            После настройки вы сможете войти в систему с созданными учетными данными
          </p>
        </div>
      </div>
    </div>
  );
};

export default InitialSetupPage; 