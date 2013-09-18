using TestFirst.Net.Examples.Service.Model;

namespace TestFirst.Net.Examples.Service.Inject
{
    internal interface IRequireUser
    {
        User InjectedUser { set;  }
    }
}
