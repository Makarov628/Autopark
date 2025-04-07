import { useEffect, useState } from 'react'

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
      const response = await fetch('/api/BrandModel/all')
      if (!response.ok) {
        throw new Error('Ошибка при загрузке BrandModel')
      }
      const data = await response.json()
      setBrandModels(data)
    } catch (error) {
      console.error(error)
    } finally {
      setLoading(false)
    }
  }

  // Создание новой записи
  const handleAdd = async (e) => {
    e.preventDefault()

    try {
      const response = await fetch('/api/BrandModel', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          brandName: formData.brandName,
          modelName: formData.modelName,
          transportType: parseInt(formData.transportType),
          fuelType: parseInt(formData.fuelType),
          seatsNumber: parseInt(formData.seatsNumber),
          maximumLoadCapacityInKillograms: parseInt(formData.maximumLoadCapacityInKillograms)
        })
      })
      if (!response.ok) {
        throw new Error('Ошибка при создании BrandModel')
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
      console.error(error)
    }
  }

  // Удаление
  const handleDelete = async (id) => {
    if (!window.confirm('Точно удалить этот BrandModel?')) return

    try {
      const response = await fetch(`/api/BrandModel/${id}`, {
        method: 'DELETE'
      })
      if (!response.ok) {
        throw new Error('Ошибка при удалении BrandModel')
      }
      await fetchAllBrandModels()
    } catch (error) {
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
      const response = await fetch('/api/BrandModel', {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          id: formData.id,
          brandName: formData.brandName,
          modelName: formData.modelName,
          transportType: parseInt(formData.transportType),
          fuelType: parseInt(formData.fuelType),
          seatsNumber: parseInt(formData.seatsNumber),
          maximumLoadCapacityInKillograms: parseInt(formData.maximumLoadCapacityInKillograms)
        })
      })
      if (!response.ok) {
        throw new Error('Ошибка при обновлении BrandModel')
      }
      // Перечитываем список
      await fetchAllBrandModels()
      cancelEdit()
    } catch (error) {
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
    <div className="p-6">
      <h1 className="text-3xl font-bold mb-4">BrandModel Management</h1>

      {loading && <div>Загрузка...</div>}

      {/* Таблица */}
      <table className="table-auto w-full border-collapse border border-black-300 mb-4">
        <thead>
          <tr className="bg-black-100">
            <th className="border border-black-300 px-4 py-2">Id</th>
            <th className="border border-black-300 px-4 py-2">Brand Name</th>
            <th className="border border-black-300 px-4 py-2">Model Name</th>
            <th className="border border-black-300 px-4 py-2">Transport Type</th>
            <th className="border border-black-300 px-4 py-2">Fuel Type</th>
            <th className="border border-black-300 px-4 py-2">Seats Number</th>
            <th className="border border-black-300 px-4 py-2">Load Capacity (kg)</th>
            <th className="border border-black-300 px-4 py-2">Actions</th>
          </tr>
        </thead>
        <tbody>
          {brandModels.map((bm) => (
            <tr key={bm.id}>
              <td className="border border-black-300 px-4 py-2">{bm.id}</td>
              <td className="border border-black-300 px-4 py-2">{bm.brandName}</td>
              <td className="border border-black-300 px-4 py-2">{bm.modelName}</td>
              <td className="border border-black-300 px-4 py-2">{bm.transportType}</td>
              <td className="border border-black-300 px-4 py-2">{bm.fuelType}</td>
              <td className="border border-black-300 px-4 py-2">{bm.seatsNumber}</td>
              <td className="border border-black-300 px-4 py-2">
                {bm.maximumLoadCapacityInKillograms}
              </td>
              <td className="border border-gray-300 px-4 py-2">
                <button
                  className="bg-blue-500 hover:bg-blue-600 text-white py-1 px-2 rounded mr-2"
                  onClick={() => startEdit(bm)}
                >
                  Edit
                </button>
                <button
                  className="bg-red-500 hover:bg-red-600 text-white py-1 px-2 rounded"
                  onClick={() => handleDelete(bm.id)}
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {/* Форма для создания / редактирования */}
      <div className="max-w-md p-4 border border-gray-200 rounded">
        <h2 className="text-xl font-bold mb-2">
          {isEditMode ? 'Edit BrandModel' : 'Add BrandModel'}
        </h2>
        <form onSubmit={isEditMode ? handleUpdate : handleAdd}>
          {/* brandName */}
          <div className="mb-2">
            <label className="block mb-1 font-semibold">Brand Name:</label>
            <input
              type="text"
              className="border border-gray-300 rounded w-full p-1"
              value={formData.brandName}
              onChange={(e) =>
                setFormData({ ...formData, brandName: e.target.value })
              }
            />
          </div>

          {/* modelName */}
          <div className="mb-2">
            <label className="block mb-1 font-semibold">Model Name:</label>
            <input
              type="text"
              className="border border-gray-300 rounded w-full p-1"
              value={formData.modelName}
              onChange={(e) =>
                setFormData({ ...formData, modelName: e.target.value })
              }
            />
          </div>

          {/* transportType */}
          <div className="mb-2">
            <label className="block mb-1 font-semibold">Transport Type:</label>
            <input
              type="number"
              className="border border-gray-300 rounded w-full p-1"
              value={formData.transportType}
              onChange={(e) =>
                setFormData({ ...formData, transportType: e.target.value })
              }
            />
            <p className="text-sm text-gray-500">
              Пример: 0=Car, 1=Truck, 2=Bus
            </p>
          </div>

          {/* fuelType */}
          <div className="mb-2">
            <label className="block mb-1 font-semibold">Fuel Type:</label>
            <input
              type="number"
              className="border border-gray-300 rounded w-full p-1"
              value={formData.fuelType}
              onChange={(e) =>
                setFormData({ ...formData, fuelType: e.target.value })
              }
            />
            <p className="text-sm text-gray-500">
              Пример: 1=Gasoline, 2=Diesel, 3=Gas, 999=None
            </p>
          </div>

          {/* seatsNumber */}
          <div className="mb-2">
            <label className="block mb-1 font-semibold">Seats Number:</label>
            <input
              type="number"
              className="border border-gray-300 rounded w-full p-1"
              value={formData.seatsNumber}
              onChange={(e) =>
                setFormData({ ...formData, seatsNumber: e.target.value })
              }
            />
          </div>

          {/* maximumLoadCapacityInKillograms */}
          <div className="mb-2">
            <label className="block mb-1 font-semibold">
              Max Load Capacity (kg):
            </label>
            <input
              type="number"
              className="border border-gray-300 rounded w-full p-1"
              value={formData.maximumLoadCapacityInKillograms}
              onChange={(e) =>
                setFormData({
                  ...formData,
                  maximumLoadCapacityInKillograms: e.target.value
                })
              }
            />
          </div>

          {/* Buttons */}
          <div className="flex gap-2 mt-4">
            <button
              type="submit"
              className="bg-green-500 hover:bg-green-600 text-white font-semibold py-2 px-4 rounded"
            >
              {isEditMode ? 'Save Changes' : 'Add BrandModel'}
            </button>
            {isEditMode && (
              <button
                type="button"
                onClick={cancelEdit}
                className="bg-gray-500 hover:bg-gray-600 text-white font-semibold py-2 px-4 rounded"
              >
                Cancel
              </button>
            )}
          </div>
        </form>
      </div>
    </div>
  )
}

export default BrandModel
