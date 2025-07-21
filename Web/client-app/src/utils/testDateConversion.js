/**
 * Тестирует конвертацию дат для формы редактирования
 */
function testDateConversion() {
  console.log('=== Тест конвертации дат для формы ===');
  
  // Тест 1: Конвертация из API в datetime-local
  const apiDate = '2024-07-10T12:00:00+03:00';
  console.log('API дата:', apiDate);
  
  try {
    const date = new Date(apiDate);
    const localDate = new Date(date.getTime() + date.getTimezoneOffset() * 60000);
    const datetimeLocal = localDate.toISOString().slice(0, 16);
    console.log('datetime-local:', datetimeLocal);
  } catch (error) {
    console.error('Ошибка конвертации из API:', error);
  }
  
  // Тест 2: Конвертация из datetime-local в UTC для отправки
  const datetimeLocalValue = '2024-07-10T15:00';
  console.log('\ndatetime-local значение:', datetimeLocalValue);
  
  try {
    const localDate = new Date(datetimeLocalValue);
    const utcDate = new Date(localDate.getTime() - localDate.getTimezoneOffset() * 60000);
    const utcISO = utcDate.toISOString();
    console.log('UTC для отправки:', utcISO);
  } catch (error) {
    console.error('Ошибка конвертации в UTC:', error);
  }
}

export { testDateConversion };
