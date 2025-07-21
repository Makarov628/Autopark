import React from 'react';
import { Link, Outlet } from 'react-router-dom';

const ManagerLayout = () => {
  return (
    <div className="min-h-screen flex flex-col bg-gray-50">
      <header className="bg-blue-700 text-white p-4 flex items-center justify-between">
        <div className="font-bold text-lg">Автопарк — Кабинет менеджера</div>
        <nav>
          <Link to="/manager/dashboard" className="mr-4 hover:underline">Панель</Link>
          <Link to="/manager/enterprises" className="mr-4 hover:underline">Предприятия</Link>
          <Link to="/manager/vehicles" className="mr-4 hover:underline">Транспорт</Link>
        </nav>
      </header>
      <main className="flex-1 p-6">
        <Outlet />
      </main>
    </div>
  );
};

export default ManagerLayout; 