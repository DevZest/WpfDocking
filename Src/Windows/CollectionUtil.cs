using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DevZest.Windows
{
    internal static class CollectionUtil
    {
        internal static void Synchronize<T>(IEnumerable<T> source, ReadOnlyCollection<T> target, Action<int, T> insertAction, Action<T> removeAction)
        {
            Synchronize(source, null, target, insertAction, removeAction);
        }

        internal static void Synchronize<T>(IEnumerable<T> source, Predicate<T> predicate, ReadOnlyCollection<T> target, Action<int, T> insertAction, Action<T> removeAction)
        {
            int count = 0;
            foreach (T item in source)
            {
                if (predicate != null && !predicate(item))
                    continue;

                if (!target.Contains(item))
                    insertAction(count, item);
                else if (target.IndexOf(item) > count)
                {
                    for (int i = target.IndexOf(item) - 1; i >= count; i--)
                        removeAction(target[i]);
                }
                else
                    Debug.Assert(target.IndexOf(item) == count);

                count++;
            }

            for (int i = target.Count - 1; i >= count; i--)
                removeAction(target[i]);

            Debug.Assert(count == target.Count);
        }
    }
}
