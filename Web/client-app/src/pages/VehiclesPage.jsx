import { useEffect, useState } from 'react';
import Alert from '../components/ui/Alert';
import Button from '../components/ui/Button';
import Input from '../components/ui/Input';
import LoadingSpinner from '../components/ui/LoadingSpinner';
import Select from '../components/ui/Select';
import apiService from '../services/apiService';

const VehiclesPage = () => {
  const [vehicles, setVehicles] = useState([]);
  const [enterprises, setEnterprises] = useState([]);
  const [brandModels, setBrandModels] = useState([]);
  const [drivers, setDrivers] = useState([]);
  const [formData, setFormData] = useState({
    name: '',
    price: '',
    mileageInKilometers: '',
    color: '',
    registrationNumber: '',
    brandModelId: '',
    enterpriseId: '',
    activeDriverId: ''
  });
  const [editingId, setEditingId] = useState(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    fetchVehicles();
    fetchEnterprises();
    fetchBrandModels();
    fetchDrivers();
  }, []);

  const fetchVehicles = async () => {
    try {
      setLoading(true);
      const response = await apiService.get('/vehicles');
      if (!response.ok) {
        throw new Error('Ошибка при загрузке транспорта');
      }
      const data = await response.json();
      setVehicles(data);
    } catch (err) {
      setError('Ошибка при загрузке транспорта');
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

  const fetchBrandModels = async () => {
    try {
      const response = await apiService.get('/brandmodels');
      if (!response.ok) {
        throw new Error('Ошибка при загрузке моделей');
      }
      const data = await response.json();
      setBrandModels(data);
    } catch (err) {
      setError('Ошибка при загрузке моделей');
    }
  };

  const fetchDrivers = async () => {
    try {
      const response = await apiService.get('/drivers');
      if (!response.ok) {
        throw new Error('Ошибка при загрузке водителей');
      }
      const data = await response.json();
      setDrivers(data);
    } catch (err) {
      setError('Ошибка при загрузке водителей');
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
      setLoading(true);
      const submitData = {
        ...formData,
        price: parseFloat(formData.price),
        mileageInKilometers: parseFloat(formData.mileageInKilometers),
        brandModelId: parseInt(formData.brandModelId),
        enterpriseId: parseInt(formData.enterpriseId),
        activeDriverId: formData.activeDriverId ? parseInt(formData.activeDriverId) : null
      };

      const url = '/vehicles';
      const method = editingId ? 'PUT' : 'POST';
      const body = editingId ? { id: editingId, ...submitData } : submitData;

      const response = method === 'POST' 
        ? await apiService.post(url, body)
        : await apiService.put(url, body);

      if (!response.ok) {
        throw new Error('Ошибка при сохранении транспорта');
      }

      setFormData({
        name: '',
        price: '',
        mileageInKilometers: '',
        color: '',
        registrationNumber: '',
        brandModelId: '',
        enterpriseId: '',
        activeDriverId: ''
      });
      setEditingId(null);
      await fetchVehicles();
    } catch (err) {
      setError('Ошибка при сохранении транспорта');
    } finally {
      setLoading(false);
    }
  };

  const handleEdit = (vehicle) => {
    setFormData({
      name: vehicle.name,
      price: vehicle.price.toString(),
      mileageInKilometers: vehicle.mileageInKilometers.toString(),
      color: vehicle.color,
      registrationNumber: vehicle.registrationNumber,
      brandModelId: vehicle.brandModelId.toString(),
      enterpriseId: vehicle.enterpriseId.toString(),
      activeDriverId: vehicle.activeDriverId?.toString() || ''
    });
    setEditingId(vehicle.id);
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Точно удалить этот транспорт?')) return;
    try {
      setLoading(true);
      const response = await apiService.delete(`/vehicles/${id}`);
      if (!response.ok) {
        throw new Error('Ошибка при удалении транспорта');
      }
      await fetchVehicles();
    } catch (err) {
      setError('Ошибка при удалении транспорта');
    } finally {
      setLoading(false);
    }
  };

  // Получаем список водителей для выбранного предприятия
  const getAvailableDrivers = () => {
    if (!formData.enterpriseId) return [];
    return drivers.filter(driver =>
      driver.enterpriseId === parseInt(formData.enterpriseId)
    );
  };

  if (loading) {
    return <LoadingSpinner size="lg" text="Загрузка транспорта..." />;
  }

  return (
    <div className="container mx-auto">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-white">Управление транспортом</h1>
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
          {editingId ? 'Редактировать транспорт' : 'Добавить транспорт'}
        </h2>
        
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <Input
            label="Название"
            name="name"
            value={formData.name}
            onChange={handleInputChange}
            required
          />

          <Input
            label="Цена"
            name="price"
            type="number"
            step="0.01"
            value={formData.price}
            onChange={handleInputChange}
            required
          />

          <Input
            label="Пробег (км)"
            name="mileageInKilometers"
            type="number"
            value={formData.mileageInKilometers}
            onChange={handleInputChange}
            required
          />

          <Input
            label="Цвет"
            name="color"
            value={formData.color}
            onChange={handleInputChange}
            required
          />

          <Input
            label="Регистрационный номер"
            name="registrationNumber"
            value={formData.registrationNumber}
            onChange={handleInputChange}
            required
          />

          <Select
            label="Модель"
            name="brandModelId"
            value={formData.brandModelId}
            onChange={handleInputChange}
            options={brandModels.map(model => ({
              value: model.id,
              label: `${model.brandName} ${model.modelName}`
            }))}
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
            label="Активный водитель"
            name="activeDriverId"
            value={formData.activeDriverId}
            onChange={handleInputChange}
            options={getAvailableDrivers().map(driver => ({
              value: driver.id,
              label: `${driver.firstName} ${driver.lastName}`
            }))}
            placeholder="Нет активного водителя"
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
                setEditingId(null);
                setFormData({
                  name: '',
                  price: '',
                  mileageInKilometers: '',
                  color: '',
                  registrationNumber: '',
                  brandModelId: '',
                  enterpriseId: '',
                  activeDriverId: ''
                });
              }}
            >
              Отмена
            </Button>
          )}
        </div>
      </form>

      <div className="bg-gray-800 rounded-lg shadow-lg overflow-hidden">
        <div className="px-6 py-4 border-b border-gray-700">
          <h3 className="text-lg font-medium text-white">Список транспорта</h3>
        </div>
        <div className="overflow-x-auto">
          <table className="min-w-full">
            <thead className="bg-gray-700">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Название</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Цена</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Пробег</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Цвет</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Номер</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Действия</th>
              </tr>
            </thead>
            <tbody className="bg-gray-800 divide-y divide-gray-700">
              {vehicles.map(vehicle => (
                <tr key={vehicle.id}>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">{vehicle.name}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">{vehicle.price}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">{vehicle.mileageInKilometers}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">{vehicle.color}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">{vehicle.registrationNumber}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => handleEdit(vehicle)}
                      className="mr-2"
                    >
                      Редактировать
                    </Button>
                    <Button
                      variant="danger"
                      size="sm"
                      onClick={() => handleDelete(vehicle.id)}
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

export default VehiclesPage; 