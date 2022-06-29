using System;
using System.Collections.Generic;

namespace MyLinqLib
{
  public static class MyLinqExtensions
  {

    #region ---------------------------------------- Filter

    public delegate bool MyFilter<T>(T item);
    public static IEnumerable<T> Where<T>(
      this IEnumerable<T> cltn,
      MyFilter<T> filter)
    {
      foreach (T item in cltn)
      {
        if (filter(item)) yield return item;
      }
    }

    public static IEnumerable<T> Distinct<T>(this IEnumerable<T> cltn)
    {
      var uniques = new HashSet<T>();
      foreach (var item in cltn)
      {
        uniques.Add(item);
      }
      foreach (var item in uniques) yield return item;
    }

    public static IEnumerable<T> Take<T>(this IEnumerable<T> cltn, int nr)
    {
      int count = 1;
      foreach (var item in cltn)
      {
        if (count > nr) break;
        yield return item;
        count++;
      }
    }

    public static IEnumerable<T> TakeWhile<T>(this IEnumerable<T> cltn, Predicate<T> filter)
    {
      foreach (T item in cltn)
      {
        if (filter(item) == false) break;
        yield return item;
      }
    }

    public static IEnumerable<T> Skip<T>(this IEnumerable<T> cltn, int nr)
    {
      int count = 1;
      foreach (var item in cltn)
      {
        if (count > nr) yield return item;
        count++;
      }
    }

    public static IEnumerable<T> SkipWhile<T>(this IEnumerable<T> cltn, Predicate<T> filter)
    {
      bool doTrySkip = true;
      foreach (T item in cltn)
      {
        if (doTrySkip && filter(item)) continue;
        doTrySkip = false;
        yield return item;
      }
    }

    #endregion

    #region ---------------------------------------- Projection
    public static IEnumerable<S> Select<T, S>(this IEnumerable<T> cltn, Func<T, S> transformer)
    {
      List<S> ret = new();
      foreach (var item in cltn)
      {
        ret.Add(transformer(item));
      }
      return ret;
    }
    #endregion

    #region ---------------------------------------- First/Last
    public static T First<T>(this IEnumerable<T> cltn)
    {
      foreach (var item in cltn)
      {
        return item;
      }
      throw new Exception("Sequence contains no elements");
    }

    public static T FirstOrDefault<T>(this IEnumerable<T> cltn)
    {
      foreach (var item in cltn)
      {
        return item;
      }
      return default(T);
    }

    public static T Last<T>(this IEnumerable<T> cltn)
    {
      if (!cltn.Any()) throw new Exception("Sequence contains no elements");
      T last = default(T);
      foreach (var item in cltn)
      {
        last = item;
      }
      return last;
    }

    public static T LastOrDefault<T>(this IEnumerable<T> cltn)
    {
      if (!cltn.Any()) return default(T);
      T last = default(T);
      foreach (var item in cltn)
      {
        last = item;
      }
      return last;
    }

    public static T First<T>(this IEnumerable<T> cltn, Predicate<T> condition)
    {
      foreach (var item in cltn)
      {
        if (condition(item)) return item;
      }
      throw new Exception("Sequence contains no elements with specified condition");
    }

    public static T Last<T>(this IEnumerable<T> cltn, Predicate<T> condition)
    {
      T last = default(T);
      bool found = false;
      foreach (var item in cltn)
      {
        if (!condition(item)) continue;
        last = item;
        found = true;
      }
      if (found) return last;
      throw new Exception("Sequence contains no elements with specified condition");
    }

    public static T FirstOrDefault<T>(this IEnumerable<T> cltn, Predicate<T> condition)
    {
      foreach (var item in cltn)
      {
        if (condition(item)) return item;
      }
      return default(T);
    }

    public static T LastOrDefault<T>(this IEnumerable<T> cltn, Predicate<T> condition)
    {
      T last = default(T);
      foreach (var item in cltn)
      {
        if (condition(item)) last = item;
      }
      return last;
    }

    #endregion

