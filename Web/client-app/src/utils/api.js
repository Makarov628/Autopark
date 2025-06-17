// Утилита для работы с API с поддержкой CSRF токенов

// Функция для получения CSRF токена из cookie
function getCsrfToken() {
  const name = 'XSRF-TOKEN';
  const value = `; ${document.cookie}`;
  const parts = value.split(`; ${name}=`);
  if (parts.length === 2) {
    return parts.pop().split(';').shift();
  }
  return null;
}

// Функция для получения CSRF токена с сервера
async function fetchCsrfToken() {
  try {
    // Запрос к эндпоинту, который установит CSRF cookie и вернет токен
    const response = await fetch('/api/csrf/token', {
      method: 'GET',
      credentials: 'include' // Важно для получения cookie
    });
    
    if (response.ok) {
      const data = await response.json();
      // Токен автоматически сохраняется в cookie через сервер
      return data.token;
    }
  } catch (error) {
    console.warn('Не удалось получить CSRF токен:', error);
  }
  return null;
}

// Функция для выхода из системы
export async function logout() {
  try {
    // Получаем CSRF токен для logout
    const csrfToken = await fetchCsrfToken();
    
    // Выполняем logout с CSRF токеном
    const response = await fetch('/api/auth/logout', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...(csrfToken && { 'X-CSRF-TOKEN': csrfToken })
      },
      credentials: 'include'
    });

    if (response.ok) {
      console.log('Успешный выход из системы');
      return true;
    } else {
      console.warn('Ошибка при выходе:', response.status);
      return false;
    }
  } catch (error) {
    console.warn('Ошибка при выходе:', error);
    return false;
  }
}

// Основная функция для HTTP-запросов
export async function apiRequest(url, options = {}) {
  const {
    method = 'GET',
    body,
    headers = {},
    ...restOptions
  } = options;

  // Получаем CSRF токен
  let csrfToken = getCsrfToken();
  
  // Если токена нет и это не GET запрос, пытаемся его получить
  if (!csrfToken && method !== 'GET') {
    csrfToken = await fetchCsrfToken();
  }

  // Подготавливаем заголовки
  const requestHeaders = {
    'Content-Type': 'application/json',
    ...headers
  };

  // Добавляем CSRF токен в заголовки для небезопасных методов
  if (csrfToken && method !== 'GET') {
    requestHeaders['X-CSRF-TOKEN'] = csrfToken;
  }

  // Подготавливаем опции запроса
  const requestOptions = {
    method,
    headers: requestHeaders,
    credentials: 'include', // Важно для отправки cookie
    ...restOptions
  };

  // Добавляем тело запроса, если есть
  if (body) {
    requestOptions.body = JSON.stringify(body);
  }

  // Выполняем запрос
  const response = await fetch(url, requestOptions);

  // Обрабатываем ошибки авторизации
  if (response.status === 401) {
    // Перенаправляем на страницу входа или обновляем страницу
    window.location.reload();
    return response;
  }

  // Если получили 400 Bad Request и это может быть связано с CSRF,
  // пытаемся обновить токен и повторить запрос
  if (response.status === 400 && method !== 'GET') {
    const responseText = await response.text();
    if (responseText.includes('CSRF') || responseText.includes('antiforgery')) {
      console.log('CSRF токен устарел, обновляем...');
      const newCsrfToken = await fetchCsrfToken();
      
      if (newCsrfToken && newCsrfToken !== csrfToken) {
        requestHeaders['X-CSRF-TOKEN'] = newCsrfToken;
        return fetch(url, requestOptions);
      }
    }
  }

  return response;
}

// Удобные функции для разных типов запросов
export const api = {
  get: (url, options = {}) => apiRequest(url, { ...options, method: 'GET' }),
  
  post: (url, body, options = {}) => apiRequest(url, { 
    ...options, 
    method: 'POST', 
    body 
  }),
  
  put: (url, body, options = {}) => apiRequest(url, { 
    ...options, 
    method: 'PUT', 
    body 
  }),
  
  delete: (url, options = {}) => apiRequest(url, { ...options, method: 'DELETE' }),
  
  patch: (url, body, options = {}) => apiRequest(url, { 
    ...options, 
    method: 'PATCH', 
    body 
  })
};

// Инициализация CSRF токена при загрузке приложения
export async function initializeCsrf() {
  await fetchCsrfToken();
} 