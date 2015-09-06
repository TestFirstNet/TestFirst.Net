using System;

namespace TestFirst.Net.Inject
{
    public interface IInjectGuard
    {
        /// <summary>
        /// Return true if the given object should have dependencies injected
        /// </summary>
        /// <param name="obj">Object.</param>
        bool Inject(object obj);
    }
}

