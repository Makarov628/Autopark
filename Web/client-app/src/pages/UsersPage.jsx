import { useEffect, useState } from 'react';
import Alert from '../components/ui/Alert';
import LoadingSpinner from '../components/ui/LoadingSpinner';
import apiService from '../services/apiService';

const ROLE_LABELS = {
  0: 'Админ',
  1: 'Менеджер',
  2: 'Водитель'
};

const UsersPage = () => {
  const [users, setUsers] = useState([]);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      setLoading(true);
      const response = await apiService.get('/user');
      if (!response.ok) throw new Error('Ошибка при загрузке пользователей');
      const data = await response.json();
      setUsers(data);
    } catch {
      setError('Ошибка при загрузке пользователей');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <LoadingSpinner size="lg" text="Загрузка пользователей..." />;
  }

  return (
    <div className="container mx-auto">
      <h1 className="text-3xl font-bold text-white mb-6">Пользователи</h1>
      {error && <Alert type="error" message={error} onClose={() => setError('')} className="mb-4" />}
      <div className="bg-gray-800 rounded-lg shadow-lg p-6">
        <table className="min-w-full">
          <thead>
            <tr className="bg-gray-700">
              <th className="px-4 py-2 text-left text-xs font-medium text-gray-400 uppercase">ID</th>
              <th className="px-4 py-2 text-left text-xs font-medium text-gray-400 uppercase">ФИО</th>
              <th className="px-4 py-2 text-left text-xs font-medium text-gray-400 uppercase">Email</th>
              <th className="px-4 py-2 text-left text-xs font-medium text-gray-400 uppercase">Телефон</th>
              <th className="px-4 py-2 text-left text-xs font-medium text-gray-400 uppercase">Роли</th>
              <th className="px-4 py-2 text-left text-xs font-medium text-gray-400 uppercase">Активен</th>
              <th className="px-4 py-2 text-left text-xs font-medium text-gray-400 uppercase">Создан</th>
            </tr>
          </thead>
          <tbody>
            {users.map(user => (
              <tr key={user.id} className="border-b border-gray-700">
                <td className="px-4 py-2 text-white">{user.id}</td>
                <td className="px-4 py-2 text-white">{user.lastName} {user.firstName}</td>
                <td className="px-4 py-2 text-white">{user.email}</td>
                <td className="px-4 py-2 text-white">{user.phone}</td>
                <td className="px-4 py-2 text-white">{(user.roles || []).map(r => ROLE_LABELS[r] || r).join(', ')}</td>
                <td className="px-4 py-2 text-white">{user.isActive ? 'Да' : 'Нет'}</td>
                <td className="px-4 py-2 text-white">{new Date(user.createdAt).toLocaleDateString('ru-RU')}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default UsersPage; 