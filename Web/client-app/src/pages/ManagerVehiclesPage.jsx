import React, { useEffect, useState } from 'react';
import DateTimeWithTimeZone from '../components/ui/DateTimeWithTimeZone';
import apiService from '../services/apiService';

const ManagerVehiclesPage = () => {
  const [vehicles, setVehicles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    apiService.getVehicles()
      .then(data => {
        // data.items если пагинация, иначе просто data
        setVehicles(data.items || data);
      })
      .catch(() => setError('Ошибка загрузки транспорта'))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <div>Загрузка транспорта...</div>;
  if (error) return <div className="text-red-500">{error}</div>;

  return (
    <div>
      <h2 className="text-2xl font-bold mb-4">Мой транспорт</h2>
      {vehicles.length === 0 ? (
        <p>Нет доступного транспорта.</p>
      ) : (
        <table className="min-w-full border">
          <thead>
            <tr>
              <th className="border px-2 py-1">Модель</th>
              <th className="border px-2 py-1">Госномер</th>
              <th className="border px-2 py-1">Дата покупки</th>
            </tr>
          </thead>
          <tbody>
            {vehicles.map(vehicle => (
              <tr key={vehicle.id}>
                <td className="border px-2 py-1">{vehicle.modelName || vehicle.model || '-'}</td>
                <td className="border px-2 py-1">{vehicle.registrationNumber || '-'}</td>
                <td className="border px-2 py-1">
                  <DateTimeWithTimeZone
                    isoString={vehicle.purchaseDate}
                    enterpriseTimeZone={vehicle.enterpriseTimeZoneId || vehicle.enterpriseTimeZone || vehicle.timeZoneId}
                  />
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
};

export default ManagerVehiclesPage; 