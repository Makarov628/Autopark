import React from 'react';
import { NavLink } from 'react-router-dom';

const navigation = [
  { name: 'Панель управления', href: '/', icon: '📊' },
  { name: 'Пользователи', href: '/users', icon: '🧑‍💻' },
  { name: 'Транспорт', href: '/vehicles', icon: '🚗' },
  { name: 'Модели', href: '/brandmodels', icon: '🏷️' },
  { name: 'Предприятия', href: '/enterprises', icon: '🏢' },
  { name: 'Водители', href: '/drivers', icon: '👨‍💼' },
  { name: 'Менеджеры', href: '/managers', icon: '👔' },
];

const Sidebar = () => {
  return (
    <div className="w-64 bg-gray-800 min-h-screen shadow-lg">
      <nav className="mt-5 px-2">
        <div className="space-y-1">
          {navigation.map((item) => (
            <NavLink
              key={item.name}
              to={item.href}
              className={({ isActive }) =>
                `group flex items-center px-2 py-2 text-sm font-medium rounded-md transition-colors duration-200 ${
                  isActive
                    ? 'bg-gray-900 text-white'
                    : 'text-gray-300 hover:bg-gray-700 hover:text-white'
                }`
              }
            >
              <span className="mr-3 text-lg">{item.icon}</span>
              {item.name}
            </NavLink>
          ))}
        </div>
      </nav>
    </div>
  );
};

export default Sidebar; 