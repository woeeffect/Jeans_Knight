using System.Collections.Generic;

namespace EventBusSystem
{
    internal class SubscribersList<TSubscriber> where TSubscriber : class
    {
        private bool _needsCleanUp = false;

        public bool Executing;

        public readonly List<TSubscriber> List = new List<TSubscriber>();

        public void Add(TSubscriber subscriber)
        {
            List.Add(subscriber);
        }

        public void Remove(TSubscriber subscriber)
        {
            if (Executing)
            {
                var i = List.IndexOf(subscriber);
                if (i >= 0)
                {
                    _needsCleanUp = true;
                    List[i] = null;
                }
            }
            else
            {
                List.Remove(subscriber);
            }
        }

        public void Cleanup()
        {
            if (!_needsCleanUp)
            {
                return;
            }

            List.RemoveAll(s => s == null);
            _needsCleanUp = false;
        }
    }
}
