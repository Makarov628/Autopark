import { DateTime } from "luxon";

/**
 * Тестирует конвертацию дат транспорта
 */
function testDateTimeConversion() {
  console.log('=== Тест конвертации дат транспорта ===');
  
  // Тестовые данные
  const testCases = [
    {
      name: 'Москва (UTC+3)',
      isoString: '2024-07-10T12:00:00+03:00',
      enterpriseTimeZone: 'Europe/Moscow'
    },
    {
      name: 'UTC',
      isoString: '2024-07-10T12:00:00Z',
      enterpriseTimeZone: null
    },
    {
      name: 'Нью-Йорк (UTC-5)',
      isoString: '2024-07-10T12:00:00-05:00',
      enterpriseTimeZone: 'America/New_York'
    },
    {
      name: 'Токио (UTC+9)',
      isoString: '2024-07-10T12:00:00+09:00',
      enterpriseTimeZone: 'Asia/Tokyo'
    }
  ];

  testCases.forEach(testCase => {
    console.log(`\n--- ${testCase.name} ---`);
    console.log(`ISO: ${testCase.isoString}`);
    
    try {
      // Парсим с сохранением зоны
      const dtFromApi = DateTime.fromISO(testCase.isoString, { setZone: true });
      console.log(`Парсинг: ${dtFromApi.toISO()}`);
      console.log(`Зона: ${dtFromApi.zoneName}`);
      
      // Конвертируем в локальную зону
      const clientZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
      const dtClient = dtFromApi.setZone(clientZone);
      console.log(`Локальное время (${clientZone}): ${dtClient.toLocaleString(DateTime.DATETIME_MED)}`);
      
      // Проверяем вашу функцию
      const result = formatVehicleDate(testCase.isoString);
      console.log(`formatVehicleDate результат: ${result}`);
      
    } catch (error) {
      console.error(`Ошибка: ${error.message}`);
    }
  });
}

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

// Экспортируем для использования
export { formatVehicleDate, testDateTimeConversion };
