// JavaScript для кабинета менеджера

document.addEventListener('DOMContentLoaded', function() {
    // Инициализация всех компонентов
    initializeTooltips();
    initializeModals();
    initializeTables();
    initializeForms();
    
    // Анимация загрузки карточек
    animateCards();
});

// Инициализация тултипов Bootstrap
function initializeTooltips() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// Инициализация модальных окон
function initializeModals() {
    const modalTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="modal"]'));
    modalTriggerList.map(function (modalTriggerEl) {
        return new bootstrap.Modal(modalTriggerEl);
    });
}

// Инициализация таблиц с сортировкой
function initializeTables() {
    const tables = document.querySelectorAll('.table-sortable');
    tables.forEach(table => {
        const headers = table.querySelectorAll('th[data-sort]');
        headers.forEach(header => {
            header.addEventListener('click', () => {
                sortTable(table, header);
            });
        });
    });
}

// Функция сортировки таблицы
function sortTable(table, header) {
    const column = header.getAttribute('data-sort');
    const tbody = table.querySelector('tbody');
    const rows = Array.from(tbody.querySelectorAll('tr'));
    
    const isAscending = header.classList.contains('sort-asc');
    
    rows.sort((a, b) => {
        const aValue = a.querySelector(`td[data-${column}]`).textContent;
        const bValue = b.querySelector(`td[data-${column}]`).textContent;
        
        if (isAscending) {
            return bValue.localeCompare(aValue);
        } else {
            return aValue.localeCompare(bValue);
        }
    });
    
    // Обновляем классы сортировки
    table.querySelectorAll('th').forEach(th => {
        th.classList.remove('sort-asc', 'sort-desc');
    });
    
    header.classList.add(isAscending ? 'sort-desc' : 'sort-asc');
    
    // Перестраиваем таблицу
    rows.forEach(row => tbody.appendChild(row));
}

// Инициализация форм с валидацией
function initializeForms() {
    const forms = document.querySelectorAll('.needs-validation');
    forms.forEach(form => {
        form.addEventListener('submit', function(event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        });
    });
}

// Анимация карточек при загрузке
function animateCards() {
    const cards = document.querySelectorAll('.card');
    cards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        
        setTimeout(() => {
            card.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, index * 100);
    });
}

// Функция для показа уведомлений
function showNotification(message, type = 'info') {
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
    alertDiv.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    document.body.appendChild(alertDiv);
    
    // Автоматически скрываем через 5 секунд
    setTimeout(() => {
        if (alertDiv.parentNode) {
            alertDiv.remove();
        }
    }, 5000);
}

// Функция для подтверждения действий
function confirmAction(message, callback) {
    if (confirm(message)) {
        callback();
    }
}

// Функция для загрузки данных через AJAX
function loadData(url, targetElement, loadingText = 'Загрузка...') {
    const element = document.querySelector(targetElement);
    if (element) {
        element.innerHTML = `<div class="text-center"><i class="fas fa-spinner fa-spin"></i> ${loadingText}</div>`;
        
        fetch(url)
            .then(response => response.text())
            .then(html => {
                element.innerHTML = html;
            })
            .catch(error => {
                element.innerHTML = `<div class="alert alert-danger">Ошибка загрузки данных: ${error.message}</div>`;
            });
    }
}

// Функция для отправки форм через AJAX
function submitFormAjax(formElement, successCallback, errorCallback) {
    const form = document.querySelector(formElement);
    if (!form) return;
    
    const formData = new FormData(form);
    const submitButton = form.querySelector('button[type="submit"]');
    const originalText = submitButton.textContent;
    
    submitButton.disabled = true;
    submitButton.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Отправка...';
    
    fetch(form.action, {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        submitButton.disabled = false;
        submitButton.textContent = originalText;
        
        if (data.success) {
            showNotification(data.message || 'Операция выполнена успешно', 'success');
            if (successCallback) successCallback(data);
        } else {
            showNotification(data.message || 'Произошла ошибка', 'danger');
            if (errorCallback) errorCallback(data);
        }
    })
    .catch(error => {
        submitButton.disabled = false;
        submitButton.textContent = originalText;
        showNotification('Ошибка сети', 'danger');
        if (errorCallback) errorCallback(error);
    });
}

// Функция для экспорта данных в CSV
function exportToCSV(tableSelector, filename = 'export.csv') {
    const table = document.querySelector(tableSelector);
    if (!table) return;
    
    const rows = table.querySelectorAll('tr');
    let csv = [];
    
    rows.forEach(row => {
        const cols = row.querySelectorAll('td, th');
        const rowData = [];
        cols.forEach(col => {
            rowData.push(`"${col.textContent.trim()}"`);
        });
        csv.push(rowData.join(','));
    });
    
    const csvContent = csv.join('\n');
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    
    if (link.download !== undefined) {
        const url = URL.createObjectURL(blob);
        link.setAttribute('href', url);
        link.setAttribute('download', filename);
        link.style.visibility = 'hidden';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
}

// Глобальные функции для использования в Razor Pages
window.managerUtils = {
    showNotification,
    confirmAction,
    loadData,
    submitFormAjax,
    exportToCSV
}; 