using System;

namespace CoreRepository
{
    public class DisposableRepository : IDisposable
    {
        #region IDisposable

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            //Dispose(true);
            // Ensure that the destructor is not called 
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}