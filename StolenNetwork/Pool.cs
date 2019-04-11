using System;

namespace StolenNetwork
{
    public static class Pool<T> where T : class, new()
    {
        #region Extensions

        private class Collection
        {
            public T[] Buffer;

            public Collection()
            {
                Reset();
            }

            public long ItemsInStack { get; set; }

            public long ItemsInUse { get; set; }

            public long ItemsCreated { get; set; }

            public long ItemsTaken { get; set; }

            public void Reset()
            {
                Buffer = new T[512];
                ItemsInStack = 0;
                ItemsInUse = 0;
                ItemsCreated = 0;
                ItemsTaken = 0;
            }
        }

        #endregion

        #region Public Vars

        private static Collection _collection = new Collection();

        #endregion

        #region Public Methods

        public static T Get()
        {
            if (_collection.ItemsInStack > 0)
            {
                --_collection.ItemsInStack;
                ++_collection.ItemsInUse;

                var @object = _collection.Buffer[_collection.ItemsInStack];
                _collection.Buffer[_collection.ItemsInStack] = null;

                ++_collection.ItemsTaken;

                return @object;
            }

            ++_collection.ItemsCreated;
            ++_collection.ItemsInUse;

            return Activator.CreateInstance<T>();
        }

        public static void Free(ref T @object)
        {
            if (@object == null)
                throw new ArgumentNullException(nameof(@object));

            if (_collection.ItemsInUse >= _collection.Buffer.Length)
            {
                --_collection.ItemsInUse;

                @object = null;
            }
            else
            {
                _collection.Buffer[_collection.ItemsInStack] = @object;

                ++_collection.ItemsInStack;
                --_collection.ItemsInUse;

                @object = null;
            }
        }

        #endregion
    }
}
