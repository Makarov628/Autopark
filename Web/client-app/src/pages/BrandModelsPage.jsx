import { useCallback, useEffect, useState } from 'react';
import Alert from '../components/ui/Alert';
import Button from '../components/ui/Button';
import Input from '../components/ui/Input';
import LoadingSpinner from '../components/ui/LoadingSpinner';
import Pagination from '../components/ui/Pagination';
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
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [sortBy, setSortBy] = useState('brandName');
  const [sortDirection, setSortDirection] = useState('asc');
  const [search, setSearch] = useState('');

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

  useEffect(() => {
    fetchBrandModels();
  }, [page, pageSize, sortBy, sortDirection]);

  const fetchBrandModels = useCallback(async () => {
    try {
      setLoading(true);
      const response = await apiService.getBrandModels({
        page: page,
        pageSize: pageSize,
        sortBy,
        sortDirection,
        search: search || undefined
      });
      if (!response.ok) throw new Error('Ошибка при загрузке моделей');
      const responseData = await response.json();
      setBrandModels(responseData.items);
      setPage(responseData.page);
      setPageSize(responseData.pageSize);
      setTotalPages(responseData.totalPages);
      setTotalCount(responseData.totalCount);
    } catch (err) {
      setError('Ошибка при загрузке моделей');
    } finally {
      setLoading(false);
    }
  }, [page, pageSize, sortBy, sortDirection, search]);

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

  const handleSearchChange = (e) => {
    setSearch(e.target.value);
    setPage(1); // Сбрасываем на первую страницу при изменении поиска
  };

  const handleSearchSubmit = (e) => {
    e.preventDefault();
    fetchBrandModels();
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

      <div className="bg-gray-800 p-6 rounded-lg shadow-lg mb-8">
        {/* Поиск */}
        <form onSubmit={handleSearchSubmit} className="mb-4">
          <div className="flex gap-2">
            <Input
              placeholder="Поиск по марке или модели..."
              value={search}
              onChange={handleSearchChange}
              className="flex-1"
            />
            <Button type="submit">
              Поиск
            </Button>
          </div>
        </form>

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
            Страница {page} из {totalPages} (всего {totalCount} моделей)
          </div>
        </div>
        <div className="overflow-x-auto">
          <table className="min-w-full bg-gray-900 text-gray-200 rounded-lg">
            <thead>
              <tr>
                <th className="px-4 py-2 cursor-pointer" onClick={() => handleSort('brandName')}>Марка {renderSortIcon('brandName')}</th>
                <th className="px-4 py-2 cursor-pointer" onClick={() => handleSort('modelName')}>Модель {renderSortIcon('modelName')}</th>
                <th className="px-4 py-2">Тип топлива</th>
                <th className="px-4 py-2">Тип транспорта</th>
                <th className="px-4 py-2">Действия</th>
              </tr>
            </thead>
            <tbody>
              {brandModels?.map(model => (
                <tr key={model.id} className="border-b border-gray-700">
                  <td className="px-4 py-2">{model.brandName}</td>
                  <td className="px-4 py-2">{model.modelName}</td>
                  <td className="px-4 py-2">{getFuelTypeLabel(model.fuelType)}</td>
                  <td className="px-4 py-2">{getTransportTypeLabel(model.transportType)}</td>
                  <td className="px-4 py-2">
                    <Button size="sm" variant="secondary" onClick={() => handleEdit(model)}>Редактировать</Button>
                    <Button size="sm" variant="danger" onClick={() => handleDelete(model.id)} className="ml-2">Удалить</Button>
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

  function getFuelTypeLabel(type) {
    const found = fuelTypes.find(f => f.value === type);
    return found ? found.label : '';
  }

  function getTransportTypeLabel(type) {
    const found = transportTypes.find(t => t.value === type);
    return found ? found.label : '';
  }
};

export default BrandModelsPage; 