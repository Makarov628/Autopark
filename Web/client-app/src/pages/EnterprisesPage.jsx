import { useEffect, useState } from 'react';
import Alert from '../components/ui/Alert';
import Button from '../components/ui/Button';
import Input from '../components/ui/Input';
import LoadingSpinner from '../components/ui/LoadingSpinner';
import apiService from '../services/apiService';

const EnterprisesPage = () => {
  const [enterprises, setEnterprises] = useState([]);
  const [formData, setFormData] = useState({
    name: '',
    address: '',
    phone: ''
  });
  const [editingId, setEditingId] = useState(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    fetchEnterprises();
  }, []);

  const fetchEnterprises = async () => {
    try {
      setLoading(true);
      const response = await apiService.get('/enterprises');
      if (!response.ok) {
        throw new Error('Ошибка при загрузке предприятий');
      }
      const data = await response.json();
      setEnterprises(data);
    } catch (err) {
      setError('Ошибка при загрузке предприятий');
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
        phone: ''
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
      phone: enterprise.phone
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

          <Input
            label="Телефон"
            name="phone"
            type="tel"
            value={formData.phone}
            onChange={handleInputChange}
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
                  name: '',
                  address: '',
                  phone: ''
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
          <h3 className="text-lg font-medium text-white">Список предприятий</h3>
        </div>
        <div className="overflow-x-auto">
          <table className="min-w-full">
            <thead className="bg-gray-700">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Название</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Адрес</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Телефон</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-300 uppercase tracking-wider">Действия</th>
              </tr>
            </thead>
            <tbody className="bg-gray-800 divide-y divide-gray-700">
              {enterprises.map(enterprise => (
                <tr key={enterprise.id}>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">{enterprise.name}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">{enterprise.address}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">{enterprise.phone}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => handleEdit(enterprise)}
                      className="mr-2"
                    >
                      Редактировать
                    </Button>
                    <Button
                      variant="danger"
                      size="sm"
                      onClick={() => handleDelete(enterprise.id)}
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

export default EnterprisesPage; 