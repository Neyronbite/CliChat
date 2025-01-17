using Data.Entities;

namespace Data.Repository
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        public Context _context { get; set; }

        public IAsyncRepository<User> UserRepository { get; set; }
        public IAsyncRepository<Message> MessageRepository { get; set; }

        public UnitOfWork(Context context)
        {
            _context = context;

            UserRepository = new AsyncRepository<User>(_context);
            MessageRepository = new AsyncRepository<Message>(_context);
        }

        public async void Commit()
        {
            await _context.SaveChangesAsync();
        }
        public async void Dispose()
        {
            await _context.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}
