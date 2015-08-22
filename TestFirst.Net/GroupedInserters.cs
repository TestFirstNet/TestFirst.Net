using System;
using System.Collections.Generic;
using System.Linq;

namespace TestFirst.Net
{
    /// <summary>
    /// Collect a number of <see cref="IInserter"/>'s together to represent a set of relationships.
    /// 
    /// Note: Suggested usage is to subclass this to create your own model and hide all these methods from users. This way you
    /// can ensure your model is limited and controlled and documented
    /// 
    /// Note:think of this as simply a glorified dictionary with convenient inserter lookup and register logic (which it really is). Various
    /// methods operate to locate the inserter of interest and mutates it
    /// 
    /// TODO:should this be renamed to InserterModel? is rally acollection of inserters to represent a model in the db
    /// </summary>
    public class GroupedInserters:IInserter
    {
        private const int DefaultIndex = 0;

        private readonly IDictionary<string, IInserter> m_insertersByKey = new Dictionary<string, IInserter>();
        //can't gaurantee order in above and some inserters may require others to be run first
        private readonly IList<IInserter> m_insertersInOrderAdded = new List<IInserter>();

        /// <summary>
        /// Holds the next available index we can use for the given inserter type. Allows us to automatically increment
        /// keys when adding an inserter for which the user has not provided a key
        /// </summary>
        private readonly IDictionary<Type, int> m_nextAvailableIndexByInserterType = new Dictionary<Type, int>();

        /// <summary>
        /// Encourage specific subclasses to group operations instead of each caller adding their own inserters
        /// willy nilly
        /// </summary>
        protected GroupedInserters()
        {
        }

        /// <summary>
        /// Adds a new inserter of type T.
        /// If one already exists, adds another with the next available index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T Add<T>() where T : IInserter, ICloneable, new()
        {
            int autoIndex = GetNextAvailableIndexForType<T>();
            return Add(new T(), autoIndex);
        }

        /// <summary>
        /// Use to add multiple inserters with known IDs.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <returns></returns>
        protected T Add<T>(int index) where T : IInserter, ICloneable, new()
        {
            return Add(new T(), index);
        }

        protected T Add<T>(T inserter) where T : IInserter, ICloneable
        {
            return Add(inserter, DefaultIndex);
        }

        protected T AddAutoIndex<T>() where T : IInserter, ICloneable, new()
        {
            return AddAutoIndex(new T());
        }

        protected T AddAutoIndex<T>(T inserter) where T : IInserter, ICloneable
        {
            var autoIndex = GetNextAvailableIndexForType<T>();
            return Add(inserter, autoIndex);
        }

        protected T Add<T>(T inserter, int index) where T : IInserter, ICloneable
        {
            string internalKey = ToKey(inserter.GetType(), index);
            //todo: catch exception and suggest AddAnother()
            InternalAddInserterByKey(internalKey, inserter);
            IncrementNextAvailableIndexForType<T>(index);

            return inserter;
        }

        protected T Add<T>(Enum key) where T : IInserter, ICloneable, new()
        {
            return Add(new T(), key);
        }

        protected T Add<T>(T inserter, Enum key) where T : IInserter, ICloneable
        {
            string internalKey = ToKey(inserter.GetType(), key);
            InternalAddInserterByKey(internalKey, inserter);
            return inserter;
        }

        protected T Add<T>(String userKey) where T : IInserter, ICloneable, new()
        {
            return Add(new T(), userKey);
        }

        protected T Add<T>(T inserter, String userKey) where T : IInserter, ICloneable
        {
            string internalKey = ToKey(inserter.GetType(), userKey);
            InternalAddInserterByKey(internalKey, inserter);
            return inserter;
        }

