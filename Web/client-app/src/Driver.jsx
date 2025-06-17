import { useEffect, useState } from 'react';
import { api } from './utils/api';

const Driver = () => {
  const [drivers, setDrivers] = useState([]);
  const [enterprises, setEnterprises] = useState([]);
  const [vehicles, setVehicles] = useState([]);
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    dateOfBirth: '',
    salary: '',
    enterpriseId: '',
    attachedVehicleId: ''
  });
  const [editingId, setEditingId] = useState(null);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchDrivers();
    fetchEnterprises();
    fetchVehicles();
  }, []);

  const fetchDrivers = async () => {
    try {
      const response = await api.get('/api/Drivers');
      if (!response.ok) throw new Error('Ошибка при загрузке водителей');
      const data = await response.json();
      setDrivers(data);
    } catch (err) {
      setError('Ошибка при загрузке водителей');
    }
  };

  const fetchEnterprises = async () => {
    try {
      const response = await api.get('/api/Enterprises');
      if (!response.ok) throw new Error('Ошибка при загрузке предприятий');
      const data = await response.json();
      setEnterprises(data);
    } catch (err) {
      setError('Ошибка при загрузке предприятий');
    }
  };

  const fetchVehicles = async () => {
    try {
      const response = await api.get('/api/Vehicles');
      if (!response.ok) throw new Error('Ошибка при загрузке транспорта');
      const data = await response.json();
      setVehicles(data);
    } catch (err) {
      setError('Ошибка при загрузке транспорта');
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const submitData = {
        ...formData,
        dateOfBirth: new Date(formData.dateOfBirth + 'T00:00:00').toISOString(),
        salary: parseFloat(formData.salary),
        enterpriseId: parseInt(formData.enterpriseId)
      };

      // Добавляем attachedVehicleId только при обновлении
      if (editingId && formData.attachedVehicleId) {
        submitData.attachedVehicleId = parseInt(formData.attachedVehicleId);
      }

      const url = '/api/Drivers';
      const method = editingId ? 'PUT' : 'POST';
      const body = editingId ? { id: editingId, ...submitData } : submitData;

      const response = method === 'POST' 
        ? await api.post(url, body)
        : await api.put(url, body);

      if (!response.ok) throw new Error('Ошибка при сохранении водителя');

      setFormData({
        firstName: '',
        lastName: '',
        dateOfBirth: '',
        salary: '',
        enterpriseId: '',
        attachedVehicleId: ''
      });
      setEditingId(null);
      fetchDrivers();
    } catch (err) {
      setError('Ошибка при сохранении водителя');
    }
  };

  const handleEdit = (driver) => {
    setFormData({
      firstName: driver.firstName,
      lastName: driver.lastName,
      dateOfBirth: new Date(driver.dateOfBirth).toISOString().split('T')[0],
      salary: driver.salary.toString(),
      enterpriseId: driver.enterpriseId.toString(),
      attachedVehicleId: driver.attachedVehicleId?.toString() || ''
    });
    setEditingId(driver.id);
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Точно удалить этого водителя?')) return;
    try {
      const response = await api.delete(`/api/Drivers/${id}`);
      
      if (!response.ok) throw new Error('Ошибка при удалении водителя');
      
      fetchDrivers();
    } catch (err) {
      setError('Ошибка при удалении водителя');
    }
  };

  // Получаем список транспорта для выбранного предприятия
  const getAvailableVehicles = () => {
    if (!formData.enterpriseId) return [];
    return vehicles.filter(vehicle =>
      vehicle.enterpriseId === parseInt(formData.enterpriseId)
    );
  };

  return (
    <div className="container mx-auto p-4">
      <h1 className="text-2xl font-bold mb-4">Водители</h1>
      
      {error && <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">{error}</div>}
      
      <form onSubmit={handleSubmit} className="mb-8 bg-gray-800 p-4 rounded shadow">
        <h2 className="text-xl font-semibold mb-4">{editingId ? 'Редактировать водителя' : 'Добавить водителя'}</h2>
        
        <div className="mb-4">
          <label className="block text-white-700 text-sm font-bold mb-2">
            Имя:
            <input
              type="text"
              name="firstName"
              value={formData.firstName}
              onChange={handleInputChange}
              className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline"
              required
            />
          </label>
        </div>

        <div className="mb-4">
          <label className="block text-white-700 text-sm font-bold mb-2">
            Фамилия:
            <input
              type="text"
              name="lastName"
              value={formData.lastName}
              onChange={handleInputChange}
              className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline"
              required
            />
          </label>
        </div>

        <div className="mb-4">
          <label className="block text-white-700 text-sm font-bold mb-2">
            Дата рождения:
            <input
              type="date"
              name="dateOfBirth"
              value={formData.dateOfBirth}
              onChange={handleInputChange}
              className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline"
              required
            />
          </label>
        </div>

        <div className="mb-4">
          <label className="block text-white-700 text-sm font-bold mb-2">
            Зарплата:
            <input
              type="number"
              step="0.01"
              name="salary"
              value={formData.salary}
              onChange={handleInputChange}
              className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline"
              required
            />
          </label>
        </div>

        <div className="mb-4">
          <label className="block text-white-700 text-sm font-bold mb-2">
            Предприятие:
            <select
              name="enterpriseId"
              value={formData.enterpriseId}
              onChange={handleInputChange}
              className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline"
              required
            >
              <option value="">Выберите предприятие</option>
              {enterprises.map(enterprise => (
                <option key={enterprise.id} value={enterprise.id}>
                  {enterprise.name}
                </option>
              ))}
            </select>
          </label>
        </div>

        {editingId && (
          <div className="mb-4">
            <label className="block text-white-700 text-sm font-bold mb-2">
              Транспорт:
              <select
                name="attachedVehicleId"
                value={formData.attachedVehicleId}
                onChange={handleInputChange}
                className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline"
              >
                <option value="">Не прикреплен</option>
                {getAvailableVehicles().map(vehicle => (
                  <option key={vehicle.id} value={vehicle.id}>
                    {vehicle.name} ({vehicle.registrationNumber})
                  </option>
                ))}
              </select>
            </label>
          </div>
        )}

        <button
          type="submit"
          className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
        >
          {editingId ? 'Сохранить изменения' : 'Добавить'}
        </button>
      </form>

      <div className="bg-gray-800 shadow-md rounded">
        <table className="min-w-full">
          <thead>
            <tr className="bg-gray-800">
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">ID</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Имя</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Фамилия</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Дата рождения</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Зарплата</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Предприятие</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Транспорт</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Действия</th>
            </tr>
          </thead>
          <tbody className="bg-gray-800 divide-y divide-gray-700">
            {drivers.map((driver) => (
              <tr key={driver.id}>
                <td className="px-6 py-4 whitespace-nowrap">{driver.id}</td>
                <td className="px-6 py-4 whitespace-nowrap">{driver.firstName}</td>
                <td className="px-6 py-4 whitespace-nowrap">{driver.lastName}</td>
                <td className="px-6 py-4 whitespace-nowrap">
                  {new Date(driver.dateOfBirth).toLocaleDateString()}
                </td>
                <td className="px-6 py-4 whitespace-nowrap">{driver.salary}</td>
                <td className="px-6 py-4 whitespace-nowrap">
                  {enterprises.find(e => e.id === driver.enterpriseId)?.name}
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  {vehicles.find(v => v.id === driver.attachedVehicleId)?.name || 'Не прикреплен'}
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <button
                    onClick={() => handleEdit(driver)}
                    className="text-indigo-600 hover:text-indigo-900 mr-4"
                  >
                    Редактировать
                  </button>
                  <button
                    onClick={() => handleDelete(driver.id)}
                    className="text-red-600 hover:text-red-900"
                  >
                    Удалить
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default Driver; 