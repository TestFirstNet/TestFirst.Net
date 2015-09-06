using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;

namespace TestFirst.Net.Inject
{
    public class InjectOnlyOnceGuard : IInjectGuard,IDisposable
    {
        private IList<object> _injected = new List<object> ();

        public bool Inject(object obj){
            if (_injected.Contains (obj)) {
                return false;
            }
            _injected.Add (obj);
            return true;
        }

        public void Dispose(){
            _injected.Clear ();
        }
    }

}

