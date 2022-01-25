using System;

namespace File.Domain.Services.Abstraction
{
    public interface ITimeProvider
    {
        DateTime UtcNow();
    }
}
