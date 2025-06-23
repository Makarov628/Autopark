import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import authService from '../services/authService';
import useAuthStore from '../stores/authStore';
import Alert from './ui/Alert';
import Button from './ui/Button';
import Input from './ui/Input';

const Login = () => {
  const [formData, setFormData] = useState({
    email: '',
    password: ''
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { setLoading: setAuthLoading } = useAuthStore();
  const navigate = useNavigate();

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    setAuthLoading(true);

    try {
      await authService.login(formData.email, formData.password);
      navigate('/');
    } catch (err) {
      setError(err.message || 'Ошибка при попытке входа');
    } finally {
      setLoading(false);
      setAuthLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-900">
      <div className="max-w-md w-full space-y-8">
        <div>
          <h2 className="mt-6 text-center text-3xl font-extrabold text-white">
            Вход в систему
          </h2>
          <p className="mt-2 text-center text-sm text-gray-400">
            Автопарк - Система управления
          </p>
        </div>
        
        {error && (
          <Alert 
            type="error" 
            message={error} 
            onClose={() => setError('')}
          />
        )}
        
        <form className="mt-8 space-y-6" onSubmit={handleSubmit}>
          <Input
            label="Email"
            name="email"
            type="email"
            value={formData.email}
            onChange={handleInputChange}
            required
          />
          
          <Input
            label="Пароль"
            name="password"
            type="password"
            value={formData.password}
            onChange={handleInputChange}
            required
          />

          <Button
            type="submit"
            loading={loading}
            className="w-full"
          >
            Войти
          </Button>
        </form>

        <div className="text-center space-y-2">
          <p className="text-sm text-gray-400">
            Нет аккаунта?{' '}
            <Link 
              to="/register"
              className="text-blue-400 hover:text-blue-300"
            >
              Зарегистрироваться
            </Link>
          </p>
          <p className="text-sm text-gray-400">
            Нужна активация?{' '}
            <Link 
              to="/activate"
              className="text-blue-400 hover:text-blue-300"
            >
              Активировать аккаунт
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default Login; 