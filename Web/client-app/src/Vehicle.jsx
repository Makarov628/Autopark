import { useEffect, useState } from 'react'

// Здесь мы предполагаем, что у нас есть
// interface VehicleResponse { ... brandModelId: number, ... }
// interface BrandModelResponse { id: number; brandName: string; ... }

function VehiclePage() {
  const [vehicles, setVehicles] = useState([])
  const [loading, setLoading] = useState(false)

  // brandModels - список для селекта
  const [brandModels, setBrandModels] = useState([])

  // Форма Vehicle
  const [formData, setFormData] = useState({
    id: 0,
    name: '',
    price: 0,
    mileageInKilometers: 0,
    color: '',
    registrationNumber: '',
    brandModelId: 0  // <-- добавляем поле brandModelId
  })

  const [isEditMode, setIsEditMode] = useState(false)

  useEffect(() => {
    fetchVehicles()
    fetchBrandModels() // <-- подтягиваем BrandModels один раз
  }, [])

  const fetchVehicles = async () => {
    try {
      setLoading(true)
      const response = await fetch('/api/Vehicle/all')
      if (!response.ok) {
        throw new Error('Ошибка при загрузке Vehicle')
      }
      const data = await response.json()
      setVehicles(data)
    } catch (error) {
      console.error(error)
    } finally {
      setLoading(false)
    }
  }

  const fetchBrandModels = async () => {
    try {
      const response = await fetch('/api/BrandModel/all')
      if (!response.ok) {
        throw new Error('Ошибка при загрузке BrandModel')
      }
      const data = await response.json()
      setBrandModels(data)
    } catch (error) {
      console.error(error)
    }
  }

  const handleAddVehicle = async (e) => {
    e.preventDefault()

    try {
      const response = await fetch('/api/Vehicle', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          name: formData.name,
          price: parseFloat(formData.price),
          mileageInKilometers: parseFloat(formData.mileageInKilometers),
          color: formData.color,
          registrationNumber: formData.registrationNumber,
          brandModelId: parseInt(formData.brandModelId, 10)
        })
      })

      if (!response.ok) {
        throw new Error('Ошибка при создании записи')
      }

      await fetchVehicles()

      // сбрасываем форму
      setFormData({
        id: 0,
        name: '',
        price: 0,
        mileageInKilometers: 0,
        color: '',
        registrationNumber: '',
        brandModelId: 0
      })
    } catch (error) {
      console.error(error)
    }
  }

  const handleDelete = async (id) => {
    if (!window.confirm('Точно удалить этот транспорт?')) return
    try {
      const response = await fetch(`/api/Vehicle/${id}`, {
        method: 'DELETE'
      })
      if (!response.ok) {
        throw new Error('Ошибка при удалении')
      }
      await fetchVehicles()
    } catch (error) {
      console.error(error)
    }
  }

  const startEdit = (vehicle) => {
    setFormData({
      id: vehicle.id,
      name: vehicle.name || '',
      price: vehicle.price || 0,
      mileageInKilometers: vehicle.mileageInKilometers || 0,
      color: vehicle.color || '',
      registrationNumber: vehicle.registrationNumber || '',
      brandModelId: vehicle.brandModelId || 0
    })
    setIsEditMode(true)
  }

  const handleUpdateVehicle = async (e) => {
    e.preventDefault()
    try {
      const response = await fetch('/api/Vehicle', {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          id: formData.id,
          name: formData.name,
          price: parseFloat(formData.price),
          mileageInKilometers: parseFloat(formData.mileageInKilometers),
          color: formData.color,
          registrationNumber: formData.registrationNumber,
          brandModelId: parseInt(formData.brandModelId, 10)
        })
      })
      if (!response.ok) {
        throw new Error('Ошибка при обновлении записи')
      }
      await fetchVehicles()
      cancelEdit()
    } catch (error) {
      console.error(error)
    }
  }

  const cancelEdit = () => {
    setIsEditMode(false)
    setFormData({
      id: 0,
      name: '',
      price: 0,
      mileageInKilometers: 0,
      color: '',
      registrationNumber: '',
      brandModelId: 0
    })
  }

  return (
    <div className="p-6">
      <h1 className="text-3xl font-bold mb-4">Autopark Vehicles</h1>

      {loading && <div className="mb-4">Загрузка...</div>}

      {/* Таблица */}
      <table className="table-auto w-full border-collapse border border-black-300 mb-4">
        <thead>
          <tr className="bg-black-100">
            <th className="border border-black-300 px-4 py-2">Id</th>
            <th className="border border-black-300 px-4 py-2">Name</th>
            <th className="border border-black-300 px-4 py-2">Price</th>
            <th className="border border-black-300 px-4 py-2">Mileage</th>
            <th className="border border-black-300 px-4 py-2">Color</th>
            <th className="border border-black-300 px-4 py-2">Registration #</th>
            <th className="border border-black-300 px-4 py-2">BrandModelId</th>
            <th className="border border-black-300 px-4 py-2">Actions</th>
          </tr>
        </thead>
        <tbody>
          {vehicles.map((v) => (
            <tr key={v.id}>
              <td className="border border-black-300 px-4 py-2">{v.id}</td>
              <td className="border border-black-300 px-4 py-2">{v.name}</td>
              <td className="border border-black-300 px-4 py-2">{v.price}</td>
              <td className="border border-black-300 px-4 py-2">{v.mileageInKilometers}</td>
              <td className="border border-black-300 px-4 py-2">{v.color}</td>
              <td className="border border-black-300 px-4 py-2">{v.registrationNumber}</td>
              <td className="border border-black-300 px-4 py-2">{v.brandModelId}</td>
              <td className="border border-black-300 px-4 py-2">
                <button
                  className="bg-blue-500 hover:bg-blue-600 text-white py-1 px-2 rounded mr-2"
                  onClick={() => startEdit(v)}
                >
                  Edit
                </button>
                <button
                  className="bg-red-500 hover:bg-red-600 text-white py-1 px-2 rounded"
                  onClick={() => handleDelete(v.id)}
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {/* Форма для создания/редактирования */}
      <div className="max-w-md p-4 border border-gray-200 rounded">
        <h2 className="text-xl font-bold mb-2">
          {isEditMode ? 'Edit Vehicle' : 'Add Vehicle'}
        </h2>
        <form onSubmit={isEditMode ? handleUpdateVehicle : handleAddVehicle}>
          {/* name */}
          <div className="mb-2">
            <label className="block mb-1 font-semibold">Name:</label>
            <input
              type="text"
              className="border border-gray-300 rounded w-full p-1"
              value={formData.name}
              onChange={(e) =>
                setFormData((prev) => ({ ...prev, name: e.target.value }))
              }
            />
          </div>

          {/* price */}
          <div className="mb-2">
            <label className="block mb-1 font-semibold">Price:</label>
            <input
              type="number"
              step="0.01"
              className="border border-gray-300 rounded w-full p-1"
              value={formData.price}
              onChange={(e) =>
                setFormData((prev) => ({ ...prev, price: e.target.value }))
              }
            />
          </div>

          {/* mileage */}
          <div className="mb-2">
            <label className="block mb-1 font-semibold">Mileage (Km):</label>
            <input
              type="number"
              className="border border-gray-300 rounded w-full p-1"
              value={formData.mileageInKilometers}
              onChange={(e) =>
                setFormData((prev) => ({
                  ...prev,
                  mileageInKilometers: e.target.value
                }))
              }
            />
          </div>

          {/* color */}
          <div className="mb-2">
            <label className="block mb-1 font-semibold">Color:</label>
            <input
              type="text"
              className="border border-gray-300 rounded w-full p-1"
              value={formData.color}
              onChange={(e) =>
                setFormData((prev) => ({ ...prev, color: e.target.value }))
              }
            />
          </div>

          {/* registrationNumber */}
          <div className="mb-2">
            <label className="block mb-1 font-semibold">
              Registration Number:
            </label>
            <input
              type="text"
              className="border border-gray-300 rounded w-full p-1"
              value={formData.registrationNumber}
              onChange={(e) =>
                setFormData((prev) => ({
                  ...prev,
                  registrationNumber: e.target.value
                }))
              }
            />
          </div>

          {/* brandModelId — селект из brandModels */}
          <div className="mb-2">
            <label className="block mb-1 font-semibold">BrandModel:</label>
            <select
              className="border border-gray-300 rounded w-full p-1"
              value={formData.brandModelId}
              onChange={(e) =>
                setFormData((prev) => ({
                  ...prev,
                  brandModelId: e.target.value
                }))
              }
            >
              <option value={0}>-- select a BrandModel --</option>
              {brandModels.map((bm) => (
                <option key={bm.id} value={bm.id}>
                  {/* Можно вывести название, например "Toyota / Corolla" */}
                  {bm.brandName} / {bm.modelName}
                </option>
              ))}
            </select>
          </div>

          {/* Buttons */}
          <div className="flex gap-2 mt-4">
            <button
              type="submit"
              className="bg-green-500 hover:bg-green-600 text-white font-semibold py-2 px-4 rounded"
            >
              {isEditMode ? 'Save Changes' : 'Add Vehicle'}
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

export default VehiclePage
