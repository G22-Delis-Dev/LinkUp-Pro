using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LinkUpPro.Domain.Interfaces.Repositories;

public interface IGenericRepository<T, TId> where T : class
{
    // Operaciones básicas CRUD
    Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    // Operaciones de consulta
    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate, 
        CancellationToken cancellationToken = default);

    Task<T?> FindOneAsync(
        Expression<Func<T, bool>> predicate, 
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate, 
        CancellationToken cancellationToken = default);

    // Query para consultas complejas
    IQueryable<T> Query();
}