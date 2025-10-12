using System.Linq.Expressions;

namespace TechLabManagement.Core.Interfaces;

public interface IRepository<T> where T : class, IEntity
{
	T Add(T entity);
	void Update(T entity);
	bool Delete(Guid id);
	T? GetById(Guid id);
	IEnumerable<T> GetAll();
	IEnumerable<T> Query(Expression<Func<T, bool>> predicate);
}


