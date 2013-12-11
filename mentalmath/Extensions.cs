using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mentalmath
{
    /// <summary>
    /// Extension-Methods for mentalmath
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets the closest lower match of the given number in the list. Example: list=(2,7,8,15,19), num=14, Result = 8
        /// </summary>
        /// <param name="list"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int ClosestTo(this List<int> list, int num)
        {
            int best = 0;

            foreach(int i in list.OrderBy(i=>i))
            {
                if (i > num)
                    break;
                best = i;
            }
            return best;
        }

        public static int Remove<T>(this ObservableCollection<T> coll, Func<T, bool> condition)
        {
            var itemsToRemove = coll.Where(condition).ToList();

            foreach (var itemToRemove in itemsToRemove)
                coll.Remove(itemToRemove);

            return itemsToRemove.Count;
        }
    }
}