        private void InternalAddInserterByKey<T>( String internalKey, T inserter) where T : IInserter, ICloneable
        {
            try
            {
                m_insertersByKey.Add(internalKey, inserter);
                m_insertersInOrderAdded.Add(inserter);
            }
            catch(ArgumentException e)
            {
                throw new ArgumentException(String.Format("Error adding inserter {0} with key '{1}'. Already have keys [{2}]", inserter, internalKey, String.Join(",\n",m_insertersByKey.Keys)),e);
            }
        }

        /// <summary>
        /// Return the inserter of the given type or null if it doesn't exist
        /// </summary>
        public T Get<T>() where T : IInserter, ICloneable
        {
            return Get<T>(DefaultIndex);
        }

        public T Get<T>(Enum key) where T : IInserter, ICloneable
        {
            return With<T>(key);
        }

        public T Get<T>(int index) where T : IInserter, ICloneable
        {
            return With<T>(index);
        }

        public T GetOrAdd<T>() where T : IInserter, ICloneable, new()
        {
            return WithOrAdd<T>(DefaultIndex);
        }

        public T GetOrAdd<T>(Func<T> factory) where T : IInserter, ICloneable
        {
            return WithOrAdd(DefaultIndex, factory);
        }

        public IEnumerable<T> GetAll<T>() where T : IInserter, ICloneable
        {
            return WithAll<T>();
        }


        protected void With<T>(Action<T> action) where T : IInserter, ICloneable
        {
            var inserters = WithAll<T>().ToList();
            if(inserters.Count != 1)
            {
                throw new TestFirstException("Expected 1 inserter of type " + typeof(T).FullName + " but found " + inserters.Count);
            }

            action.Invoke(inserters.FirstOrDefault());
        }

        /// <summary>
        /// Applies action to all inserters of type T, in no specific order.
        /// Returns list of matched inserters, or empty list.
        /// </summary>
        /// <typeparam name="T">The inserter type</typeparam>
        /// <returns>List of inserters</returns>
        protected void WithAll<T>(Action<T> action) where T : IInserter, ICloneable
        {
            WithAll<T>().ToList().ForEach(action.Invoke);
        }

        /// <summary>
        /// Get all inserters of type T, in no specific order, or empty list.
        /// </summary>
        /// <typeparam name="T">The inserter type</typeparam>
        /// <returns>List of inserters</returns>
        private IEnumerable<T> WithAll<T>() where T : IInserter, ICloneable
        {
            return m_insertersByKey.Values.Where(inserter => inserter is T).Cast<T>();
        }

        /// <summary>
        /// Return the inserter of the given type, with the given key, if it exists, else throw an exception
        /// </summary>e 
        /// <typeparam name="T">the inserter type</typeparam>
        /// <param name="key">the sub key</param>
        /// <returns>the configured instance of the given type</returns>
        private T With<T>(Enum key) where T : IInserter, ICloneable
        {
            IInserter inserter;
            var internalKey = ToKey(typeof(T), key);
            if (!m_insertersByKey.TryGetValue(internalKey, out inserter))
            {
                var pairs = m_insertersByKey.Select(pair => pair.Key + "=" + pair.Value.GetType().FullName);
                var availableKeysAndTypes = String.Join(",", pairs);
                throw new InvalidOperationException(String.Format("no inserter of type '{0}' with key {1}. Have [{2}]", typeof(T).FullName, key, availableKeysAndTypes));
            }
            return (T)inserter;
        }

        /// <summary>
        /// Return the inserter of the given type, with the given index, if it exists, else throw an exception
        /// </summary>
        /// <typeparam name="T">the inserter type</typeparam>
        /// <param name="index">the sub index</param>
        /// <returns>the configured instance of the given type</returns>
        private T With<T>(int index) where T : IInserter, ICloneable
        {
            IInserter inserter;
            string internalKey = ToKey(typeof(T), index);
            if (!m_insertersByKey.TryGetValue(internalKey, out inserter))
            {
                var pairs = m_insertersByKey.Select(pair => pair.Key + "=" + pair.Value.GetType().FullName);
                var availableKeysAndTypes = String.Join(",", pairs);
                throw new InvalidOperationException(String.Format("no inserter of type '{0}' with index {1}. Have [{2}]", typeof(T).FullName, index, availableKeysAndTypes));
            }
            return (T)inserter;
        }

