using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.IRepository;
using Website.Model.Entities;

namespace Website.Repository
{
    public class WebsiteRepository<T> : IWebsiteRepository<T> where T : class
    {
        private readonly StockContext _context;
        private readonly DbSet<T> _entities;
        public WebsiteRepository()
        {
            _context = new StockContext();
            _entities = _context.Set<T>();
        }
        public WebsiteRepository(StockContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }
        public IEnumerable<T> GetAll()
        {
            return _entities.AsEnumerable();
        }
        public T GetById(int id)
        {
            return _entities.Find(id);
        }
        public int Insert(T entity)
        {
            _entities.Add(entity);
            return _context.SaveChanges();
        }
        public int Update(T entity)
        {
            _entities.Update(entity);
            return _context.SaveChanges();
        }
        public int Delete(int id)
        {
            var entity = GetById(id);
            _entities.Remove(entity);
            return _context.SaveChanges();
        }
    }
}
