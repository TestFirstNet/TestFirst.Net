namespace TestFirst.Net.Examples.Service.Inject
{
    internal class DependencyInjector
    {
        internal IProvider<User> UserProvider { private get; set; }
        internal IProvider<Account> AccountProvider { private get; set; }

        public T Inject<T>(T instance)
        {
            if (instance is IRequireUser && UserProvider != null)
            {
                ((IRequireUser)instance).InjectedUser = UserProvider.Provide();
            }
            if (instance is IRequireAccount && AccountProvider != null)
            {
                ((IRequireAccount)instance).InjectedAccount = AccountProvider.Provide();
            }
            return instance;
        }
    }
}
