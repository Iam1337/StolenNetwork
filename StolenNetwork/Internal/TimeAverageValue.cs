using System;

namespace StolenNetwork.Internal
{
    public struct TimeAverageValue
    {
        #region Public Vars

        private DateTime _refreshTime;

        private ulong _counterPrev;

        private ulong _counterNext;

        #endregion

        #region Public Methods

        public ulong Calculate()
        {
            var time = DateTime.Now;
            var delta = time.Subtract(_refreshTime).TotalSeconds;

            if (delta < 0.0)
            {
                delta = 0.0;

                _refreshTime = time;
                _counterNext = 0;
            }

            if (delta >= 1.0)
            {
                delta = 0.0;

                _refreshTime = time;
                _counterPrev = _counterNext;
                _counterNext = 0;
            }

            return (ulong)(_counterPrev * (1.0 - delta)) + _counterNext;
        }

        public void Increment()
        {
            ++_counterNext;
        }

        public void Add(ulong value)
        {
            _counterNext += value;
        }

        public void Reset()
        {
            _counterPrev = 0;
            _counterNext = 0;
        }

        #endregion
    }
}
