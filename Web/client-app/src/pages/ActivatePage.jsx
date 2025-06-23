import { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import Alert from '../components/ui/Alert';
import Button from '../components/ui/Button';
import Input from '../components/ui/Input';
import authService from '../services/authService';

const ActivatePage = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    token: '',
    password: '',
    repeatPassword: ''
  });
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);

  // Получаем токен из URL параметров
  useEffect(() => {
    const tokenFromUrl = searchParams.get('token');
    if (tokenFromUrl) {
      setFormData(prev => ({ ...prev, token: tokenFromUrl }));
    }
  }, [searchParams]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const validateForm = () => {
    if (!formData.token.trim()) {
      setError('Токен активации обязателен');
      return false;
    }
    
    if (!formData.password) {
      setError('Пароль обязателен');
      return false;
    }
    
    if (formData.password.length < 6) {
      setError('Пароль должен содержать минимум 6 символов');
      return false;
    }
    
    if (formData.password !== formData.repeatPassword) {
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
      await authService.activate(formData.token, formData.password, formData.repeatPassword);
      setSuccess('Аккаунт успешно активирован! Перенаправление на страницу входа...');
      
      // Перенаправляем на страницу входа через 2 секунды
      setTimeout(() => {
        navigate('/login');
      }, 2000);
    } catch (err) {
      setError(err.message || 'Ошибка при активации аккаунта');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-900">
      <div className="max-w-md w-full space-y-8">
        <div>
          <h2 className="mt-6 text-center text-3xl font-extrabold text-white">
            Активация аккаунта
          </h2>
          <p className="mt-2 text-center text-sm text-gray-400">
            Введите токен активации и установите пароль
          </p>
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
            label="Токен активации"
            name="token"
            type="password"
            value={formData.token}
            onChange={handleInputChange}
            placeholder="Введите токен из письма"
            required
          />
          
          <Input
            label="Новый пароль"
            name="password"
            type="password"
            value={formData.password}
            onChange={handleInputChange}
            placeholder="Минимум 6 символов"
            required
          />
          
          <Input
            label="Подтвердите пароль"
            name="repeatPassword"
            type="password"
            value={formData.repeatPassword}
            onChange={handleInputChange}
            placeholder="Повторите пароль"
            required
          />

          <div className="flex space-x-3">
            <Button
              type="submit"
              loading={loading}
              className="flex-1"
            >
              Активировать
            </Button>
            <Button
              type="button"
              variant="secondary"
              onClick={() => navigate('/login')}
              className="flex-1"
            >
              Назад к входу
            </Button>
          </div>
        </form>

        <div className="text-center">
          <p className="text-sm text-gray-400">
            Не получили письмо с токеном?{' '}
            <button 
              className="text-blue-400 hover:text-blue-300"
              onClick={() => navigate('/login')}
            >
              Обратитесь к администратору
            </button>
          </p>
        </div>
      </div>
    </div>
  );
};

export default ActivatePage; 