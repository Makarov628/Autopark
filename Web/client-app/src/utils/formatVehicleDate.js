import { DateTime } from "luxon";

/**
 * Форматирует дату транспорта для отображения в локальной таймзоне пользователя.
 * @param {string} apiIso - ISO строка с смещением от сервера (например, '2024-07-10T12:00:00+03:00')
 * @returns {string} строка в формате 'dd.MM.yyyy HH:mm'
 */
function formatVehicleDate(apiIso) {
  if (!apiIso) return '-';
  
  try {
    // ISO со смещением предприятия → ZonedDateTime
    const clientZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
    return DateTime.fromISO(apiIso, { setZone: true })
        .setZone(clientZone)
        .toLocaleString(DateTime.DATETIME_MED);
  } catch (error) {
    console.error('Ошибка форматирования даты:', error, 'ISO string:', apiIso);
    return 'Ошибка даты';
  }
}

export default formatVehicleDate; 