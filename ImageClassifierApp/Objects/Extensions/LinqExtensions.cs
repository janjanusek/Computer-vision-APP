using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageClassifierApp.Properties;

namespace ImageClassifierApp.Objects.Extensions
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Method provides ForEach functionality with Exception handling. 
        /// If exception is throwed item will be replaced with it's default value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="paItems"></param>
        /// <param name="paFunc"></param>
        /// <param name="paExceptionHandler"></param>
        /// <returns></returns>
        public static IEnumerable<T> TryOrDefault<T, TException>(this IEnumerable<T> paItems, [NotNull]Func<T, T> paFunc, Action<TException, T> paExceptionHandler = null) where TException : Exception
        {
            foreach (var paItem in paItems)
            {
                yield return HandleInvoke<T, TException>(paItem, paFunc, (exception, arg2) =>
                {
                    paExceptionHandler?.Invoke(exception, arg2);
                    return default(T);
                });
            }
        }

        /// <summary>
        /// Method provides ForEachAsync functionality with Exception handling. 
        /// If exception is throwed item will be replaced with it's default value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="paItems"></param>
        /// <param name="paFunc"></param>
        /// <param name="paExceptionHandler"></param>
        /// <returns></returns>
        public static IEnumerable<T> TryOrDefaultAsync<T, TException>(this IEnumerable<T> paItems, [NotNull]Func<T, T> paFunc, Action<TException, T> paExceptionHandler = null) where TException : Exception
        {
            var items = new LinkedList<T>(paItems);
            var tasks = items.Select(item => new Task<T>(() =>
            {
                return HandleInvoke<T, TException>(item, paFunc, (exception, arg2) =>
                {
                    paExceptionHandler?.Invoke(exception, arg2);
                    return default(T);
                });
            })).ToArray();
            tasks.ForEach(t => t.Start()).ExecuteQuery();
            Task.WaitAll(tasks);
            foreach (var task in tasks)
            {
                var value = task.Result;
                task.Dispose();
                yield return value;
            }
        }

        /// <summary>
        /// Method provide ForEach functionality with Exception handling
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="paItems"></param>
        /// <param name="paFunc"></param>
        /// <param name="paExceptionHandler"></param>
        /// <returns></returns>
        public static IEnumerable<T> TryOrHandle<T, TException>(this IEnumerable<T> paItems, [NotNull]Func<T, T> paFunc, [NotNull]Func<TException, T, T> paExceptionHandler) where TException : Exception
        {
            foreach (var paItem in paItems)
            {
                yield return HandleInvoke<T, TException>(paItem, paFunc, paExceptionHandler);
            }
        }

        /// <summary>
        /// Method provide ForEachAsync functionality with Exception handling
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="paItems"></param>
        /// <param name="paFunc"></param>
        /// <param name="paExceptionHandler"></param>
        /// <returns></returns>
        public static IEnumerable<T> TryOrHandleAsync<T, TException>(this IEnumerable<T> paItems, [NotNull]Func<T, T> paFunc, [NotNull]Func<TException, T, T> paExceptionHandler) where TException : Exception
        {
            var items = new LinkedList<T>(paItems);
            var tasks = items.Select(item =>
                new Task<T>(() => HandleInvoke<T, TException>(item, paFunc, paExceptionHandler)))
                .ToArray();
            tasks.ForEach(t => t.Start()).ExecuteQuery();
            Task.WaitAll(tasks);
            foreach (var task in tasks)
            {
                var value = task.Result;
                task.Dispose();
                yield return value;
            }
        }

        /// <summary>
        /// Method provides funcionality of Action.Invoke with exception handling
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="paItem"></param>
        /// <param name="paFunc"></param>
        /// <param name="paExceptionHandler"></param>
        /// <returns></returns>
        private static T HandleInvoke<T, TException>(T paItem, [NotNull]Func<T, T> paFunc, Func<TException, T, T> paExceptionHandler = null) where TException : Exception
        {
            try
            {
                return paFunc.Invoke(paItem);
            }
            catch (TException e)
            {
                return paExceptionHandler != null ? paExceptionHandler.Invoke(e, paItem) : default(T);
            }
        }

        /// <summary>
        /// Method provides ForEach funcionality over all collection. 
        /// Every Action is executed synchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paItems"></param>
        /// <param name="paActionForEachItem"></param>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> paItems, Action<T> paActionForEachItem)
        {
            foreach (var item in paItems)
            {
                paActionForEachItem.Invoke(item);
                yield return item;
            }
        }

        /// <summary>
        /// Method provides ForEach funcionality over all collection. 
        /// Every Action is executed as separate parrallel Task
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paItems"></param>
        /// <param name="paActionForEachItem"></param>
        /// <returns></returns>
        public static IEnumerable<T> ForEachAsync<T>(this IEnumerable<T> paItems, Action<T> paActionForEachItem)
        {
            var waiting = new LinkedList<T>(paItems);
            var tasks = waiting.Select(item => new Task(() => { paActionForEachItem.Invoke(item); })).ToArray();
            tasks.ForEach(t => t.Start()).ExecuteQuery();
            Task.WaitAll(tasks);
            tasks.ForEach(t => t.Dispose()).ExecuteQuery();
            foreach (var item in waiting)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Method turns any item into a collection of type <typeparam name="TOut"></typeparam>
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="paItem"></param>
        /// <returns></returns>
        public static IEnumerable<TOut> AsCollection<TIn, TOut>(this TIn paItem) where TOut : class
        {
            if (paItem is IEnumerable<TOut> collection)
            {
                foreach (var item in collection)
                {
                    yield return item;
                }
            }
            else
            {
                yield return new[] { paItem }.Select(item => item is TOut ? item as TOut : default(TOut)).FirstOrDefault();
            }
        }

        /// <summary>
        /// Method turns any item into a collection of type <typeparam name="TOut"></typeparam>
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="paItem"></param>
        /// <param name="paConvFunc"></param>
        /// <returns></returns>
        public static IEnumerable<TOut> AsCollection<TIn, TOut>(this TIn paItem, [NotNull]Func<TIn, TOut> paConvFunc) where TOut : class
        {
            if (paItem is IEnumerable<TOut> collection)
            {
                foreach (var item in collection)
                {
                    yield return item;
                }
            }
            else
            {
                yield return new[] { paItem }.Select(paConvFunc).FirstOrDefault();
            }
        }

        /// <summary>
        /// Method execute all items in foreach loop in case it's used some of ForEach extesion or Try... LINQ extesion
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paItems"></param>
        public static void ExecuteQuery<T>(this IEnumerable<T> paItems)
        {
            foreach (var paItem in paItems)
            {
                // just iterate through whole collection
            }
        }

        /// <summary>
        /// Method execute all items in foreach loop in case it's used some of ForEach extesion or Try... LINQ extesion
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paItems"></param>
        public static async Task ExecuteQueryAsync<T>(this IEnumerable<T> paItems)
        {
            var task = new Task(() =>
            {
                ExecuteQuery(paItems);
            });
            using (task)
            {
                task.Start();
                await task;
            }
        }

        /// <summary>
        /// Method return index of item in specified collection with complexity O(N)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paItems"></param>
        /// <param name="paItem"></param>
        /// <returns></returns>
        public static int IndexOf<T>(this IEnumerable<T> paItems, T paItem)
        {
            var index = 0;
            foreach (var item in paItems)
            {
                if (item.Equals(paItem))
                    return index;
                index++;
            }
            return -1;
        }
    }
}