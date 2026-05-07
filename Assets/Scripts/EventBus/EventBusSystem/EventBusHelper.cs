using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBusSystem
{
    internal static class EventBusHelper
    {
        private static Dictionary<Type, List<Type>> _cachedSubscriberTypes =
            new Dictionary<Type, List<Type>>();

        public static List<Type> GetSubscriberTypes(
            IGlobalSubscriber globalSubscriber)
        {
            Type type = globalSubscriber.GetType();
            if (_cachedSubscriberTypes.ContainsKey(type))
            {
                return _cachedSubscriberTypes[type];
            }

            List<Type> subscriberTypes = type
                .GetInterfaces()
                .Where(t => t.GetInterfaces()
                    .Contains(typeof(IGlobalSubscriber)))
                .ToList();

            _cachedSubscriberTypes[type] = subscriberTypes;
            return subscriberTypes;
        }
    }
}
