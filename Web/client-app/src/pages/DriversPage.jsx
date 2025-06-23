import { useEffect, useState } from 'react';
import Alert from '../components/ui/Alert';
import Button from '../components/ui/Button';
import Input from '../components/ui/Input';
import LoadingSpinner from '../components/ui/LoadingSpinner';
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

  useEffect(() => {
    fetchDrivers();
    fetchEnterprises();
    fetchAvailableUsers();
    fetchVehicles();
  }, []);

  const fetchDrivers = async () => {
    try {
      setLoading(true);
      const response = await apiService.get('/drivers');
      if (!response.ok) {
        throw new Error('Ошибка при загрузке водителей');
      }
      const data = await response.json();
      setDrivers(data);
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
      setEnterprises(data);
    } catch (err) {
      setError('Ошибка при загрузке предприятий');
    }
  };

  const fetchAvailableUsers = async () => {
    try {
      const users = await apiService.getUsersWithoutRole('Driver');
      setAvailableUsers(users);
    } catch {
      setError('Ошибка при загрузке пользователей');
    }
  };

  const fetchVehicles = async () => {
    try {
      const response = await apiService.get('/vehicles');
      if (!response.ok) throw new Error('Ошибка при загрузке автомобилей');
      const data = await response.json();
      setVehicles(data);
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

      <div className="bg-gray-800 rounded-lg shadow-lg overflow-hidden">
        <div className="px-6 py-4 border-b border-gray-700">
          <h3 className="text-lg font-medium text-white">Список водителей</h3>
        </div>
        <div className="overflow-x-auto">
          <table className="min-w-full">
            <thead className="bg-gray-700">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">ID пользователя</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Имя</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Фамилия</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Автомобиль</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Предприятие</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Действия</th>
              </tr>
            </thead>
            <tbody className="bg-gray-800 divide-y divide-gray-700">
              {drivers.map(driver => (
                <tr key={driver.id}>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">{driver.userId}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">{driver.firstName}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">{driver.lastName}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">{
                    driver.attachedVehicleId
                      ? (vehicles.find(v => v.id === driver.attachedVehicleId)?.registrationNumber || driver.attachedVehicleId)
                      : '—'
                  }</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">
                    {enterprises.find(e => e.id === driver.enterpriseId)?.name || 'Неизвестно'}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => handleEdit(driver)}
                      className="mr-2"
                    >
                      Редактировать
                    </Button>
                    <Button
                      variant="danger"
                      size="sm"
                      onClick={() => handleDelete(driver.id)}
                    >
                      Удалить
                    </Button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};

export default DriversPage; 