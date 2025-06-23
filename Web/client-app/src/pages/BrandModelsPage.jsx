import { useEffect, useState } from 'react';
import Alert from '../components/ui/Alert';
import Button from '../components/ui/Button';
import Input from '../components/ui/Input';
import LoadingSpinner from '../components/ui/LoadingSpinner';
import Select from '../components/ui/Select';
import apiService from '../services/apiService';

const BrandModelsPage = () => {
  const [brandModels, setBrandModels] = useState([]);
  const [formData, setFormData] = useState({
    brandName: '',
    modelName: '',
    fuelType: 999,
    transportType: 0
  });
  const [editingId, setEditingId] = useState(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const fuelTypes = [
    { value: 999, label: 'Нет' },
    { value: 1, label: 'Бензин' },
    { value: 2, label: 'Дизель' },
    { value: 3, label: 'Электрический' },
  ];

  const transportTypes = [
    { value: 0, label: 'Легковой автомобиль' },
    { value: 1, label: 'Грузовик' },
    { value: 2, label: 'Автобус' },
  ];

  useEffect(() => {
    fetchBrandModels();
  }, []);

  const fetchBrandModels = async () => {
    try {
      setLoading(true);
      const response = await apiService.get('/brandmodels');
      if (!response.ok) {
        throw new Error('Ошибка при загрузке моделей');
      }
      const data = await response.json();
      setBrandModels(data);
    } catch (err) {
      setError('Ошибка при загрузке моделей');
    } finally {
      setLoading(false);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleNumberInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: parseInt(value)
    }));
  }

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      setLoading(true);
      const url = '/brandmodels';
      const method = editingId ? 'PUT' : 'POST';
      const body = editingId ? { id: editingId, ...formData } : formData;

      const response = method === 'POST' 
        ? await apiService.post(url, body)
        : await apiService.put(url, body);

      if (!response.ok) {
        throw new Error('Ошибка при сохранении модели');
      }

      setFormData({
        brandName: '',
        modelName: '',
        fuelType: 999,
        transportType: 0
      });
      setEditingId(null);
      await fetchBrandModels();
    } catch (err) {
      setError('Ошибка при сохранении модели');
    } finally {
      setLoading(false);
    }
  };

  const handleEdit = (model) => {
    setFormData({
      brandName: model.brandName,
      modelName: model.modelName,
      fuelType: model.fuelType,
      transportType: model.transportType
    });
    setEditingId(model.id);
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Точно удалить эту модель?')) return;
    try {
      setLoading(true);
      const response = await apiService.delete(`/brandmodels/${id}`);
      if (!response.ok) {
        throw new Error('Ошибка при удалении модели');
      }
      await fetchBrandModels();
    } catch (err) {
      setError('Ошибка при удалении модели');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <LoadingSpinner size="lg" text="Загрузка моделей..." />;
  }

  return (
    <div className="container mx-auto">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-white">Управление моделями</h1>
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
          {editingId ? 'Редактировать модель' : 'Добавить модель'}
        </h2>
        
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <Input
            label="Марка"
            name="brandName"
            value={formData.brandName}
            onChange={handleInputChange}
            required
          />

          <Input
            label="Модель"
            name="modelName"
            value={formData.modelName}
            onChange={handleInputChange}
            required
          />

          <Select
            label="Тип топлива"
            name="fuelType"
            value={formData.fuelType}
            onChange={handleNumberInputChange}
            options={fuelTypes}
            required
          />

          <Select
            label="Тип транспорта"
            name="transportType"
            value={formData.transportType}
            onChange={handleNumberInputChange}
            options={transportTypes}
            required
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
                  brandName: '',
                  modelName: '',
                  fuelType: '',
                  transportType: ''
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
          <h3 className="text-lg font-medium text-white">Список моделей</h3>
        </div>
        <div className="overflow-x-auto">
          <table className="min-w-full">
            <thead className="bg-gray-700">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Марка</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Модель</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Топливо</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Тип</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Действия</th>
              </tr>
            </thead>
            <tbody className="bg-gray-800 divide-y divide-gray-700">
              {brandModels.map(model => (
                <tr key={model.id}>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">{model.brandName}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">{model.modelName}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">
                    {fuelTypes.find(f => f.value === model.fuelType)?.label || model.fuelType}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">
                    {transportTypes.find(t => t.value === model.transportType)?.label || model.transportType}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => handleEdit(model)}
                      className="mr-2"
                    >
                      Редактировать
                    </Button>
                    <Button
                      variant="danger"
                      size="sm"
                      onClick={() => handleDelete(model.id)}
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

export default BrandModelsPage; 