using System.Collections.Concurrent;

namespace bs
{
    public class KeyValueStorage
    {
        ConcurrentDictionary<string, object> _dictionary;
        public KeyValueStorage()
        {

            _dictionary = new ConcurrentDictionary<string,object>();
        }
    }
}
