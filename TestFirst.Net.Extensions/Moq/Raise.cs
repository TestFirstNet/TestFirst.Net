using System;

namespace TestFirst.Net.Extensions.Moq
{
    public static class Raise
    {
        /// <summary>
        /// Raise an event on the given mocked event handler
        /// </summary>
        /// <typeparam name="T">The type of EventArgs the event handler requires</typeparam>
        /// <param name="sender">The sender of the event</param>
        /// <param name="args">The arguments of the event</param>
        /// <returns>A null event handler</returns>
        public static EventHandler<T> Event<T>(object sender, T args)
            where T : EventArgs
        {
            RaisedEvent.Next = new RaisedEvent(sender, args);
            return default(EventHandler<T>);
        }

        /// <summary>
        /// Raise an event on the given mocked event handler where it is a specific type
        /// of delegate - for example, PropertyChangedEventHandler
        /// </summary>
        /// <typeparam name="T">The type of the event handler</typeparam>
        /// <param name="sender">The sender of the event</param>
        /// <param name="args">The arguments of the event</param>
        /// <returns>A null event handler</returns>
        public static T Event<T>(object sender, EventArgs args)
        {
            RaisedEvent.Next = new RaisedEvent(sender, args);
            return default(T);
        }
    }
}
