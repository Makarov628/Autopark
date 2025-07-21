import React from 'react';
import formatVehicleDate from '../../utils/formatVehicleDate';

/**
 * Простой компонент для отображения даты транспорта в локальной таймзоне
 * @param {string} isoString - ISO строка с смещением от сервера
 */
const SimpleDateTime = ({ isoString }) => {
  if (!isoString) return <span>-</span>;
  
  return (
    <span title={`ISO: ${isoString}`}>
      {formatVehicleDate(isoString)}
    </span>
  );
};

export default SimpleDateTime; 