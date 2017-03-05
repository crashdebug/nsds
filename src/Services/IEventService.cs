using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace WebApplication.Interfaces
{
	public interface IEventService
	{
		void Register(string eventName, object handler);
		void Invoke(string eventName, params object[] args);
	}

    public class EventService : IEventService
    {
		private IDictionary<string, IList<Action<object[]>>> handlers = new ConcurrentDictionary<string, IList<Action<object[]>>>();

        public void Invoke(string eventName, params object[] args)
        {
			foreach (var e in this.handlers[eventName])
			{
				e.Invoke(args);
			}
        }

        public void Register(string eventName, object handler)
        {
            var h = handler as Action<object[]>;
			if (h == null)
			{
				throw new ArgumentException("Handler should be of type Action<object[]>");
			}
			if (!this.handlers.ContainsKey(eventName))
			{
				this.handlers.Add(eventName, new List<Action<object[]>>());
			}
			this.handlers[eventName].Add(h);
        }
    }
}