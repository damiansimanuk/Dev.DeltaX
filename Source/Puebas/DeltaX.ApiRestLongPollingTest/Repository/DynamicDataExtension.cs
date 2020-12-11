using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static Task<List<TResult>> GetItemsAsync<TResult, TKey>(
            this IObservable<IChangeSet<TResult, TKey>> source,
            TimeSpan? timeout = null,
            CancellationToken? cancellationToken = null,
            Func<IChangeSet<TResult, TKey>, bool> onNext = null)
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
    }
}
