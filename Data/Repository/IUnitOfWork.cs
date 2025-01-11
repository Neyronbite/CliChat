using Data.Entities;

namespace Data.Repository
{
    public interface IUnitOfWork
    {
        IAsyncRepository<User> UserRepository { get; set; }

        void Commit();
    }
}
