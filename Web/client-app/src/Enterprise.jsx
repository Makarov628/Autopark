import { useEffect, useState } from 'react';
import { api } from './utils/api';

const Enterprise = () => {
  const [enterprises, setEnterprises] = useState([]);
  const [vehicles, setVehicles] = useState([]);
  const [drivers, setDrivers] = useState([]);
  const [formData, setFormData] = useState({
    name: '',
    address: ''
  });
  const [editingId, setEditingId] = useState(null);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchEnterprises();
    fetchVehicles();
    fetchDrivers();
  }, []);

  const fetchEnterprises = async () => {
    try {
      setError('');
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
      console.error('Ошибка при загрузке транспорта:', err);
    }
  };

  const fetchDrivers = async () => {
    try {
      const response = await api.get('/api/Drivers');
      if (!response.ok) throw new Error('Ошибка при загрузке водителей');
      const data = await response.json();
      setDrivers(data);
    } catch (err) {
      console.error('Ошибка при загрузке водителей:', err);
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
      setError('');
      const url = '/api/Enterprises';
      const method = editingId ? 'PUT' : 'POST';
      const body = editingId ? { id: editingId, ...formData } : formData;

      const response = method === 'POST' 
        ? await api.post(url, body)
        : await api.put(url, body);

      if (!response.ok) throw new Error('Ошибка при сохранении предприятия');

      setFormData({ name: '', address: '' });
      setEditingId(null);
      fetchEnterprises();
    } catch (err) {
      setError('Ошибка при сохранении предприятия');
    }
  };

  const handleEdit = (enterprise) => {
    setFormData({
      name: enterprise.name,
      address: enterprise.address
    });
    setEditingId(enterprise.id);
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Точно удалить это предприятие?')) return;
    try {
      setError('');
      const response = await api.delete(`/api/Enterprises/${id}`);
      
      if (!response.ok) throw new Error('Ошибка при удалении предприятия');
      
      fetchEnterprises();
    } catch (err) {
      setError('Ошибка при удалении предприятия');
    }
  };

  // Получаем связанные данные для предприятия
  const getEnterpriseVehicles = (enterpriseId) => {
    return vehicles.filter(vehicle => vehicle.enterpriseId === enterpriseId);
  };

  const getEnterpriseDrivers = (enterpriseId) => {
    return drivers.filter(driver => driver.enterpriseId === enterpriseId);
  };

  return (
    <div className="container mx-auto p-4">
      <h1 className="text-2xl font-bold mb-4">Предприятия</h1>
      
      {error && <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">{error}</div>}
      
      <form onSubmit={handleSubmit} className="mb-8 bg-gray-800 p-4 rounded shadow">
        <h2 className="text-xl font-semibold mb-4">{editingId ? 'Редактировать предприятие' : 'Добавить предприятие'}</h2>
        
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div className="mb-4">
            <label className="block text-gray-200 text-sm font-bold mb-2">
              Название:
              <input
                type="text"
                name="name"
                value={formData.name}
                onChange={handleInputChange}
                className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline"
                required
              />
            </label>
          </div>

          <div className="mb-4">
            <label className="block text-gray-200 text-sm font-bold mb-2">
              Адрес:
              <input
                type="text"
                name="address"
                value={formData.address}
                onChange={handleInputChange}
                className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline"
                required
              />
            </label>
          </div>
        </div>

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
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Название</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Адрес</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Транспорт</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Водители</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Действия</th>
            </tr>
          </thead>
          <tbody className="bg-gray-800 divide-y divide-gray-700">
            {enterprises.map((enterprise) => {
              const enterpriseVehicles = getEnterpriseVehicles(enterprise.id);
              const enterpriseDrivers = getEnterpriseDrivers(enterprise.id);
              
              return (
                <tr key={enterprise.id}>
                  <td className="px-6 py-4 whitespace-nowrap">{enterprise.id}</td>
                  <td className="px-6 py-4 whitespace-nowrap">{enterprise.name}</td>
                  <td className="px-6 py-4 whitespace-nowrap">{enterprise.address}</td>
                  <td className="px-6 py-4">
                    <div className="text-sm">
                      {enterpriseVehicles.length > 0 ? (
                        <ul className="list-disc list-inside">
                          {enterpriseVehicles.slice(0, 3).map(vehicle => (
                            <li key={vehicle.id}>{vehicle.name} ({vehicle.registrationNumber})</li>
                          ))}
                          {enterpriseVehicles.length > 3 && (
                            <li className="text-gray-500">... и еще {enterpriseVehicles.length - 3}</li>
                          )}
                        </ul>
                      ) : (
                        <span className="text-gray-500">Нет транспорта</span>
                      )}
                    </div>
                  </td>
                  <td className="px-6 py-4">
                    <div className="text-sm">
                      {enterpriseDrivers.length > 0 ? (
                        <ul className="list-disc list-inside">
                          {enterpriseDrivers.slice(0, 3).map(driver => (
                            <li key={driver.id}>{driver.firstName} {driver.lastName}</li>
                          ))}
                          {enterpriseDrivers.length > 3 && (
                            <li className="text-gray-500">... и еще {enterpriseDrivers.length - 3}</li>
                          )}
                        </ul>
                      ) : (
                        <span className="text-gray-500">Нет водителей</span>
                      )}
                    </div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <button
                      onClick={() => handleEdit(enterprise)}
                      className="text-indigo-600 hover:text-indigo-900 mr-4"
                    >
                      Редактировать
                    </button>
                    <button
                      onClick={() => handleDelete(enterprise.id)}
                      className="text-red-600 hover:text-red-900"
                    >
                      Удалить
                    </button>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default Enterprise; 