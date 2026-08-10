namespace RescuePC.Software.EntityFrameworkCore;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