    #region ---------------------------------------- Aggregation
    public static double Average(this IEnumerable<int> cltn)
    {
      double sum = 0.0;
      int count = 0;
      foreach (var item in cltn)
      {
        sum += item;
        count++;
      }
      return count == 0 ? 0 : sum / count;
    }
    public static double Average(this IEnumerable<double> cltn)
    {
      double sum = 0;
      int count = 0;
      foreach (var item in cltn)
      {
        sum += item;
        count++;
      }
      return count == 0 ? 0 : sum / count;
    }
    public static int Average<T>(this IEnumerable<T> cltn, Func<T, int> f)
    {
      int sum = 0;
      int count = 0;
      foreach (var item in cltn)
      {
        sum += f(item);
        count++;
      }
      return count == 0 ? 0 : sum / count;
    }
    public static double Average<T>(this IEnumerable<T> cltn, Func<T, double> f)
    {
      double sum = 0;
      int count = 0;
      foreach (var item in cltn)
      {
        sum += f(item);
        count++;
      }
      return count == 0 ? 0 : sum / count;
    }

    public static int Max(this IEnumerable<int> cltn)
    {
      if (cltn.Count() == 0) throw new Exception("Sequence contains no elements");
      int max = -int.MaxValue;
      foreach (var item in cltn)
      {
        if (item > max) max = item;
      }
      return max;
    }
    public static double Max(this IEnumerable<double> cltn)
    {
      if (cltn.Count() == 0) throw new Exception("Sequence contains no elements");
      double max = -double.MaxValue;
      foreach (var item in cltn)
      {
        if (item > max) max = item;
      }
      return max;
    }

    public static int Count<T>(this IEnumerable<T> cltn)
    {
      int count = 0;
      foreach (var item in cltn)
      {
        count++;
      }
      return count;
    }
    public static int Count<T>(this IEnumerable<T> cltn, Predicate<T> filter)
    {
      int count = 0;
      foreach (var item in cltn)
      {
        if (filter(item)) count++;
      }
      return count;
    }
    public static bool Any<T>(this IEnumerable<T> cltn)
    {
      return cltn.Count() > 0;
    }

    #endregion

    #region ---------------------------------------- Sorting
    public static IEnumerable<T> OrderBy<T, T_out>(
      this List<T> cltn,
      Func<T, T_out> f) where T_out : IComparable
    {
      cltn.Sort((a, b) => f(a).CompareTo(f(b)));
      return cltn;
    }
    //public static IEnumerable<T> OrderBy<T>(this List<T> cltn, Func<T, int> f)
    //{
    //  Comparison<T> comparer = (a, b) =>
    //  {
    //    int x = f(a).CompareTo(f(b));
    //    return x;
    //    //return f(a) - f(b);
    //    //if (f(a) == f(b)) return 0;
    //    //if (f(a) > f(b)) return 1;
    //    //return -1;
    //  };
    //  cltn.Sort(comparer);
    //  return cltn;
    //}
    //public static IEnumerable<T> OrderBy<T>(this List<T> cltn, Func<T, double> f)
    //{

    //  Comparison<T> comparer = (a, b) =>
    //  {
    //    int x = f(a).CompareTo(f(b));
    //    return x;
    //  };
    //  cltn.Sort(comparer);
    //  return cltn;
    //}
    public static IEnumerable<T> OrderByDescending<T>(this List<T> cltn, Func<T, int> f)
    {
      Comparison<T> comparer = (a, b) =>
      {
        int x = f(b).CompareTo(f(a));
        return x;
      };
      cltn.Sort(comparer);
      return cltn;
    }
    public static IEnumerable<T> OrderByDescending<T>(this List<T> cltn, Func<T, double> f)
    {
      Comparison<T> comparer = (a, b) =>
      {
        int x = f(b).CompareTo(f(a));
        return x;
      };
      cltn.Sort(comparer);
      return cltn;
    }

    public static IEnumerable<T> Reverse<T>(this IEnumerable<T> cltn)
    {
      var ret = new List<T>();
      foreach (var item in cltn)
      {
        ret.Insert(0, item);
      }
      return ret;
    }
    #endregion

    #region ---------------------------------------- Conversion
    public static List<T> ToList<T>(this IEnumerable<T> cltn)
    {
      var list = new List<T>();
      foreach (var item in cltn)
      {
        list.Add(item);
      }
      return list;
    }

    public static T[] ToArray<T>(this IEnumerable<T> cltn)
    {
      var array = new T[cltn.Count()];
      int i = 0;
      foreach (var item in cltn)
      {
        array[i] = item;
      }
      return array;
    }
    #endregion

  }
}
