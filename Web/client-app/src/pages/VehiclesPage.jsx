import { useEffect, useState } from 'react';
import Alert from '../components/ui/Alert';
import Button from '../components/ui/Button';
import DateTimeWithTimeZone from '../components/ui/DateTimeWithTimeZone';
import Input from '../components/ui/Input';
import Pagination from '../components/ui/Pagination';
import Select from '../components/ui/Select';
import apiService from '../services/apiService';
import { testDateTimeConversion } from '../utils/testDateTimeConversion';

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
    activeDriverId: '',
    purchaseDate: ''
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
  const [filterBrandModelId, setFilterBrandModelId] = useState('');
  const [filterSearch, setFilterSearch] = useState('');

  useEffect(() => {
    fetchVehicles();
    fetchEnterprises();
    fetchBrandModels();
    fetchDrivers();
  }, []);

  useEffect(() => {
    fetchVehicles();
  }, [page, pageSize, sortBy, sortDirection, filterEnterpriseId, filterBrandModelId, filterSearch]);

  const fetchVehicles = async () => {
    try {
      setLoading(true);
      const data = await apiService.getVehicles({
        page,
        pageSize,
        sortBy,
        sortDirection,
        enterpriseId: filterEnterpriseId || undefined,
        brandModelId: filterBrandModelId || undefined,
        search: filterSearch || undefined
      });
      setVehicles(data.items);
      setPage(data.page);
      setPageSize(data.pageSize);
      setTotalPages(data.totalPages);
      setTotalCount(data.totalCount);
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
      setEnterprises(data.items);
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
      console.log(data)
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
      setDrivers(data.items);
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
      
      // Конвертируем дату в DateTimeOffset
      let purchaseDateOffset = null;
      if (formData.purchaseDate) {
        try {
          const localDate = new Date(formData.purchaseDate);
          // Создаем DateTimeOffset в UTC
          purchaseDateOffset = new Date(localDate.getTime() - localDate.getTimezoneOffset() * 60000).toISOString();
        } catch (error) {
          console.error('Ошибка конвертации даты:', error);
        }
      }

      const submitData = {
        ...formData,
        price: parseFloat(formData.price),
        mileageInKilometers: parseFloat(formData.mileageInKilometers),
        brandModelId: parseInt(formData.brandModelId),
        enterpriseId: parseInt(formData.enterpriseId),
        activeDriverId: formData.activeDriverId ? parseInt(formData.activeDriverId) : null,
        purchaseDate: purchaseDateOffset
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
        activeDriverId: '',
        purchaseDate: ''
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
    // Конвертируем ISO строку в формат datetime-local
    let purchaseDateLocal = '';
    if (vehicle.purchaseDate) {
      try {
        const date = new Date(vehicle.purchaseDate);
        // Конвертируем UTC в локальное время для datetime-local
        const localDate = new Date(date.getTime() + date.getTimezoneOffset() * 60000);
        purchaseDateLocal = localDate.toISOString().slice(0, 16);
      } catch (error) {
        console.error('Ошибка парсинга даты:', error);
      }
    }

    setFormData({
      name: vehicle.name,
      price: vehicle.price.toString(),
      mileageInKilometers: vehicle.mileageInKilometers.toString(),
      color: vehicle.color,
      registrationNumber: vehicle.registrationNumber,
      brandModelId: vehicle.brandModelId.toString(),
      enterpriseId: vehicle.enterpriseId.toString(),
      activeDriverId: vehicle.activeDriverId?.toString() || '',
      purchaseDate: purchaseDateLocal
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

  // if (loading) {
  //   return <LoadingSpinner size="lg" text="Загрузка транспорта..." />;
  // }

  return (
    <div className="container mx-auto">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-white">Управление транспортом</h1>
        <Button
          onClick={() => {
            testDateTimeConversion();
            alert('Результаты теста в консоли браузера (F12)');
          }}
          variant="secondary"
        >
          Тест конвертации дат
        </Button>
        <Button
          onClick={() => {
            testDateConversion();
            alert('Результаты теста формы в консоли браузера (F12)');
          }}
          variant="secondary"
          className="ml-2"
        >
          Тест дат формы
        </Button>
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
            options={(brandModels || []).map(model => ({
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

          <Input
            label="Дата покупки (UTC)"
            name="purchaseDate"
            type="datetime-local"
            value={formData.purchaseDate}
            onChange={handleInputChange}
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
                  activeDriverId: '',
                  purchaseDate: ''
                });
              }}
            >
              Отмена
            </Button>
          )}
        </div>
      </form>

      <div className="bg-gray-800 p-6 rounded-lg shadow-lg mb-8">
        {/* Фильтры */}
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
            <Select
              label="Фильтр по модели"
              value={filterBrandModelId}
              onChange={e => { setFilterBrandModelId(e.target.value); setPage(1); }}
              options={[{ value: '', label: 'Все модели' }, ...(brandModels?.map(bm => ({ value: bm.id, label: `${bm.brandName} ${bm.modelName}` })) || [])]}
              className="w-56"
            />
          </div>
          <div className="mb-2 md:mb-0">
            <Input
              label="Поиск (название или номер)"
              value={filterSearch}
              onChange={e => { setFilterSearch(e.target.value); setPage(1); }}
              className="w-56"
              placeholder="Введите текст..."
            />
          </div>
        </div>
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
            Страница {page} из {totalPages} (всего {totalCount} машин)
          </div>
        </div>
        <div className="overflow-x-auto">
          <table className="min-w-full bg-gray-900 text-gray-200 rounded-lg">
            <thead>
              <tr>
                <th className="px-4 py-2 cursor-pointer" onClick={() => handleSort('name')}>Название {renderSortIcon('name')}</th>
                <th className="px-4 py-2 cursor-pointer" onClick={() => handleSort('registrationNumber')}>Гос. номер {renderSortIcon('registrationNumber')}</th>
                <th className="px-4 py-2 cursor-pointer" onClick={() => handleSort('price')}>Цена {renderSortIcon('price')}</th>
                <th className="px-4 py-2">Пробег</th>
                <th className="px-4 py-2">Цвет</th>
                <th className="px-4 py-2">Модель</th>
                <th className="px-4 py-2">Предприятие</th>
                <th className="px-4 py-2">Дата покупки</th>
                <th className="px-4 py-2">Действия</th>
              </tr>
            </thead>
            <tbody>
              {vehicles.map(vehicle => (
                <tr key={vehicle.id} className="border-b border-gray-700">
                  <td className="px-4 py-2">{vehicle.name}</td>
                  <td className="px-4 py-2">{vehicle.registrationNumber}</td>
                  <td className="px-4 py-2">{vehicle.price}</td>
                  <td className="px-4 py-2">{vehicle.mileageInKilometers}</td>
                  <td className="px-4 py-2">{vehicle.color}</td>
                  <td className="px-4 py-2">{getBrandModelName(vehicle.brandModelId)}</td>
                  <td className="px-4 py-2">{getEnterpriseName(vehicle.enterpriseId)}</td>
                  <td className="px-4 py-2">
                    <DateTimeWithTimeZone
                      isoString={vehicle.purchaseDate}
                      enterpriseTimeZone={(() => {
                        const ent = enterprises.find(e => e.id === vehicle.enterpriseId);
                        return ent?.timeZoneId;
                      })()}
                    />
                    {/* Альтернативный простой способ */}
                    {/* <div className="text-xs text-gray-400 mt-1">
                      Простое: <SimpleDateTime isoString={vehicle.purchaseDate} />
                    </div> */}
                  </td>
                  <td className="px-4 py-2">
                    <Button size="sm" variant="secondary" onClick={() => handleEdit(vehicle)}>Редактировать</Button>
                    <Button size="sm" variant="danger" onClick={() => handleDelete(vehicle.id)} className="ml-2">Удалить</Button>
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

  function getBrandModelName(id) {
    const model = brandModels?.find(bm => bm.id === id);
    return model ? `${model.brandName} ${model.modelName}` : '';
  }

  function getEnterpriseName(id) {
    const ent = enterprises.find(e => e.id === id);
    return ent ? ent.name : '';
  }
};

export default VehiclesPage; 