using System.Runtime.CompilerServices;

namespace Healthcare_Hospital_Management_System.Infrastructure
{
    public interface IXkcdClient
    {
        IAsyncEnumerable<(string? Name, byte[] Content)> GetComicImageAsync(int number1, int number2, [EnumeratorCancellation] CancellationToken cancellationToken = default);
    }
}
