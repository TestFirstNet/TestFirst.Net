using System;

namespace Inject
{
    public interface IRequirePostInjectNotification
    {
        void OnAfterInject();
    }
}

