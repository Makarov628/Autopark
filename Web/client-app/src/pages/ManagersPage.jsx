import { useEffect, useState } from 'react';
import Alert from '../components/ui/Alert';
import Button from '../components/ui/Button';
import LoadingSpinner from '../components/ui/LoadingSpinner';
import Select from '../components/ui/Select';
import apiService from '../services/apiService';

const ManagersPage = () => {
  const [managers, setManagers] = useState([]);
  const [enterprises, setEnterprises] = useState([]);
  const [availableUsers, setAvailableUsers] = useState([]);
  const [formData, setFormData] = useState({
    userId: '',
    enterpriseIds: []
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [editingEnterprisesId, setEditingEnterprisesId] = useState(null);
  const [editingEnterpriseIds, setEditingEnterpriseIds] = useState([]);

  useEffect(() => {
    fetchManagers();
    fetchEnterprises();
    fetchAvailableUsers();
  }, []);

  const fetchManagers = async () => {
    try {
      setLoading(true);
      const response = await apiService.get('/managers');
      if (!response.ok) throw new Error('Ошибка при загрузке менеджеров');
      const data = await response.json();
      setManagers(data);
    } catch {
      setError('Ошибка при загрузке менеджеров');
    } finally {
      setLoading(false);
    }
  };

  const fetchEnterprises = async () => {
    try {
      const response = await apiService.get('/enterprises');
      if (!response.ok) throw new Error('Ошибка при загрузке предприятий');
      const data = await response.json();
      setEnterprises(data);
    } catch {
      setError('Ошибка при загрузке предприятий');
    }
  };

  const fetchAvailableUsers = async () => {
    try {
      const users = await apiService.getUsersWithoutRole('Manager');
      setAvailableUsers(users);
    } catch {
      setError('Ошибка при загрузке пользователей');
    }
  };

  const handleInputChange = (e) => {
    const { name, value, options } = e.target;
    if (name === 'enterpriseIds') {
      // Множественный select
      const selected = Array.from(options).filter(o => o.selected).map(o => o.value);
      setFormData(prev => ({ ...prev, [name]: selected }));
    } else {
      setFormData(prev => ({ ...prev, [name]: value }));
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      setLoading(true);
      const submitData = {
        userId: parseInt(formData.userId),
        enterpriseIds: formData.enterpriseIds.map(Number)
      };
      const response = await apiService.post('/managers', submitData);
      if (!response.ok) throw new Error('Ошибка при добавлении менеджера');
      setFormData({ userId: '', enterpriseIds: [] });
      await fetchManagers();
      await fetchAvailableUsers();
    } catch {
      setError('Ошибка при добавлении менеджера');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Точно удалить этого менеджера?')) return;
    try {
      setLoading(true);
      const response = await apiService.delete(`/managers/${id}`);
      if (!response.ok) throw new Error('Ошибка при удалении менеджера');
      await fetchManagers();
      await fetchAvailableUsers();
    } catch {
      setError('Ошибка при удалении менеджера');
    } finally {
      setLoading(false);
    }
  };

  const handleEditEnterprises = (manager) => {
    setEditingEnterprisesId(manager.id);
    setEditingEnterpriseIds(manager.enterpriseIds.map(String));
  };

  const handleSaveEnterprises = async (managerId) => {
    try {
      setLoading(true);
      const response = await apiService.put(`/managers/${managerId}/enterprises`, editingEnterpriseIds.map(Number));
      if (!response.ok) throw new Error('Ошибка при обновлении предприятий');
      setEditingEnterprisesId(null);
      setEditingEnterpriseIds([]);
      await fetchManagers();
    } catch {
      setError('Ошибка при обновлении предприятий');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <LoadingSpinner size="lg" text="Загрузка менеджеров..." />;
  }

  return (
    <div className="container mx-auto">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-white">Управление менеджерами</h1>
      </div>
      {error && (
        <Alert type="error" message={error} onClose={() => setError('')} className="mb-4" />
      )}
      <form onSubmit={handleSubmit} className="mb-8 bg-gray-800 p-6 rounded-lg shadow-lg">
        <h2 className="text-xl font-semibold mb-4 text-white">Назначить менеджера</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <Select
            label="Пользователь"
            name="userId"
            value={formData.userId}
            onChange={handleInputChange}
            options={availableUsers.map(user => ({
              value: user.id,
              label: `${user.lastName} ${user.firstName} (${user.email || user.phone})`
            }))}
            required
          />
          <select
            multiple
            name="enterpriseIds"
            value={formData.enterpriseIds}
            onChange={handleInputChange}
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline border-gray-600 bg-gray-900"
            required
          >
            {enterprises.map(ent => (
              <option key={ent.id} value={ent.id}>{ent.name}</option>
            ))}
          </select>
        </div>
        <div className="flex space-x-2 mt-4">
          <Button type="submit" loading={loading}>Назначить</Button>
        </div>
      </form>
      <div className="bg-gray-800 rounded-lg shadow-lg p-6">
        <h2 className="text-xl font-semibold mb-4 text-white">Список менеджеров</h2>
        <table className="min-w-full">
          <thead>
            <tr className="bg-gray-700">
              <th className="px-4 py-2 text-left text-xs font-medium text-gray-400 uppercase">ID</th>
              <th className="px-4 py-2 text-left text-xs font-medium text-gray-400 uppercase">Пользователь</th>
              <th className="px-4 py-2 text-left text-xs font-medium text-gray-400 uppercase">Предприятия</th>
              <th className="px-4 py-2 text-left text-xs font-medium text-gray-400 uppercase">Действия</th>
            </tr>
          </thead>
          <tbody>
            {managers.map(manager => (
              <tr key={manager.id} className="border-b border-gray-700">
                <td className="px-4 py-2 text-white">{manager.id}</td>
                <td className="px-4 py-2 text-white">
                  {manager.lastName} {manager.firstName} ({manager.email || manager.phone})
                </td>
                <td className="px-4 py-2 text-white">
                  {editingEnterprisesId === manager.id ? (
                    <select
                      multiple
                      value={editingEnterpriseIds}
                      onChange={e => {
                        const selected = Array.from(e.target.options).filter(o => o.selected).map(o => o.value);
                        setEditingEnterpriseIds(selected);
                      }}
                      className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline border-gray-600 bg-gray-900"
                    >
                      {enterprises.map(ent => (
                        <option key={ent.id} value={ent.id}>{ent.name}</option>
                      ))}
                    </select>
                  ) : (
                    (manager.enterpriseIds || [])
                      .map(eid => {
                        const ent = enterprises.find(e => e.id === eid);
                        return ent ? ent.name : eid;
                      })
                      .join(', ')
                  )}
                </td>
                <td className="px-4 py-2 text-white space-x-2">
                  {editingEnterprisesId === manager.id ? (
                    <>
                      <Button size="sm" onClick={() => handleSaveEnterprises(manager.id)}>
                        Сохранить
                      </Button>
                      <Button size="sm" variant="secondary" onClick={() => setEditingEnterprisesId(null)}>
                        Отмена
                      </Button>
                    </>
                  ) : (
                    <>
                      <Button size="sm" variant="secondary" onClick={() => handleEditEnterprises(manager)}>
                        Изменить предприятия
                      </Button>
                      <Button size="sm" variant="danger" onClick={() => handleDelete(manager.id)}>
                        Удалить
                      </Button>
                    </>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default ManagersPage; 