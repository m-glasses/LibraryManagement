using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Services.Interfaces;

namespace LibraryManagement.Services
{
    public class GenericService<TEntity> : IGenericService<TEntity> where TEntity : BaseEntity
    {
        private readonly LibraryDbContext _context;
        public GenericService(LibraryDbContext context)
        {
            _context = context;
        }
        public TEntity Create(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            _context.Set<TEntity>().Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public bool Delete(int id)
        {
            TEntity? entity = GetById(id);
            if (entity == null)
            {
                return false;
            }

            _context.Set<TEntity>().Remove(entity);
            _context.SaveChanges();
            return true;
        }

        public List<TEntity> GetAll()
        {
            return _context.Set<TEntity>().ToList();
        }

        public TEntity? GetById(int id)
        {
            return _context.Set<TEntity>().FirstOrDefault(t => t.Id == id);
        }

        public bool Update(TEntity entity)
        {
            if (entity == null)
            {
                return false;
            }
            if (!_context.Set<TEntity>().Any(t => t.Id == entity.Id))
            {
                return false;
            }

            _context.Set<TEntity>().Update(entity);
            _context.SaveChanges();
            return true;
        }
    }
}
