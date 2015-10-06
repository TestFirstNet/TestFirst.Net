using System;
using System.Threading;

namespace TestFirst.Net.Extensions.Moq
{
    internal class RaisedEvent
    {
        private static readonly ThreadLocal<RaisedEvent> NextRaisedEvent = new ThreadLocal<RaisedEvent>();

        internal RaisedEvent(object sender, EventArgs args)
        {
            Sender = sender;
            Args = args;
        }

        public object Sender { get; private set; }
        public EventArgs Args { get; private set; }

        internal static RaisedEvent Next
        {
            get { return NextRaisedEvent.Value; }
            set { NextRaisedEvent.Value = value; }
        }
    }
}
