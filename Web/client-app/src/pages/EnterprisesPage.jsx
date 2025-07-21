import { useEffect, useState } from 'react';
import Alert from '../components/ui/Alert';
import Button from '../components/ui/Button';
import Input from '../components/ui/Input';
import LoadingSpinner from '../components/ui/LoadingSpinner';
import Pagination from '../components/ui/Pagination';
import Select from '../components/ui/Select';
import TimeZoneSelect from '../components/ui/TimeZoneSelect';
import apiService from '../services/apiService';

const EnterprisesPage = () => {
  const [enterprises, setEnterprises] = useState([]);
  const [formData, setFormData] = useState({
    name: '',
    address: '',
    phone: '',
    timeZone: ''
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
  const [filterSearch, setFilterSearch] = useState('');
  const [timeZones, setTimeZones] = useState([]);

  useEffect(() => {
    fetchEnterprises();
    fetchTimeZones();
  }, [page, pageSize, sortBy, sortDirection, filterSearch]);

  const fetchEnterprises = async () => {
    try {
      setLoading(true);
      const response = await apiService.getEnterprises({
        page,
        pageSize,
        sortBy,
        sortDirection,
        search: filterSearch || undefined
      });
      if (!response.ok) throw new Error('Ошибка при загрузке предприятий');
      const responseData = await response.json();
      setEnterprises(responseData.items);
      setPage(responseData.page);
      setPageSize(responseData.pageSize);
      setTotalPages(responseData.totalPages);
      setTotalCount(responseData.totalCount);
    } catch {
      setError('Ошибка при загрузке предприятий');
    } finally {
      setLoading(false);
    }
  };

  const fetchTimeZones = async () => {
    try {
      const response = await apiService.getTimezones();
      setTimeZones(response);
    } catch (err) {
      console.log(err)
      setError('Ошибка при загрузке таймзон');
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleTimeZoneChange = (tz) => {
    setFormData(prev => ({
      ...prev,
      timeZone: tz
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      setLoading(true);
      const url = '/enterprises';
      const method = editingId ? 'PUT' : 'POST';
      const body = editingId ? { id: editingId, ...formData } : formData;

      const response = method === 'POST' 
        ? await apiService.post(url, body)
        : await apiService.put(url, body);

      if (!response.ok) {
        throw new Error('Ошибка при сохранении предприятия');
      }

      setFormData({
        name: '',
        address: '',
        phone: '',
        timeZone: ''
      });
      setEditingId(null);
      await fetchEnterprises();
    } catch (err) {
      setError('Ошибка при сохранении предприятия');
    } finally {
      setLoading(false);
    }
  };

  const handleEdit = (enterprise) => {
    setFormData({
      name: enterprise.name,
      address: enterprise.address,
      phone: enterprise.phone,
      timeZone: enterprise.timeZoneId || ''
    });
    setEditingId(enterprise.id);
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Точно удалить это предприятие?')) return;
    try {
      setLoading(true);
      const response = await apiService.delete(`/enterprises/${id}`);
      if (!response.ok) {
        throw new Error('Ошибка при удалении предприятия');
      }
      await fetchEnterprises();
    } catch (err) {
      setError('Ошибка при удалении предприятия');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <LoadingSpinner size="lg" text="Загрузка предприятий..." />;
  }

  return (
    <div className="container mx-auto">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-white">Управление предприятиями</h1>
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
          {editingId ? 'Редактировать предприятие' : 'Добавить предприятие'}
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
            label="Адрес"
            name="address"
            value={formData.address}
            onChange={handleInputChange}
            required
          />

          {/* <Input
            label="Телефон"
            name="phone"
            type="tel"
            value={formData.phone}
            onChange={handleInputChange}
            required
          /> */}
          <TimeZoneSelect
            value={formData.timeZone}
            onChange={handleTimeZoneChange}
            label="Таймзона предприятия"
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
                  address: '',
                  phone: '',
                  timeZone: ''
                });
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
            Страница {page} из {totalPages} (всего {totalCount} предприятий)
          </div>
        </div>
        <div className="flex flex-col md:flex-row md:items-end md:space-x-4 mb-4">
          <div className="mb-2 md:mb-0">
            <Input
              label="Поиск по названию или адресу"
              value={filterSearch}
              onChange={e => { setFilterSearch(e.target.value); setPage(1); }}
              className="w-56"
              placeholder="Введите название или адрес..."
            />
          </div>
        </div>
        <div className="overflow-x-auto">
          <table className="min-w-full bg-gray-900 text-gray-200 rounded-lg">
            <thead>
              <tr>
                <th className="px-4 py-2 cursor-pointer" onClick={() => handleSort('name')}>Название {renderSortIcon('name')}</th>
                <th className="px-4 py-2 cursor-pointer" onClick={() => handleSort('address')}>Адрес {renderSortIcon('address')}</th>
                <th className="px-4 py-2">Машин</th>
                <th className="px-4 py-2">Водителей</th>
                <th className="px-4 py-2">Время</th>
                <th className="px-4 py-2">Действия</th>
              </tr>
            </thead>
            <tbody>
              {enterprises.map(ent => (
                <tr key={ent.id} className="border-b border-gray-700">
                  <td className="px-4 py-2">{ent.name}</td>
                  <td className="px-4 py-2">{ent.address}</td>
                  <td className="px-4 py-2">{ent.vehicleIds?.length || 0}</td>
                  <td className="px-4 py-2">{ent.driverIds?.length || 0}</td>
                  <td className="px-4 py-2">
                    {ent.timeZoneId
                      ? (timeZones?.find(tz => tz.id === ent.timeZoneId)?.displayName || ent.timeZoneId)
                      : 'UTC'}
                  </td>
                  <td className="px-4 py-2 space-x-2">
                    <button
                      className="text-blue-400 hover:underline mr-2"
                      onClick={() => handleEdit(ent)}
                      title="Редактировать"
                    >
                      ✏️
                    </button>
                    <button
                      className="text-red-400 hover:underline"
                      onClick={() => handleDelete(ent.id)}
                      title="Удалить"
                    >
                      🗑️
                    </button>
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
};

export default EnterprisesPage; 