        private T WithOrAdd<T>(int index) where T : IInserter, ICloneable, new()
        {
            return WithOrAdd(index,()=>new T());
        }

        private T WithOrAdd<T>(int index, Func<T> factory) where T : IInserter, ICloneable
        {
            IInserter inserter;
            string internalKey = ToKey(typeof(T), index);
            if (!m_insertersByKey.TryGetValue(internalKey, out inserter))
            {
                //lets add it
                inserter = Add(factory.Invoke(),index);
            }
            return (T)inserter;
        }

        protected void RemoveAll<T>(Func<T, bool> condition) where T : IInserter, ICloneable
        {
            var inserters = GetAll<T>().ToList();

            foreach (var inserter in inserters)
            {
                if (condition(inserter))
                {
                    Remove(inserter);
                }
            }
        }

        protected void Remove(IInserter inserter)
        {
            foreach (var entry in m_insertersByKey.Where(kvp => kvp.Value == inserter).ToList())
            {
                m_insertersByKey.Remove(entry);
            }
            m_insertersInOrderAdded.Remove(inserter);
        }

        public IList<T> GetWhere<T>(Func<T, bool> condition) where T : IInserter, ICloneable
        {
            return new List<T>(GetAll<T>().Where(condition));
        }

        /// <summary>
        /// Return all the inserters added. Useful if you need to perform any dependency resolution (most likely via property
        /// injection)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IInserter> GetInserters()
        {
            return new List<IInserter>(m_insertersByKey.Values);
        }

        public virtual void Insert()
        {
            foreach (var inserter in GetInsertersInInsertionOrder())
            {
                inserter.Insert();
            }
        }

        /// <summary>
        /// Override this if you wish to perform custom recordering of inserters. May be useful if you add inserters on the fly
        /// and these ae required to be run before previously added inserters
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<IInserter> GetInsertersInInsertionOrder()
        {
            return m_insertersInOrderAdded;
        }

        public virtual GroupedInserters Clone()
        {
            return Clone(new GroupedInserters());
        }

        public T Clone<T>(T newSubClassInstance) where T:GroupedInserters
        {
            var clonedInserters = new Dictionary<string, IInserter>();
            //clone inserters first before assigning, in case clone throws an error
            foreach (var entry in m_insertersByKey)
            {
                var inserter = entry.Value;
                clonedInserters.Add(entry.Key, (IInserter)((ICloneable)inserter));
            }
            //should now be a safe operation to copy cloned inserters across
            foreach (var entry in clonedInserters)
            {
                newSubClassInstance.m_insertersByKey[entry.Key] = entry.Value;
            }
            return newSubClassInstance;
        }

        private void IncrementNextAvailableIndexForType<T>(int insertedIndex)
        {
            // be careful that we don't decrement, as user might be inserting in a random order
            int nextIndex = GetNextAvailableIndexForType<T>();
            if (insertedIndex >= nextIndex)
            {
                m_nextAvailableIndexByInserterType[typeof (T)] = insertedIndex + 1;
            }
        }

        private int GetNextAvailableIndexForType<T>()
        {
            int result;
            return m_nextAvailableIndexByInserterType.TryGetValue(typeof(T), out result)
                       ? result
                       : DefaultIndex;
        }

        private static string ToKey(Type t, Enum key)
        {
            return t.FullName + ":" + key.GetType().Name + ":" + key;
        }

        private static string ToKey(Type t, int index)
        {
            return t.FullName + ":" + index;
        }

        private static string ToKey(Type t, String userSuppliedKey)
        {
            return t.FullName + ":<user-key>:" + userSuppliedKey;
        }
    }
}