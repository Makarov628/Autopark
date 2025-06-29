import { useEffect, useState } from 'react'
import apiService from './services/apiService'

// Здесь мы предполагаем, что у нас есть
// interface VehicleResponse { ... brandModelId: number, ... }
// interface BrandModelResponse { id: number; brandName: string; ... }

function VehiclePage() {
  const [vehicles, setVehicles] = useState([])
  const [enterprises, setEnterprises] = useState([])
  const [brandModels, setBrandModels] = useState([])
  const [drivers, setDrivers] = useState([])
  const [formData, setFormData] = useState({
    name: '',
    price: '',
    mileageInKilometers: '',
    color: '',
    registrationNumber: '',
    brandModelId: '',
    enterpriseId: '',
    activeDriverId: ''
  })
  const [editingId, setEditingId] = useState(null)
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    fetchVehicles()
    fetchEnterprises()
    fetchBrandModels()
    fetchDrivers()
  }, [])

  const fetchVehicles = async () => {
    try {
      setLoading(true)
      const response = await apiService.get('/vehicles')
      if (!response.ok) {
        throw new Error('Ошибка при загрузке транспорта')
      }
      const data = await response.json()
      setVehicles(data)
    } catch (err) {
      setError('Ошибка при загрузке транспорта')
    } finally {
      setLoading(false)
    }
  }

  const fetchEnterprises = async () => {
    try {
      const response = await apiService.get('/enterprises')
      if (!response.ok) {
        throw new Error('Ошибка при загрузке предприятий')
      }
      const data = await response.json()
      setEnterprises(data)
    } catch (err) {
      setError('Ошибка при загрузке предприятий')
    }
  }

  const fetchBrandModels = async () => {
    try {
      const response = await apiService.get('/brandmodels')
      if (!response.ok) {
        throw new Error('Ошибка при загрузке моделей')
      }
      const data = await response.json()
      setBrandModels(data)
    } catch (err) {
      setError('Ошибка при загрузке моделей')
    }
  }

  const fetchDrivers = async () => {
    try {
      const response = await apiService.get('/drivers')
      if (!response.ok) {
        throw new Error('Ошибка при загрузке водителей')
      }
      const data = await response.json()
      setDrivers(data)
    } catch (err) {
      setError('Ошибка при загрузке водителей')
    }
  }

  const handleInputChange = (e) => {
    const { name, value } = e.target
    setFormData(prev => ({
      ...prev,
      [name]: value
    }))
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    try {
      setLoading(true)
      const submitData = {
        ...formData,
        price: parseFloat(formData.price),
        mileageInKilometers: parseFloat(formData.mileageInKilometers),
        brandModelId: parseInt(formData.brandModelId),
        enterpriseId: parseInt(formData.enterpriseId),
        activeDriverId: formData.activeDriverId ? parseInt(formData.activeDriverId) : null
      }

      const url = '/vehicles'
      const method = editingId ? 'PUT' : 'POST'
      const body = editingId ? { id: editingId, ...submitData } : submitData

      const response = method === 'POST' 
        ? await apiService.post(url, body)
        : await apiService.put(url, body)

      if (!response.ok) {
        throw new Error('Ошибка при сохранении транспорта')
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
      })
      setEditingId(null)
      await fetchVehicles()
    } catch (err) {
      setError('Ошибка при сохранении транспорта')
    } finally {
      setLoading(false)
    }
  }

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
    })
    setEditingId(vehicle.id)
  }

  const handleDelete = async (id) => {
    if (!window.confirm('Точно удалить этот транспорт?')) return
    try {
      setLoading(true)
      const response = await apiService.delete(`/vehicles/${id}`)
      if (!response.ok) {
        throw new Error('Ошибка при удалении транспорта')
      }
      await fetchVehicles()
    } catch (err) {
      setError('Ошибка при удалении транспорта')
    } finally {
      setLoading(false)
    }
  }

  // Получаем список водителей для выбранного предприятия
  const getAvailableDrivers = () => {
    if (!formData.enterpriseId) return [];
    return drivers.filter(driver =>
      driver.enterpriseId === parseInt(formData.enterpriseId)
    );
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center p-8">
        <div className="flex items-center space-x-2">
          <svg className="animate-spin h-6 w-6 text-blue-500" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
          <span>Загрузка...</span>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto p-4">
      <h1 className="text-2xl font-bold mb-4">Транспорт</h1>
      
      {error && <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">{error}</div>}
      
      <form onSubmit={handleSubmit} className="mb-8 bg-gray-800 p-4 rounded shadow">
        <h2 className="text-xl font-semibold mb-4">{editingId ? 'Редактировать транспорт' : 'Добавить транспорт'}</h2>
        
        <div className="mb-4">
          <label className="block text-white-700 text-sm font-bold mb-2">
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
          <label className="block text-white-700 text-sm font-bold mb-2">
            Цена:
            <input
              type="number"
              step="0.01"
              name="price"
              value={formData.price}
              onChange={handleInputChange}
              className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline"
              required
            />
          </label>
        </div>

        <div className="mb-4">
          <label className="block text-white-700 text-sm font-bold mb-2">
            Пробег (км):
            <input
              type="number"
              name="mileageInKilometers"
              value={formData.mileageInKilometers}
              onChange={handleInputChange}
              className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline"
              required
            />
          </label>
        </div>

        <div className="mb-4">
          <label className="block text-white-700 text-sm font-bold mb-2">
            Цвет:
            <input
              type="text"
              name="color"
              value={formData.color}
              onChange={handleInputChange}
              className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline"
              required
            />
          </label>
        </div>

        <div className="mb-4">
          <label className="block text-white-700 text-sm font-bold mb-2">
            Регистрационный номер:
            <input
              type="text"
              name="registrationNumber"
              value={formData.registrationNumber}
              onChange={handleInputChange}
              className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline"
              required
            />
          </label>
        </div>

        <div className="mb-4">
          <label className="block text-white-700 text-sm font-bold mb-2">
            Модель:
            <select
              name="brandModelId"
              value={formData.brandModelId}
              onChange={handleInputChange}
              className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline"
              required
            >
              <option value="">Выберите модель</option>
              {brandModels.map(model => (
                <option key={model.id} value={model.id}>
                  {model.brandName} {model.modelName}
                </option>
              ))}
            </select>
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

        <div className="mb-4">
          <label className="block text-white-700 text-sm font-bold mb-2">
            Активный водитель:
            <select
              name="activeDriverId"
              value={formData.activeDriverId}
              onChange={handleInputChange}
              className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline"
            >
              <option value="">Нет активного водителя</option>
              {getAvailableDrivers().map(driver => (
                <option key={driver.id} value={driver.id}>
                  {driver.firstName} {driver.lastName}
                </option>
              ))}
            </select>
          </label>
        </div>

        <div className="flex space-x-2">
          <button
            type="submit"
            className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
            disabled={loading}
          >
            {editingId ? 'Обновить' : 'Добавить'}
          </button>
          {editingId && (
            <button
              type="button"
              onClick={() => {
                setEditingId(null)
                setFormData({
                  name: '',
                  price: '',
                  mileageInKilometers: '',
                  color: '',
                  registrationNumber: '',
                  brandModelId: '',
                  enterpriseId: '',
                  activeDriverId: ''
                })
              }}
              className="bg-gray-500 hover:bg-gray-700 text-white font-bold py-2 px-4 rounded"
            >
              Отмена
            </button>
          )}
        </div>
      </form>

      <div className="bg-gray-800 rounded shadow overflow-hidden">
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
                  <button
                    onClick={() => handleEdit(vehicle)}
                    className="text-indigo-400 hover:text-indigo-300 mr-2"
                  >
                    Редактировать
                  </button>
                  <button
                    onClick={() => handleDelete(vehicle.id)}
                    className="text-red-400 hover:text-red-300"
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
  )
}

export default VehiclePage
