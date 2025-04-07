import { useState } from 'react'

// Импортируем ваши компоненты (VehiclePage и BrandModelPage)
import VehiclePage from './Vehicle'
import BrandModelPage from './BrandModel'

function App() {
  // Определяем состояние, которое говорит, что сейчас показываем
  const [activeTab, setActiveTab] = useState('vehicle')
  // Возможные значения: 'vehicle' или 'brandModel'

  return (
    <div className="p-6">
      <h1 className="text-3xl font-bold mb-4">Autopark Management</h1>

      {/* Кнопки-переключатели */}
      <div className="mb-4">
        <button
          onClick={() => setActiveTab('vehicle')}
          className={`mr-2 px-4 py-2 border rounded ${
            activeTab === 'vehicle' ? 'bg-blue-500 text-white' : 'bg-black-300 text-white'
          }`}
        >
          Vehicles
        </button>
        <button
          onClick={() => setActiveTab('brandModel')}
          className={`px-4 py-2 border rounded ${
            activeTab === 'brandModel' ? 'bg-blue-500 text-white' : 'bg-black-300 text-white'
          }`}
        >
          Brand Models
        </button>
      </div>

      {/* Рендерим компонент в зависимости от состояния */}
      {activeTab === 'vehicle' && <VehiclePage />}
      {activeTab === 'brandModel' && <BrandModelPage />}
    </div>
  )
}

export default App