using System.Linq.Expressions;

namespace AwesomeGIC.Domain.Interfaces
{
    public interface IRepository<T>
    {
        void Add(T entity);

        T GetById(int id);

        IEnumerable<T> GetAll();

        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);

        void Update(T entity);

        void Delete(T entity);
    }
}