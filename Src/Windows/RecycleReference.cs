using System;

namespace DevZest.Windows
{
    internal class RecycleReference<T>
        where T : class
    {
        private bool _isNull = true;
        private WeakReference _reference;

        public T Target
        {
            get { return _isNull || _reference == null || !_reference.IsAlive ? null : (T)_reference.Target; }
            set
            {
                if (value == null)
                    _isNull = true;
                else
                {
                    _isNull = false;
                    if (_reference == null)
                        _reference = new WeakReference(value);
                    _reference.Target = value;
                }
            }
        }
    }
}
