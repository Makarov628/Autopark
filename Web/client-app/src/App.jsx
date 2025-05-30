import { useState } from 'react'

// Импортируем компоненты
import BrandModelPage from './BrandModel'
import Driver from './Driver'
import Enterprise from './Enterprise'
import VehiclePage from './Vehicle'

function App() {
  // Определяем состояние, которое говорит, что сейчас показываем
  const [activeTab, setActiveTab] = useState('vehicle')
  // Возможные значения: 'vehicle', 'brandModel', 'enterprise', 'driver'

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
          Транспорт
        </button>
        <button
          onClick={() => setActiveTab('brandModel')}
          className={`mr-2 px-4 py-2 border rounded ${
            activeTab === 'brandModel' ? 'bg-blue-500 text-white' : 'bg-black-300 text-white'
          }`}
        >
          Модели
        </button>
        <button
          onClick={() => setActiveTab('enterprise')}
          className={`mr-2 px-4 py-2 border rounded ${
            activeTab === 'enterprise' ? 'bg-blue-500 text-white' : 'bg-black-300 text-white'
          }`}
        >
          Предприятия
        </button>
        <button
          onClick={() => setActiveTab('driver')}
          className={`px-4 py-2 border rounded ${
            activeTab === 'driver' ? 'bg-blue-500 text-white' : 'bg-black-300 text-white'
          }`}
        >
          Водители
        </button>
      </div>

      {/* Рендерим компонент в зависимости от состояния */}
      {activeTab === 'vehicle' && <VehiclePage />}
      {activeTab === 'brandModel' && <BrandModelPage />}
      {activeTab === 'enterprise' && <Enterprise />}
      {activeTab === 'driver' && <Driver />}
    </div>
  )
}

export default App