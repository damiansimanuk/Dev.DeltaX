using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeltaX.ApiRestLongPollingTest.Repository
{

    public static class DynamicDataExtension
    {

        public static Task<List<TResult>> GetItemsAsync<TResult>(
            this IObservable<IChangeSet<TResult>> source,
            TimeSpan? timeout = null,
            CancellationToken? cancellationToken = null,
            Func<IChangeSet<TResult>, bool> onNext = null)
        {
            timeout = timeout ?? TimeSpan.FromMilliseconds(-1);
            cancellationToken = cancellationToken ?? CancellationToken.None;

            return Task.Run(() =>
            {
                if (onNext == null)
                {
                    onNext = (u) => true;
                }

                using (var waitData = new ManualResetEventSlim(false))
                using (source.Bind(out var dataResult).Subscribe(u => { if (onNext(u)) { waitData.Set(); } }))
                {
                    waitData.Wait((int)timeout.Value.TotalMilliseconds, cancellationToken.Value);
                    return dataResult.ToList();
                }
            }, cancellationToken.Value);
        }

        public static Task<List<TResult>> GetItems2Async<TResult, TKey>(
            this IObservable<IChangeSet<TResult, TKey>> source,
            TimeSpan? timeout = null,
            CancellationToken? cancellationToken = null)
        {
            timeout = timeout ?? TimeSpan.FromMilliseconds(-1);
            cancellationToken = cancellationToken ?? CancellationToken.None;

            return Task.Run(() =>
            {
                using (var waitData = new ManualResetEventSlim(false))
                using (source.Bind(out var dataResult).Subscribe((u) => waitData.Set()))
                {
                    waitData.Wait((int)timeout.Value.TotalMilliseconds, cancellationToken.Value);
                    return dataResult.ToList();
                }
            }, cancellationToken.Value);
        }

        public static Task<List<TResult>> GetItemsAsync<TResult, TKey>(
            this IObservable<IChangeSet<TResult, TKey>> source,
            TimeSpan timeout,
            CancellationToken? cancellationToken = null)
        {
            cancellationToken = cancellationToken ?? CancellationToken.None;

            return Task.Run(() =>
            {
                var result = new List<TResult>();
                using (var waitData = new ManualResetEventSlim(false))
                using (var sub = source
                    .Do(c =>
                    {
                        result = c.Select(i => i.Current).ToList();
                        waitData.Set();
                    })
                    .Subscribe())
                {
                    waitData.Wait(timeout, cancellationToken.Value);
                    return result;
                }
            }, cancellationToken.Value);
        }
    }
}
