/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System;

namespace StolenNetwork
{
    internal static class Pool<T> where T : class, new()
    {
        #region Extensions

        private class Collection
        {
            #region Public Vars

			public readonly T[] Buffer;

			public long ItemsInStack;

			public long ItemsInUse;

			#endregion

            #region Public Methods

			public Collection()
			{
				Buffer = new T[512];
				ItemsInStack = 0;
				ItemsInUse = 0;
			}

            #endregion
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

				return @object;
            }

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
