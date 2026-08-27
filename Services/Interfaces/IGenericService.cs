using LibraryManagement.Models;

namespace LibraryManagement.Services.Interfaces
{
    public interface IGenericService<TEntity>
    {
        List<TEntity> GetAll();
        TEntity? GetById(int id);
        TEntity Create(TEntity entity);
        bool Update(TEntity entity);
        bool Delete(int id);
    }
}
