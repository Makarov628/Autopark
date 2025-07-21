import { useEffect } from 'react';
import { Navigate, Route, BrowserRouter as Router, Routes } from 'react-router-dom';
import authService from './services/authService';
import useAuthStore from './stores/authStore';

// Layout
import Layout from './components/layout/Layout';
// Components
import InitialSetupCheck from './components/InitialSetupCheck';

// Pages
import Login from './components/Login';
import ActivatePage from './pages/ActivatePage';
import BrandModelsPage from './pages/BrandModelsPage';
import DashboardPage from './pages/DashboardPage';
import DriversPage from './pages/DriversPage';
import EnterprisesPage from './pages/EnterprisesPage';
import InitialSetupPage from './pages/InitialSetupPage';
import ManagersPage from './pages/ManagersPage';
import RegisterPage from './pages/RegisterPage';
import UsersPage from './pages/UsersPage';
import VehiclesPage from './pages/VehiclesPage';

// Protected Route Component
const ProtectedRoute = ({ children }) => {
  const { isAuthenticated, isLoading } = useAuthStore();

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-900">
        <div className="flex items-center space-x-2">
          <svg className="animate-spin h-8 w-8 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
          <span className="text-white text-xl">Загрузка...</span>
        </div>
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return children;
};

// Компонент для проверки роли менеджера
const RequireManagerRole = ({ children }) => {
  const { getUserFromToken } = useAuthStore();
  const user = getUserFromToken();
  // if (!user || !user.roles.includes('manager')) {
  //   return <Navigate to="/" replace />;
  // }
  return children;
};

function App() {
  const { isAuthenticated, logout } = useAuthStore();

  // Проверяем авторизацию при загрузке приложения
  useEffect(() => {
    const checkAuth = async () => {
      try {
        const isValid = await authService.checkAuth();
        if (!isValid) {
          logout();
        }
      } catch (error) {
        console.warn('Ошибка при проверке авторизации:', error);
        logout();
      }
    };

    checkAuth();
  }, [logout]);

  return (
    <Router>
      <div className="App">
        <InitialSetupCheck>
          <Routes>
            {/* Public routes */}
            <Route 
              path="/setup" 
              element={<InitialSetupPage />} 
            />
            <Route 
              path="/login" 
              element={
                isAuthenticated ? <Navigate to="/" replace /> : <Login />
              } 
            />
            <Route 
              path="/register" 
              element={
                isAuthenticated ? <Navigate to="/" replace /> : <RegisterPage />
              } 
            />
            <Route 
              path="/activate" 
              element={
                isAuthenticated ? <Navigate to="/" replace /> : <ActivatePage />
              } 
            />
            
            {/* Protected routes */}
            <Route
              path="/"
              element={
                <ProtectedRoute>
                  <Layout />
                </ProtectedRoute>
              }
            >
              <Route index element={<DashboardPage />} />
              <Route path="vehicles" element={<VehiclesPage />} />
              <Route path="brandmodels" element={<BrandModelsPage />} />
              <Route path="enterprises" element={<EnterprisesPage />} />
              <Route path="drivers" element={<DriversPage />} />
              <Route path="managers" element={<ManagersPage />} />
              <Route path="users" element={<UsersPage />} />
            </Route>

            {/* Manager routes */}
            {/* <Route
              path="/"
              element={
                <ProtectedRoute>
                  <RequireManagerRole>
                    <ManagerLayout />
                  </RequireManagerRole>
                </ProtectedRoute>
              }
            >
              <Route index element={<ManagerDashboardPage />} />
              <Route path="dashboard" element={<ManagerDashboardPage />} />
              <Route path="enterprises" element={<ManagerEnterprisesPage />} />
              <Route path="vehicles" element={<ManagerVehiclesPage />} />
            </Route> */}

            {/* Catch all route */}
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </InitialSetupCheck>
      </div>
    </Router>
  );
}

export default App;