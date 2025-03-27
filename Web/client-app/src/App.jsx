import { useEffect, useState } from 'react'
import './App.css'


function App() {
  const [vehicles, setVehicles] = useState([])
  const [loading, setLoading] = useState(false)

  // Форма для добавления/редактирования
  const [formData, setFormData] = useState({
    id: '',
    name: '',
    price: 0,
    mileageInKilometers: 0,
    color: '',
    registrationNumber: ''
  })

  // Флаг, который говорит, редактируем ли мы уже существующую запись (true)
  // или добавляем новую (false)
  const [isEditMode, setIsEditMode] = useState(false)

  // ----------------------------------------------------------------
  // Загрузка данных при первом рендере
  // ----------------------------------------------------------------
  useEffect(() => {
    fetchVehicles()
  }, [])

  const fetchVehicles = async () => {
    try {
      setLoading(true)
      const response = await fetch('/api/Vehicle/all')
      if (!response.ok) {
        throw new Error('Ошибка при загрузке списка транспортных средств')
      }
      const data = await response.json()
      setVehicles(data)
    } catch (error) {
      console.error(error)
    } finally {
      setLoading(false)
    }
  }

  // ----------------------------------------------------------------
  // Создание новой записи
  // ----------------------------------------------------------------
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
          registrationNumber: formData.registrationNumber
        })
      })

      if (!response.ok) {
        response.json().then((json) => window.alert(json.detail))
        throw new Error('Ошибка при создании записи')
      }

      // После успешного добавления – обновляем список
      await fetchVehicles()

      // Сбрасываем форму
      setFormData({
        id: '',
        name: '',
        price: 0,
        mileageInKilometers: 0,
        color: '',
        registrationNumber: ''
      })
    } catch (error) {
      console.error(error)
    }
  }

  const handleDelete = async (id) => {
    if (!window.confirm('Вы уверены, что хотите удалить этот транспорт?')) {
      return
    }
    try {
      const response = await fetch(`/api/Vehicle/${id}`, {
        method: 'DELETE'
      })
      if (!response.ok) {
        response.json().then((json) => window.alert(json.detail))
        throw new Error('Ошибка при удалении записи')
      }
      // После удаления – обновляем список
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
      registrationNumber: vehicle.registrationNumber || ''
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
          registrationNumber: formData.registrationNumber
        })
      })
      if (!response.ok) {
        response.json().then((json) => window.alert(json.detail))
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
      id: '',
      name: '',
      price: 0,
      mileageInKilometers: 0,
      color: '',
      registrationNumber: ''
    })
  }

  return (
    <div className="p-6">
      <h1 className="text-3xl font-bold mb-4">Autopark Vehicles</h1>

      {/* Загрузка данных */}
      {loading && <div className="mb-4">Загрузка...</div>}

      {/* Таблица с данными */}
      <table className="table-auto w-full border-collapse border border-black-300 mb-4">
        <thead>
          <tr className="bg-black-100">
            <th className="border border-black-300 px-4 py-2">Id</th>
            <th className="border border-black-300 px-4 py-2">Name</th>
            <th className="border border-black-300 px-4 py-2">Price</th>
            <th className="border border-black-300 px-4 py-2">Mileage</th>
            <th className="border border-black-300 px-4 py-2">Color</th>
            <th className="border border-black-300 px-4 py-2">Registration Number</th>
            <th className="border border-black-300 px-4 py-2">Actions</th>
          </tr>
        </thead>
        <tbody>
          {vehicles.map((vehicle) => (
            <tr key={vehicle.id}>
              <td className="border border-black-300 px-4 py-2">{vehicle.id}</td>
              <td className="border border-black-300 px-4 py-2">{vehicle.name}</td>
              <td className="border border-black-300 px-4 py-2">{vehicle.price}</td>
              <td className="border border-black-300 px-4 py-2">{vehicle.mileageInKilometers}</td>
              <td className="border border-black-300 px-4 py-2">{vehicle.color}</td>
              <td className="border border-black-300 px-4 py-2">{vehicle.registrationNumber}</td>
              <td className="border border-black-300 px-4 py-2">
                <button
                  className="bg-blue-500 hover:bg-blue-600 text-white py-1 px-2 rounded mr-2"
                  onClick={() => startEdit(vehicle)}
                >
                  Edit
                </button>
                <button
                  className="bg-red-500 hover:bg-red-600 text-white py-1 px-2 rounded"
                  onClick={() => handleDelete(vehicle.id)}
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
            <label className="block mb-1 font-semibold">
              Mileage (Km):
            </label>
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
            <label className="block mb-1 font-semibold">Registration Number:</label>
            <input
              type="text"
              className="border border-gray-300 rounded w-full p-1"
              value={formData.registrationNumber}
              onChange={(e) =>
                setFormData((prev) => ({ ...prev, registrationNumber: e.target.value }))
              }
            />
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

export default App
