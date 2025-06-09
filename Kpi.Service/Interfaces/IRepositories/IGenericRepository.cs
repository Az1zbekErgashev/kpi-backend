using Kpi.Domain.Commons;
using System.Linq.Expressions;

namespace Kpi.Service.Interfaces.IRepositories;
public interface IGenericRepository<T> where T : Auditable
{
    IQueryable<T> GetAll(Expression<Func<T, bool>> expression = null, string[] includes = null, bool isTracking = true);
    ValueTask<T> GetAsync(Expression<Func<T, bool>> expression, bool isTracking = true, string[] includes = null);
    ValueTask<T> CreateAsync(T entity);
    T UpdateAsync(T entity);
    ValueTask<bool> DeleteAsync(int id);
    ValueTask SaveChangesAsync();
    ValueTask DeleteRangeAsync(IEnumerable<T> entities);
}
