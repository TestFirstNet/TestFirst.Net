using TestFirst.Net.Examples.Service.Model;

namespace TestFirst.Net.Examples.Service.Inject
{
    internal class DependencyInjector
    {
        internal IProvider<User>  UserProvider { set; private get; }
        internal IProvider<Account>  AccountProvider { set; private get; }

        public T Inject<T>(T instance)
        {
            if (instance is IRequireUser && UserProvider!= null)
            {
                ((IRequireUser) instance).InjectedUser = UserProvider.Provide();
            }
            if (instance is IRequireAccount && AccountProvider != null)
            {
                ((IRequireAccount) instance).InjectedAccount = AccountProvider.Provide();
            }
            return instance;
        }
    }
}
