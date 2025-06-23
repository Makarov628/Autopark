import React from 'react';

const Select = ({ 
  label, 
  error, 
  options = [], 
  placeholder = 'Выберите...',
  className = '', 
  required = false,
  ...props 
}) => {
  const selectClasses = `shadow appearance-none border rounded w-full py-2 px-3 text-gray-200 leading-tight focus:outline-none focus:shadow-outline ${
    error ? 'border-red-500' : 'border-gray-600'
  } ${className}`;
  
  return (
    <div className="mb-4">
      {label && (
        <label className="block text-white-700 text-sm font-bold mb-2">
          {label}
          {required && <span className="text-red-500 ml-1">*</span>}
        </label>
      )}
      <select
        className={selectClasses}
        required={required}
        {...props}
      >
        <option value="">{placeholder}</option>
        {options.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>
      {error && (
        <p className="text-red-500 text-xs italic mt-1">{error}</p>
      )}
    </div>
  );
};

export default Select; 