namespace WebProject.Models
{
    public interface IRepository<TEntity>
    {
            void Add(TEntity entity);
            void Update(TEntity entity);
            void Delete(int id);
            IEnumerable<TEntity> GetAll();
            TEntity GetById(int id);
        int GetId(string userId);
    }
}
