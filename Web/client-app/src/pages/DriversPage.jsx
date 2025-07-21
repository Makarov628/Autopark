import { useEffect, useState } from 'react';
import Alert from '../components/ui/Alert';
import Button from '../components/ui/Button';
import Input from '../components/ui/Input';
import LoadingSpinner from '../components/ui/LoadingSpinner';
import Pagination from '../components/ui/Pagination';
import Select from '../components/ui/Select';
import apiService from '../services/apiService';

const DriversPage = () => {
  const [drivers, setDrivers] = useState([]);
  const [enterprises, setEnterprises] = useState([]);
  const [availableUsers, setAvailableUsers] = useState([]);
  const [vehicles, setVehicles] = useState([]);
  const [formData, setFormData] = useState({
    userId: '',
    salary: '',
    enterpriseId: '',
    attachedVehicleId: ''
  });
  const [editingId, setEditingId] = useState(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [sortBy, setSortBy] = useState('id');
  const [sortDirection, setSortDirection] = useState('asc');
  const [filterEnterpriseId, setFilterEnterpriseId] = useState('');
  const [filterSearch, setFilterSearch] = useState('');

  useEffect(() => {
    fetchDrivers();
    fetchEnterprises();
    fetchAvailableUsers();
    fetchVehicles();
  }, []);

  useEffect(() => {
    fetchDrivers();
  }, [page, pageSize, sortBy, sortDirection, filterEnterpriseId, filterSearch]);

  const fetchDrivers = async () => {
    try {
      setLoading(true);
      const response = await apiService.getDrivers({
        page,
        pageSize,
        sortBy,
        sortDirection,
        enterpriseId: filterEnterpriseId || undefined,
        search: filterSearch || undefined
      });
      if (!response.ok) throw new Error('Ошибка при загрузке водителей');
      const responseData = await response.json();
      setDrivers(responseData.items);
      setPage(responseData.page);
      setPageSize(responseData.pageSize);
      setTotalPages(responseData.totalPages);
      setTotalCount(responseData.totalCount);
    } catch (err) {
      setError('Ошибка при загрузке водителей');
    } finally {
      setLoading(false);
    }
  };

  const fetchEnterprises = async () => {
    try {
      const response = await apiService.get('/enterprises');
      if (!response.ok) {
        throw new Error('Ошибка при загрузке предприятий');
      }
      const data = await response.json();
      setEnterprises(data.items);
    } catch (err) {
      setError('Ошибка при загрузке предприятий');
    }
  };

  const fetchAvailableUsers = async () => {
    try {
      const users = await apiService.getUsersWithoutRole('Driver');
      setAvailableUsers(users.items);
    } catch {
      setError('Ошибка при загрузке пользователей');
    }
  };

  const fetchVehicles = async () => {
    try {
      const response = await apiService.get('/vehicles');
      if (!response.ok) throw new Error('Ошибка при загрузке автомобилей');
      const data = await response.json();
      setVehicles(data.items);
    } catch {
      setError('Ошибка при загрузке автомобилей');
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
    if (name === 'enterpriseId') {
      setFormData(prev => ({ ...prev, attachedVehicleId: '' }));
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      setLoading(true);
      const submitData = {
        userId: parseInt(formData.userId),
        salary: parseFloat(formData.salary),
        enterpriseId: parseInt(formData.enterpriseId),
        attachedVehicleId: formData.attachedVehicleId ? parseInt(formData.attachedVehicleId) : null
      };
      const url = '/drivers';
      const method = editingId ? 'PUT' : 'POST';
      const body = editingId ? { id: editingId, ...submitData } : submitData;
      const response = method === 'POST' 
        ? await apiService.post(url, body)
        : await apiService.put(url, body);
      if (!response.ok) {
        throw new Error('Ошибка при сохранении водителя');
      }
      setFormData({ userId: '', salary: '', enterpriseId: '', attachedVehicleId: '' });
      setEditingId(null);
      await fetchDrivers();
      await fetchAvailableUsers();
    } catch (err) {
      setError('Ошибка при сохранении водителя');
    } finally {
      setLoading(false);
    }
  };

  const handleEdit = (driver) => {
    setFormData({
      userId: driver.userId?.toString() || '',
      salary: driver.salary.toString(),
      enterpriseId: driver.enterpriseId.toString(),
      attachedVehicleId: driver.attachedVehicleId ? driver.attachedVehicleId.toString() : ''
    });
    setEditingId(driver.id);
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Точно удалить этого водителя?')) return;
    try {
      setLoading(true);
      const response = await apiService.delete(`/drivers/${id}`);
      if (!response.ok) {
        throw new Error('Ошибка при удалении водителя');
      }
      await fetchDrivers();
    } catch (err) {
      setError('Ошибка при удалении водителя');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <LoadingSpinner size="lg" text="Загрузка водителей..." />;
  }

  return (
    <div className="container mx-auto">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-white">Управление водителями</h1>
      </div>
      
      {error && (
        <Alert 
          type="error" 
          message={error} 
          onClose={() => setError('')}
          className="mb-4"
        />
      )}
      
      <form onSubmit={handleSubmit} className="mb-8 bg-gray-800 p-6 rounded-lg shadow-lg">
        <h2 className="text-xl font-semibold mb-4 text-white">
          {editingId ? 'Редактировать водителя' : 'Добавить водителя'}
        </h2>
        
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
            disabled={!!editingId}
          />
          <Input
            label="Зарплата"
            name="salary"
            type="number"
            value={formData.salary}
            onChange={handleInputChange}
            required
          />
          <Select
            label="Предприятие"
            name="enterpriseId"
            value={formData.enterpriseId}
            onChange={handleInputChange}
            options={enterprises.map(enterprise => ({
              value: enterprise.id,
              label: enterprise.name
            }))}
            required
          />
          <Select
            label="Автомобиль"
            name="attachedVehicleId"
            value={formData.attachedVehicleId}
            onChange={handleInputChange}
            options={vehicles
              .filter(v => v.enterpriseId === Number(formData.enterpriseId))
              .map(vehicle => ({
                value: vehicle.id,
                label: `${vehicle.name} (${vehicle.registrationNumber})`
              }))}
            placeholder="Без автомобиля"
            required={false}
          />
        </div>

        <div className="flex space-x-2 mt-4">
          <Button
            type="submit"
            loading={loading}
          >
            {editingId ? 'Обновить' : 'Добавить'}
          </Button>
          {editingId && (
            <Button
              type="button"
              variant="secondary"
              onClick={() => {
                setFormData({ userId: '', salary: '', enterpriseId: '', attachedVehicleId: '' });
                setEditingId(null);
              }}
            >
              Отмена
            </Button>
          )}
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
            Страница {page} из {totalPages} (всего {totalCount} водителей)
          </div>
        </div>
        <div className="flex flex-col md:flex-row md:items-end md:space-x-4 mb-4">
          <div className="mb-2 md:mb-0">
            <Select
              label="Фильтр по предприятию"
              value={filterEnterpriseId}
              onChange={e => { setFilterEnterpriseId(e.target.value); setPage(1); }}
              options={[{ value: '', label: 'Все предприятия' }, ...enterprises.map(e => ({ value: e.id, label: e.name }))]}
              className="w-56"
            />
          </div>
          <div className="mb-2 md:mb-0">
            <Input
              label="Поиск по ФИО"
              value={filterSearch}
              onChange={e => { setFilterSearch(e.target.value); setPage(1); }}
              className="w-56"
              placeholder="Введите имя или фамилию..."
            />
          </div>
        </div>
        <div className="overflow-x-auto">
          <table className="min-w-full bg-gray-900 text-gray-200 rounded-lg">
            <thead>
              <tr>
                <th className="px-4 py-2 cursor-pointer" onClick={() => handleSort('lastName')}>Фамилия {renderSortIcon('lastName')}</th>
                <th className="px-4 py-2 cursor-pointer" onClick={() => handleSort('firstName')}>Имя {renderSortIcon('firstName')}</th>
                <th className="px-4 py-2 cursor-pointer" onClick={() => handleSort('salary')}>Зарплата {renderSortIcon('salary')}</th>
                <th className="px-4 py-2">Предприятие</th>
                <th className="px-4 py-2">Машина</th>
                <th className="px-4 py-2">Действия</th>
              </tr>
            </thead>
            <tbody>
              {drivers.map(driver => (
                <tr key={driver.id} className="border-b border-gray-700">
                  <td className="px-4 py-2">{driver.lastName}</td>
                  <td className="px-4 py-2">{driver.firstName}</td>
                  <td className="px-4 py-2">{driver.salary}</td>
                  <td className="px-4 py-2">{getEnterpriseName(driver.enterpriseId)}</td>
                  <td className="px-4 py-2">{getVehicleName(driver.attachedVehicleId)}</td>
                  <td className="px-4 py-2">
                    <Button size="sm" variant="secondary" onClick={() => handleEdit(driver)}>Редактировать</Button>
                    <Button size="sm" variant="danger" onClick={() => handleDelete(driver.id)} className="ml-2">Удалить</Button>
                  </td>
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

  function getEnterpriseName(id) {
    const ent = enterprises.find(e => e.id === id);
    return ent ? ent.name : '';
  }

  function getVehicleName(id) {
    const veh = vehicles.find(v => v.id === id);
    return veh ? veh.name : '';
  }
};

export default DriversPage; 