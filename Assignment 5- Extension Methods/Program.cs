using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment_5__Extension_Methods
{
    static class Program
    {
        static void Main()
        {
            int[] vals = { 5, 9, 4, 7, 15, 16, 10, 11, 14, 19, 18 };
            Console.WriteLine("Max over previous");
            Console.WriteLine(vals.MaxOverPrevious().ContentsAsString());
            Console.WriteLine("Local Maxima");
            Console.WriteLine(vals.LocalMaxima().ContentsAsString());
            Console.WriteLine("At Least K");
            Console.WriteLine(vals.AtLeastK(3, i => i >= 16 && i < 20));
            Console.WriteLine("At least half");
            Console.WriteLine(vals.AtLeastHalf(i => i >= 16 && i < 20));
            Console.WriteLine();
            Console.WriteLine("Overloaded");
            Console.WriteLine("Max over previous");
            Console.WriteLine(vals.MaxOverPrevious(i => i * 2).ContentsAsString());
            Console.WriteLine("Local Maxima");
            Console.WriteLine(vals.LocalMaxima(i => i * 2).ContentsAsString());
            Console.WriteLine("At Least K");
            Console.WriteLine(vals.AtLeastK(3, i => i >= 16 && i < 20, i => i * 2));
            Console.WriteLine("At least half");
            Console.WriteLine(vals.AtLeastHalf(i => i >= 16 && i < 20, i => i * 2));

            Console.ReadKey();
        }

        private static string ContentsAsString(this IEnumerable<int> vals)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var val in vals)
            {
                sb.Append(val + " ");
            }

            return sb.ToString();
        }

        //Rather than have a list of results, keep adding to it, and returning it, just have one yield return statement
        //Also, if the user has a .Take(x) or a condition or doesn't need all the values for any other reason, there is a benefit, as it will not need to iterate over the whole collection, especially if the transform function is processor intensive
        private static IEnumerable<int> MaxOverPrevious(this IEnumerable<int> vals, Func<int, int> transform)
        {
            int highest = transform(vals.First());
            yield return highest;

            vals.GetEnumerator().MoveNext();
            foreach (var val in vals) //todo should start from place 2 - does it with line 46?
            {
                var num = transform(val);

                if (num > highest)
                {
                    yield return (highest = num); //TODO does this work?
                }
            }
        }

        private static IEnumerable<int> MaxOverPrevious(this IEnumerable<int> vals)
        {
            return vals.MaxOverPrevious(i => i);
        }

        //Here too, if we only need part of the list, we have the advantage of deferred execution and we are avoiding creating and maintaining a list
        private static IEnumerable<int> LocalMaxima(this IEnumerable<int> vals, Func<int, int> transformer)
        {
            IEnumerator<int> enumerator = vals.GetEnumerator();
            enumerator.MoveNext();
            int prev = transformer(enumerator.Current);
            int current = prev;
            enumerator.MoveNext();
            int next = transformer(enumerator.Current);

            if (current > next)
            {
                yield return current;
            }

            foreach (var val in vals)
            {
                next = transformer(val);//needs to be the first thing because our current is our next
                if (current > prev && current > next)
                {
                    yield return current;
                }
                prev = current;
                current = next;
            }

            if (current > prev)
            {
                yield return current;
            }
        }

        private static IEnumerable<int> LocalMaxima(this IEnumerable<int> vals)
        {
            return vals.LocalMaxima(i => i);
        }

        private static bool AtLeastK(this IEnumerable<int> vals, int k, Func<int, bool> condition,
                Func<int, int> transform)

            /*
             * deferred execution helps us because this way, we don't have to transform or check all the values, only the ones we are using,
             * and we stop when we reach k correct values
             */

        {
            int counter = 0;

            for (int i = 0; i < vals.Count() && counter < k; i++)
            {
                if (condition(transform(vals.ElementAt(i))))
                    counter++;
            }

            return counter == k;
        }

        private static bool AtLeastK(this IEnumerable<int> vals, int k, Func<int, bool> condition)
        {
            return vals.AtLeastK(k, condition, i => i);
        }

        private static bool AtLeastHalf(this IEnumerable<int> vals, Func<int, bool> condition, Func<int, int> transform)
        {

            /*
             * deferred execution helps us because this way, we don't have to transform or check all the values, only the ones we are using,
             * and we stop when we reach k correct values
             * We specifically used the Count() method so that it can use the Count property if there is one 
             */

            return vals.AtLeastK((vals.Count() + 1) / 2, condition, transform);
        }

        private static bool AtLeastHalf(this IEnumerable<int> vals, Func<int, bool> condition)
        {
            return vals.AtLeastK(vals.Count() + 1 / 2, condition, i => i);
        }

        //private static IEnumerable<int> Transform(IEnumerable<int> vals, Func<int, int> transform)
        //{
        //    List<int> temp = null;
        //    if (transform != null)
        //    {
        //        temp = new List<int>();

        //        foreach (var val in vals)
        //        {
        //            temp.Add(transform(val));
        //        }

        //        vals = temp;
        //    }

        //    return temp ?? vals;
        //}
    }
}
