using Utility;

namespace AgvcWorkFactory.Interfaces
{
    public interface IAgvcCenter : ISingletonDependency
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        void Run();

        void Stop();
    }
}