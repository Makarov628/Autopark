import React from 'react';
import authService from '../../services/authService';
import useAuthStore from '../../stores/authStore';
import Button from '../ui/Button';

const Header = () => {
  const { user, logout } = useAuthStore();

  const handleLogout = async () => {
    try {
      await authService.logout();
    } catch (error) {
      console.warn('Ошибка при выходе:', error);
    }
  };

  return (
    <header className="bg-gray-800 shadow-lg border-b border-gray-700">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-16">
          <div className="flex items-center">
            <h1 className="text-xl font-bold text-white">
              Autopark Management
            </h1>
          </div>
          
          <div className="flex items-center space-x-4">
            {user && (
              <div className="flex items-center space-x-3">
                <div className="text-sm text-gray-300">
                  <span className="font-medium">{user.firstName} {user.lastName}</span>
                  <span className="text-gray-400 ml-2">({user.email})</span>
                </div>
                <Button
                  variant="danger"
                  size="sm"
                  onClick={handleLogout}
                >
                  Выйти
                </Button>
              </div>
            )}
          </div>
        </div>
      </div>
    </header>
  );
};

export default Header; 