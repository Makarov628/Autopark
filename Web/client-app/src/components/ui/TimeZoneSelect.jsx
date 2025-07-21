import React, { useEffect, useState } from 'react';
import apiService from '../../services/apiService';

const TimeZoneSelect = ({ value, onChange, label = 'Таймзона' }) => {
  const [timeZones, setTimeZones] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    apiService.getTimezones()
      .then(data => setTimeZones(data))
      .catch(() => setError('Ошибка загрузки таймзон'))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <div>Загрузка таймзон...</div>;
  if (error) return <div className="text-red-500">{error}</div>;
  console.log("selected timezone ",value);
  return (
    <div>
      <label className="block mb-1 font-medium">{label}</label>
      <select
        className="form-select w-full p-2 border rounded"
        value={value || ''}
        onChange={e => onChange(e.target.value)}
      >
        <option value="">UTC (по умолчанию)</option>
        {timeZones?.map(tz => (
          <option key={tz.id || tz.Id} value={tz.id || tz.Id}>
            {(tz.displayName || tz.DisplayName || tz.id || tz.Id) + (tz.baseUtcOffset ? ` (${tz.baseUtcOffset})` : '')}
          </option>
        ))}
      </select>
    </div>
  );
};

export default TimeZoneSelect; 