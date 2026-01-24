using MyPortfolio.Data.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace MyPortfolio.Data.Concrete
{
    // Burada <T> dediğimiz şey: Project olabilir, About olabilir, her şey olabilir.
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly MyPortfolioContext _context;

        public GenericRepository(MyPortfolioContext context)
        {
            _context = context;
        }

        public void Delete(T t)
        {
            _context.Remove(t);
            _context.SaveChanges();
        }

        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public List<T> GetList()
        {
            return _context.Set<T>().ToList();
        }

        public void Insert(T t)
        {
            _context.Add(t);
            _context.SaveChanges();
        }

        public void Update(T t)
        {
            _context.Update(t);
            _context.SaveChanges();
        }
    }
}