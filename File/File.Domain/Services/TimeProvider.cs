using System;
using File.Domain.Services.Abstraction;

namespace File.Domain.Services
{
    public class TimeProvider : ITimeProvider
    {
        public DateTime UtcNow() => DateTime.UtcNow;
    }
}
