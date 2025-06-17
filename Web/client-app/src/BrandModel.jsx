import { useEffect, useState } from 'react'
import { api } from './utils/api'

// Enum значения для TransportType
const TRANSPORT_TYPES = {
  0: 'Легковой автомобиль',
  1: 'Грузовик',
  2: 'Автобус'
}

// Enum значения для FuelType
const FUEL_TYPES = {
  1: 'Бензин',
  2: 'Дизель',
  3: 'Электричество',
  999: 'Нет'
}

// Примерная структура BrandModel, чтобы TypeScript/JS понимал
// interface BrandModel {
//   id: number
//   brandName: string
//   modelName: string
//   transportType: number
//   fuelType: number
//   seatsNumber: number
//   maximumLoadCapacityInKillograms: number
// }

function BrandModel() {
  const [brandModels, setBrandModels] = useState([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  // Форма для добавления/редактирования
  const [formData, setFormData] = useState({
    id: 0,
    brandName: '',
    modelName: '',
    transportType: 0,
    fuelType: 1,
    seatsNumber: 0,
    maximumLoadCapacityInKillograms: 0
  })

  // Флаг: редактируем ли сущность (true) или создаём новую (false)
  const [isEditMode, setIsEditMode] = useState(false)

  // Загрузка данных при монтировании
  useEffect(() => {
    fetchAllBrandModels()
  }, [])

  const fetchAllBrandModels = async () => {
    try {
      setLoading(true)
      setError('')
      const response = await api.get('/api/BrandModels')
      if (!response.ok) {
        throw new Error('Ошибка при загрузке моделей транспорта')
      }
      const data = await response.json()
      setBrandModels(data)
    } catch (error) {
      setError('Ошибка при загрузке моделей транспорта')
      console.error(error)
    } finally {
      setLoading(false)
    }
  }

  // Создание новой записи
  const handleAdd = async (e) => {
    e.preventDefault()

    try {
      setError('')
      const response = await api.post('/api/BrandModels', {
        brandName: formData.brandName,
        modelName: formData.modelName,
        transportType: parseInt(formData.transportType),
        fuelType: parseInt(formData.fuelType),
        seatsNumber: parseInt(formData.seatsNumber),
        maximumLoadCapacityInKillograms: parseInt(formData.maximumLoadCapacityInKillograms)
      })
      
      if (!response.ok) {
        throw new Error('Ошибка при создании модели транспорта')
      }

      // Обновляем список
      await fetchAllBrandModels()

      // Сброс формы
      setFormData({
        id: 0,
        brandName: '',
        modelName: '',
        transportType: 0,
        fuelType: 1,
        seatsNumber: 0,
        maximumLoadCapacityInKillograms: 0
      })
    } catch (error) {
      setError('Ошибка при создании модели транспорта')
      console.error(error)
    }
  }

  // Удаление
  const handleDelete = async (id) => {
    if (!window.confirm('Точно удалить эту модель транспорта?')) return

    try {
      setError('')
      const response = await api.delete(`/api/BrandModels/${id}`)
      if (!response.ok) {
        throw new Error('Ошибка при удалении модели транспорта')
      }
      await fetchAllBrandModels()
    } catch (error) {
      setError('Ошибка при удалении модели транспорта')
      console.error(error)
    }
  }

  // Начать редактирование (заполнить форму данными)
  const startEdit = (model) => {
    setFormData({
      id: model.id,
      brandName: model.brandName || '',
      modelName: model.modelName || '',
      transportType: model.transportType || 0,
      fuelType: model.fuelType || 1,
      seatsNumber: model.seatsNumber || 0,
      maximumLoadCapacityInKillograms: model.maximumLoadCapacityInKillograms || 0
    })
    setIsEditMode(true)
  }

  // Сохранить изменения (PUT)
  const handleUpdate = async (e) => {
    e.preventDefault()

    try {
      setError('')
      const response = await api.put('/api/BrandModels', {
        id: formData.id,
        brandName: formData.brandName,
        modelName: formData.modelName,
        transportType: parseInt(formData.transportType),
        fuelType: parseInt(formData.fuelType),
        seatsNumber: parseInt(formData.seatsNumber),
        maximumLoadCapacityInKillograms: parseInt(formData.maximumLoadCapacityInKillograms)
      })
      
      if (!response.ok) {
        throw new Error('Ошибка при обновлении модели транспорта')
      }
      // Перечитываем список
      await fetchAllBrandModels()
      cancelEdit()
    } catch (error) {
      setError('Ошибка при обновлении модели транспорта')
      console.error(error)
    }
  }

  // Отмена редактирования
  const cancelEdit = () => {
    setIsEditMode(false)
    setFormData({
      id: 0,
      brandName: '',
      modelName: '',
      transportType: 0,
      fuelType: 1,
      seatsNumber: 0,
      maximumLoadCapacityInKillograms: 0
    })
  }

  return (
    <div className="container mx-auto p-4">
      <h1 className="text-2xl font-bold mb-4">Модели транспорта</h1>
      
      {error && <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">{error}</div>}
      {loading && <div className="bg-blue-100 border border-blue-400 text-blue-700 px-4 py-3 rounded mb-4">Загрузка...</div>}

      {/* Таблица */}
      <div className="bg-gray-800 shadow-md rounded mb-8">
        <table className="min-w-full">
          <thead>
            <tr className="bg-gray-800">
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">ID</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Марка</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Модель</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Тип транспорта</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Тип топлива</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Количество мест</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Грузоподъемность (кг)</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Действия</th>
            </tr>
          </thead>
          <tbody className="bg-gray-800 divide-y divide-gray-700">
            {brandModels.map((bm) => (
              <tr key={bm.id}>
                <td className="px-6 py-4 whitespace-nowrap">{bm.id}</td>
                <td className="px-6 py-4 whitespace-nowrap">{bm.brandName}</td>
                <td className="px-6 py-4 whitespace-nowrap">{bm.modelName}</td>
                <td className="px-6 py-4 whitespace-nowrap">{TRANSPORT_TYPES[bm.transportType] || bm.transportType}</td>
                <td className="px-6 py-4 whitespace-nowrap">{FUEL_TYPES[bm.fuelType] || bm.fuelType}</td>
                <td className="px-6 py-4 whitespace-nowrap">{bm.seatsNumber}</td>
                <td className="px-6 py-4 whitespace-nowrap">{bm.maximumLoadCapacityInKillograms}</td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <button
                    onClick={() => startEdit(bm)}
                    className="text-indigo-600 hover:text-indigo-900 mr-4"
                  >
                    Редактировать
                  </button>
                  <button
                    onClick={() => handleDelete(bm.id)}
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

      {/* Форма для создания / редактирования */}
      <div className="bg-gray-800 p-4 rounded shadow">
        <h2 className="text-xl font-semibold mb-4">
          {isEditMode ? 'Редактировать модель транспорта' : 'Добавить модель транспорта'}
        </h2>
        <form onSubmit={isEditMode ? handleUpdate : handleAdd}>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {/* brandName */}
            <div className="mb-4">
              <label className="block text-gray-200 text-sm font-bold mb-2">
                Марка:
                <input
                  type="text"
                  className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline"
                  value={formData.brandName}
                  onChange={(e) =>
                    setFormData({ ...formData, brandName: e.target.value })
                  }
                  required
                />
              </label>
            </div>

            {/* modelName */}
            <div className="mb-4">
              <label className="block text-gray-200 text-sm font-bold mb-2">
                Модель:
                <input
                  type="text"
                  className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline"
                  value={formData.modelName}
                  onChange={(e) =>
                    setFormData({ ...formData, modelName: e.target.value })
                  }
                  required
                />
              </label>
            </div>

            {/* transportType */}
            <div className="mb-4">
              <label className="block text-gray-200 text-sm font-bold mb-2">
                Тип транспорта:
                <select
                  className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline"
                  value={formData.transportType}
                  onChange={(e) =>
                    setFormData({ ...formData, transportType: e.target.value })
                  }
                  required
                >
                  <option value="0">Легковой автомобиль</option>
                  <option value="1">Грузовик</option>
                  <option value="2">Автобус</option>
                </select>
              </label>
            </div>

            {/* fuelType */}
            <div className="mb-4">
              <label className="block text-gray-200 text-sm font-bold mb-2">
                Тип топлива:
                <select
                  className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline"
                  value={formData.fuelType}
                  onChange={(e) =>
                    setFormData({ ...formData, fuelType: e.target.value })
                  }
                  required
                >
                  <option value="1">Бензин</option>
                  <option value="2">Дизель</option>
                  <option value="3">Электричество</option>
                  <option value="999">Нет</option>
                </select>
              </label>
            </div>

            {/* seatsNumber */}
            <div className="mb-4">
              <label className="block text-gray-200 text-sm font-bold mb-2">
                Количество мест:
                <input
                  type="number"
                  min="0"
                  className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline"
                  value={formData.seatsNumber}
                  onChange={(e) =>
                    setFormData({ ...formData, seatsNumber: e.target.value })
                  }
                  required
                />
              </label>
            </div>

            {/* maximumLoadCapacityInKillograms */}
            <div className="mb-4">
              <label className="block text-gray-200 text-sm font-bold mb-2">
                Грузоподъемность (кг):
                <input
                  type="number"
                  min="0"
                  className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline"
                  value={formData.maximumLoadCapacityInKillograms}
                  onChange={(e) =>
                    setFormData({
                      ...formData,
                      maximumLoadCapacityInKillograms: e.target.value
                    })
                  }
                  required
                />
              </label>
            </div>
          </div>

          {/* Buttons */}
          <div className="flex gap-2 mt-4">
            <button
              type="submit"
              className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
            >
              {isEditMode ? 'Сохранить изменения' : 'Добавить'}
            </button>
            {isEditMode && (
              <button
                type="button"
                onClick={cancelEdit}
                className="bg-gray-500 hover:bg-gray-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
              >
                Отмена
              </button>
            )}
          </div>
        </form>
      </div>
    </div>
  )
}

export default BrandModel
