import { useEffect, useState } from 'react';
import Alert from '../components/ui/Alert';
import Button from '../components/ui/Button';
import Input from '../components/ui/Input';
import LoadingSpinner from '../components/ui/LoadingSpinner';
import Pagination from '../components/ui/Pagination';
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
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [sortBy, setSortBy] = useState('id');
  const [sortDirection, setSortDirection] = useState('asc');
  const [filterSearch, setFilterSearch] = useState('');

  useEffect(() => {
    fetchManagers();
    fetchEnterprises();
    fetchAvailableUsers();
  }, [page, pageSize, sortBy, sortDirection, filterSearch]);

  const fetchManagers = async () => {
    try {
      setLoading(true);
      const response = await apiService.getManagers({
        page,
        pageSize,
        sortBy,
        sortDirection,
        search: filterSearch || undefined
      });
      if (!response.ok) throw new Error('Ошибка при загрузке менеджеров');
      const responseData = await response.json();
      setManagers(responseData.items);
      setPage(responseData.page);
      setPageSize(responseData.pageSize);
      setTotalPages(responseData.totalPages);
      setTotalCount(responseData.totalCount);
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
      setEnterprises(data.items);
    } catch {
      setError('Ошибка при загрузке предприятий');
    }
  };

  const fetchAvailableUsers = async () => {
    try {
      const users = await apiService.getUsersWithoutRole('Manager');
      setAvailableUsers(users.items);
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
      <div className="bg-gray-800 p-6 rounded-lg shadow-lg mb-8">
        <div className="flex flex-col md:flex-row md:items-center md:justify-between mb-4">
          <div className="flex items-center space-x-2 mb-2 md:mb-0">
            <span className="text-gray-300">Размер страницы:</span>
            <Select
              value={pageSize}
              onChange={e => { setPageSize(Number(e.target.value)); setPage(1); }}
              options={[{ value: 10, label: '10' }, { value: 20, label: '20' }, { value: 50, label: '50' }, { value: 100, label: '100' }]}
              className="w-24"
            />
          </div>
          <div className="text-gray-300">
            Страница {page} из {totalPages} (всего {totalCount} менеджеров)
          </div>
        </div>
        <div className="flex flex-col md:flex-row md:items-end md:space-x-4 mb-4">
          <div className="mb-2 md:mb-0">
            <Input
              label="Поиск по ФИО или email"
              value={filterSearch}
              onChange={e => { setFilterSearch(e.target.value); setPage(1); }}
              className="w-56"
              placeholder="Введите ФИО или email..."
            />
          </div>
        </div>
        <div className="overflow-x-auto">
          <table className="min-w-full bg-gray-900 text-gray-200 rounded-lg">
            <thead>
              <tr>
                <th className="px-4 py-2 cursor-pointer" onClick={() => handleSort('lastName')}>Фамилия {renderSortIcon('lastName')}</th>
                <th className="px-4 py-2 cursor-pointer" onClick={() => handleSort('firstName')}>Имя {renderSortIcon('firstName')}</th>
                <th className="px-4 py-2 cursor-pointer" onClick={() => handleSort('email')}>Email {renderSortIcon('email')}</th>
                <th className="px-4 py-2">Телефон</th>
                <th className="px-4 py-2">Дата рождения</th>
                <th className="px-4 py-2">Активен</th>
              </tr>
            </thead>
            <tbody>
              {managers.map(manager => (
                <tr key={manager.id} className="border-b border-gray-700">
                  <td className="px-4 py-2">{manager.lastName}</td>
                  <td className="px-4 py-2">{manager.firstName}</td>
                  <td className="px-4 py-2">{manager.email}</td>
                  <td className="px-4 py-2">{manager.phone}</td>
                  <td className="px-4 py-2">{manager.dateOfBirth ? new Date(manager.dateOfBirth).toLocaleDateString() : ''}</td>
                  <td className="px-4 py-2">{manager.isActive ? 'Да' : 'Нет'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <div className="flex justify-between items-center mt-4">
          <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
        </div>
      </div>
    </div>
  );

  function handleSort(field) {
    if (sortBy === field) {
      setSortDirection(sortDirection === 'asc' ? 'desc' : 'asc');
    } else {
      setSortBy(field);
      setSortDirection('asc');
    }
    setPage(1);
  }

  function renderSortIcon(field) {
    if (sortBy !== field) return null;
    return sortDirection === 'asc' ? ' ▲' : ' ▼';
  }
};

export default ManagersPage; 