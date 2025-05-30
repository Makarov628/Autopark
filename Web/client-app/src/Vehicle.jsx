import { useEffect, useState } from 'react'

// Здесь мы предполагаем, что у нас есть
// interface VehicleResponse { ... brandModelId: number, ... }
// interface BrandModelResponse { id: number; brandName: string; ... }

function VehiclePage() {
  const [vehicles, setVehicles] = useState([])
  const [enterprises, setEnterprises] = useState([])
  const [brandModels, setBrandModels] = useState([])
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

  useEffect(() => {
    fetchVehicles()
    fetchEnterprises()
    fetchBrandModels()
  }, [])

  const fetchVehicles = async () => {
    try {
      const response = await fetch('/api/Vehicle/all')
      if (!response.ok) {
        throw new Error('Ошибка при загрузке транспорта')
      }
      const data = await response.json()
      setVehicles(data)
    } catch (err) {
      setError('Ошибка при загрузке транспорта')
    }
  }

  const fetchEnterprises = async () => {
    try {
      const response = await fetch('/api/Enterprise/all')
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
      const response = await fetch('/api/BrandModel/all')
      if (!response.ok) {
        throw new Error('Ошибка при загрузке моделей')
      }
      const data = await response.json()
      setBrandModels(data)
    } catch (err) {
      setError('Ошибка при загрузке моделей')
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
      const submitData = {
        ...formData,
        price: parseFloat(formData.price),
        mileageInKilometers: parseFloat(formData.mileageInKilometers),
        brandModelId: parseInt(formData.brandModelId),
        enterpriseId: parseInt(formData.enterpriseId),
        activeDriverId: formData.activeDriverId ? parseInt(formData.activeDriverId) : null
      }

      const url = '/api/Vehicle'
      const method = editingId ? 'PUT' : 'POST'
      const body = editingId ? { id: editingId, ...submitData } : submitData

      const response = await fetch(url, {
        method,
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(body)
      })

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
      fetchVehicles()
    } catch (err) {
      setError('Ошибка при сохранении транспорта')
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
      const response = await fetch(`/api/Vehicle/${id}`, {
        method: 'DELETE'
      })
      if (!response.ok) {
        throw new Error('Ошибка при удалении транспорта')
      }
      await fetchVehicles()
    } catch (err) {
      setError('Ошибка при удалении транспорта')
    }
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
              step="0.01"
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
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Цена</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Пробег</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Цвет</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Рег. номер</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Модель</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Предприятие</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Действия</th>
            </tr>
          </thead>
          <tbody className="bg-gray-800 divide-y divide-gray-700">
            {vehicles.map((vehicle) => (
              <tr key={vehicle.id}>
                <td className="px-6 py-4 whitespace-nowrap">{vehicle.id}</td>
                <td className="px-6 py-4 whitespace-nowrap">{vehicle.name}</td>
                <td className="px-6 py-4 whitespace-nowrap">{vehicle.price}</td>
                <td className="px-6 py-4 whitespace-nowrap">{vehicle.mileageInKilometers}</td>
                <td className="px-6 py-4 whitespace-nowrap">{vehicle.color}</td>
                <td className="px-6 py-4 whitespace-nowrap">{vehicle.registrationNumber}</td>
                <td className="px-6 py-4 whitespace-nowrap">
                  {brandModels.find(m => m.id === vehicle.brandModelId)?.brandName} {brandModels.find(m => m.id === vehicle.brandModelId)?.modelName}
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  {enterprises.find(e => e.id === vehicle.enterpriseId)?.name}
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <button
                    onClick={() => handleEdit(vehicle)}
                    className="text-indigo-600 hover:text-indigo-900 mr-4"
                  >
                    Редактировать
                  </button>
                  <button
                    onClick={() => handleDelete(vehicle.id)}
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
  )
}

export default VehiclePage
