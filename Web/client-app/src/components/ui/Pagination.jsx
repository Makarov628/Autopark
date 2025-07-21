import React from 'react';
import Button from './Button';

const Pagination = ({ page, totalPages, onPageChange }) => {
  if (totalPages <= 1) return null;

  // Показываем максимум 7 номеров: ... 3 4 5 6 7 ...
  const getPageNumbers = () => {
    const pages = [];
    if (totalPages <= 7) {
      for (let i = 1; i <= totalPages; i++) pages.push(i);
    } else {
      if (page <= 4) {
        pages.push(1, 2, 3, 4, 5, '...', totalPages);
      } else if (page >= totalPages - 3) {
        pages.push(1, '...', totalPages - 4, totalPages - 3, totalPages - 2, totalPages - 1, totalPages);
      } else {
        pages.push(1, '...', page - 1, page, page + 1, '...', totalPages);
      }
    }
    return pages;
  };

  return (
    <div className="flex items-center space-x-1 mt-2">
      <Button size="sm" disabled={page <= 1} onClick={() => onPageChange(page - 1)}>
        Назад
      </Button>
      {getPageNumbers().map((p, idx) =>
        p === '...'
          ? <span key={idx} className="px-2 text-gray-400">...</span>
          : <Button
              key={p}
              size="sm"
              variant={p === page ? 'primary' : 'secondary'}
              onClick={() => onPageChange(p)}
              className={p === page ? 'font-bold' : ''}
              disabled={p === page}
            >
              {p}
            </Button>
      )}
      <Button size="sm" disabled={page >= totalPages} onClick={() => onPageChange(page + 1)}>
        Вперед
      </Button>
    </div>
  );
};

export default Pagination; 