namespace ComputerStore.Services
{
    public class BaseService
    {
        protected AppDbContext Context { get; private set; }
        public BaseService(AppDbContext context)
        {
            Context = context;
        }
    }
}
