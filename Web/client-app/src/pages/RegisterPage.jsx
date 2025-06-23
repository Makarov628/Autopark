import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Alert from '../components/ui/Alert';
import Button from '../components/ui/Button';
import Input from '../components/ui/Input';
import authService from '../services/authService';

const RegisterPage = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    email: '',
    firstName: '',
    lastName: '',
    phone: '',
    dateOfBirth: ''
  });
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);

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
    
    if (!formData.dateOfBirth) {
      setError('Дата рождения обязательна');
      return false;
    }
    
    // Проверяем, что пользователь старше 18 лет
    const birthDate = new Date(formData.dateOfBirth);
    const today = new Date();
    const age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();
    
    if (age < 18 || (age === 18 && monthDiff < 0)) {
      setError('Вы должны быть старше 18 лет');
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
      await authService.register(formData);
      setSuccess('Регистрация успешна! Проверьте email для активации аккаунта.');
      
      // Очищаем форму
      setFormData({
        email: '',
        firstName: '',
        lastName: '',
        phone: '',
        dateOfBirth: ''
      });
    } catch (err) {
      setError(err.message || 'Ошибка при регистрации');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-900">
      <div className="max-w-md w-full space-y-8">
        <div>
          <h2 className="mt-6 text-center text-3xl font-extrabold text-white">
            Регистрация
          </h2>
          <p className="mt-2 text-center text-sm text-gray-400">
            Создайте новый аккаунт в системе
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
            label="Email"
            name="email"
            type="email"
            value={formData.email}
            onChange={handleInputChange}
            placeholder="your@email.com"
            required
          />
          
          <Input
            label="Имя"
            name="firstName"
            value={formData.firstName}
            onChange={handleInputChange}
            placeholder="Ваше имя"
            required
          />
          
          <Input
            label="Фамилия"
            name="lastName"
            value={formData.lastName}
            onChange={handleInputChange}
            placeholder="Ваша фамилия"
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
            label="Дата рождения"
            name="dateOfBirth"
            type="date"
            value={formData.dateOfBirth}
            onChange={handleInputChange}
            required
          />

          <div className="flex space-x-3">
            <Button
              type="submit"
              loading={loading}
              className="flex-1"
            >
              Зарегистрироваться
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
            Уже есть аккаунт?{' '}
            <Link 
              to="/login"
              className="text-blue-400 hover:text-blue-300"
            >
              Войти в систему
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default RegisterPage; 