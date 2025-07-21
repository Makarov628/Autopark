import { DateTime, FixedOffsetZone } from 'luxon';
import React from 'react';

/**
 * @param {string} isoString - ISO строка даты-времени с смещением от сервера (например, '2024-07-10T12:00:00+03:00')
 * @param {string} enterpriseTimeZone - таймзона предприятия (например, 'Europe/Moscow'). Может быть пустой или undefined (тогда UTC)
 * @param {string} [format] - формат вывода (по умолчанию 'dd.MM.yyyy HH:mm')
 */
const DateTimeWithTimeZone = ({ isoString, enterpriseTimeZone, specifier, format = 'dd.MM.yyyy HH:mm' }) => {
  if (!isoString) return <span>-</span>;

  try {
    // Парсим ISO строку с сохранением зоны (setZone: true)
    const dtFromApi = !!isoString ? DateTime.fromISO(isoString, { setZone: true }) : DateTime.now().setZone(FixedOffsetZone.parseSpecifier(specifier));
    
    // Таймзона клиента
    const clientTimeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
    const dtClient = dtFromApi.setZone(clientTimeZone);
    console.log(isoString, dtFromApi.zoneName, clientTimeZone)
    return (
      <div>
        <div>
          <span className="font-semibold">Время предприятия:</span>{' '}
          {dtFromApi.toFormat(format)}{' '}
          <span className="text-xs text-gray-500">({dtFromApi.zoneName || enterpriseTimeZone || 'UTC'})</span>
        </div>
        <div>
          <span className="font-semibold">Локальное время:</span>{' '}
          {dtClient.toFormat(format)}{' '}
          <span className="text-xs text-gray-500">({clientTimeZone})</span>
        </div>
      </div>
    );
  } catch (error) {
    console.error('Ошибка парсинга даты:', error, 'ISO string:', isoString);
    return <span className="text-red-500">Ошибка даты</span>;
  }
};

export default DateTimeWithTimeZone; 