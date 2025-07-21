import { useEffect, useState } from 'react';
import Alert from '../components/ui/Alert';
import Input from '../components/ui/Input';
import LoadingSpinner from '../components/ui/LoadingSpinner';
import Pagination from '../components/ui/Pagination';
import Select from '../components/ui/Select';
import apiService from '../services/apiService';

const ROLE_LABELS = {
  0: 'Админ',
  1: 'Менеджер',
  2: 'Водитель'
};

const UsersPage = () => {
  const [users, setUsers] = useState([]);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [sortBy, setSortBy] = useState('id');
  const [sortDirection, setSortDirection] = useState('asc');
  const [filterRole, setFilterRole] = useState('');
  const [filterSearch, setFilterSearch] = useState('');

  useEffect(() => {
    fetchUsers();
  }, [page, pageSize, sortBy, sortDirection, filterRole, filterSearch]);

  const fetchUsers = async () => {
    try {
      setLoading(true);
      const response = await apiService.getUsers({
        page,
        pageSize,
        sortBy,
        sortDirection,
        role: filterRole || undefined,
        search: filterSearch || undefined
      });
      if (!response.ok) throw new Error('Ошибка при загрузке пользователей');
      const responseData = await response.json();
      setUsers(responseData.items);
      setPage(responseData.page);
      setPageSize(responseData.pageSize);
      setTotalPages(responseData.totalPages);
      setTotalCount(responseData.totalCount);
    } catch {
      setError('Ошибка при загрузке пользователей');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <LoadingSpinner size="lg" text="Загрузка пользователей..." />;
  }

  return (
    <div className="container mx-auto">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-white">Пользователи</h1>
      </div>
      {error && <Alert type="error" message={error} onClose={() => setError('')} className="mb-4" />}
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
            Страница {page} из {totalPages} (всего {totalCount} пользователей)
          </div>
        </div>
        <div className="flex flex-col md:flex-row md:items-end md:space-x-4 mb-4">
          <div className="mb-2 md:mb-0">
            <Select
              label="Фильтр по роли"
              value={filterRole}
              onChange={e => { setFilterRole(e.target.value); setPage(1); }}
              options={[
                { value: '', label: 'Все роли' },
                { value: 0, label: 'Админ' },
                { value: 1, label: 'Менеджер' },
                { value: 2, label: 'Водитель' }
              ]}
              className="w-56"
            />
          </div>
          <div className="mb-2 md:mb-0">
            <Input
              label="Поиск по email или ФИО"
              value={filterSearch}
              onChange={e => { setFilterSearch(e.target.value); setPage(1); }}
              className="w-56"
              placeholder="Введите email или ФИО..."
            />
          </div>
        </div>
        <div className="overflow-x-auto">
          <table className="min-w-full bg-gray-900 text-gray-200 rounded-lg">
            <thead>
              <tr>
                <th className="px-4 py-2 cursor-pointer" onClick={() => handleSort('lastName')}>Фамилия {renderSortIcon('lastName')}</th>
                <th className="px-4 py-2 cursor-pointer" onClick={() => handleSort('firstName')}>Имя {renderSortIcon('firstName')}</th>
                <th className="px-4 py-2 cursor-pointer" onClick={() => handleSort('email')}>Email {renderSortIcon('email')}</th>
                <th className="px-4 py-2">Телефон</th>
                <th className="px-4 py-2">Роли</th>
              </tr>
            </thead>
            <tbody>
              {users.map(user => (
                <tr key={user.id} className="border-b border-gray-700">
                  <td className="px-4 py-2">{user.lastName}</td>
                  <td className="px-4 py-2">{user.firstName}</td>
                  <td className="px-4 py-2">{user.email}</td>
                  <td className="px-4 py-2">{user.phone}</td>
                  <td className="px-4 py-2">{user.roles && user.roles.map(r => ROLE_LABELS[r]).join(', ')}</td>
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

export default UsersPage; 