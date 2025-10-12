using System.Collections.Concurrent;
using System.Linq.Expressions;
using TechLabManagement.Core.Interfaces;

namespace TechLabManagement.Services.Repositories;

public sealed class InMemoryRepository<T> : IRepository<T> where T : class, IEntity
{
	private readonly ConcurrentDictionary<Guid, T> _store = new();

	public T Add(T entity)
	{
		_store[entity.Id] = entity;
		return entity;
	}

	public bool Delete(Guid id) => _store.TryRemove(id, out _);

	public IEnumerable<T> GetAll() => _store.Values;

	public T? GetById(Guid id) => _store.TryGetValue(id, out var val) ? val : null;

	public IEnumerable<T> Query(Expression<Func<T, bool>> predicate) => _store.Values.AsQueryable().Where(predicate);

	public void Update(T entity) => _store[entity.Id] = entity;
}


