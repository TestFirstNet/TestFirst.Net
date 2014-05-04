using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestFirst.Net.Inject
{
    public interface IClock
    {
        DateTimeOffset Now();
    }
}
