using DynamicData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeltaX.ReactiveDataTest
{

    public class DataTracker<T>
    {
        public DateTimeOffset updated;
        public T item;
        public NotifyCollectionChangedAction Action;

        public DataTracker(T item, NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Add)
        {
            this.updated = new DateTimeOffset(DateTime.Now);
            this.item = item;
            this.Action = action;
        }
    }

    class User
    {
        public int Id;
        public int Age;
        public string Name;
    }

    class Pruebas
    {

        public void Prueba1()
        {
            var myCache = new SourceCache<User, int>(t => t.Id);

            IObservable<IChangeSet<User, int>> myCacheSet = myCache.Connect();
            myCacheSet.Filter(u => u.Id == 2);
            myCache.Edit(items =>
            {
                items.Clear();
                items.AddOrUpdate(new User { Id = 2, Name = "Sima" });
            });

            var myConnection = myCacheSet.ToObservableChangeSet(t => t.Key, expireAfter: item => TimeSpan.FromHours(1));

        }

        public void Prueba2()
        {
            // Esto va en el repositorio (DB)
            var source = new SourceCache<User, int>(u => u.Id);

            var repoShared = source.Connect()
                .Transform(u => new DataTracker<User>(u));

            // En la capa de aplicacion cuando se accede al repositorio se accede al 
            var taskResult = GetItemsAsync(
                repoShared.Filter(u => u.item.Id == 442),
                TimeSpan.FromSeconds(10));
             
            // En repositorio: la actualizacion de los datos...
            source.AddOrUpdate(new User { Id = 2, Age = 3, Name = "User1" });

            source.AddOrUpdate(new[]{
                new User { Id = 1, Age = 33, Name= "User1"  },
                new User { Id = 3, Age = 44, Name= "User2"  },
                new User { Id = 33, Age = 44, Name= "User2"  },
                new User { Id = 442, Age = 44, Name= "User442"  },
            });



            var result = taskResult.Result; 
            Console.WriteLine("----- BindItems Data Count:{0} [{1}]", result.Count, string.Join(", ", result.Select(i => i.item.Id)));

            var sub = repoShared
                .Filter(u => u.item.Id >= 2)
                // .ToCollection()
                .Subscribe(u => {
                    Console.WriteLine("User Adds <{0}>", u);
                    Console.WriteLine("User Adds <{0}>", u.Adds);
                    Console.WriteLine("User Moves <{0}>", u.Moves);
                    Console.WriteLine("User Refreshes <{0}>", u.Refreshes);
                    Console.WriteLine("User Removes <{0}>", u.Removes);
                    Console.WriteLine("User Updates <{0}>", u.Updates);
                    Console.WriteLine("User GetEnumerator <{0}>", u.GetEnumerator());
                });

            source.AddOrUpdate(new User { Id = 2, Age = 3, Name = "User1" });

            var tResult = Task.Factory.StartNew(() =>
            {
                ManualResetEvent waitData = new ManualResetEvent(false);
                var sub2 = repoShared
                    .Filter(u => u.item.Id >= 2)
                    .Bind(out var dataResult)
                    .Subscribe(u => waitData.Set());

                Console.WriteLine("----- Task Data Count:{0} <{1}>", dataResult.Count, dataResult.ToList());
                waitData.WaitOne();
                Console.WriteLine("----- +++ Task Data Count:{0} <{1}>", dataResult.Count, dataResult.ToList());
                sub2.Dispose();
                return dataResult.ToList();
            });

            var tResult2 = GetItemsAsync(repoShared.Filter(u => u.item.Id == 442), TimeSpan.FromSeconds(10));

            Thread.Sleep(2000);

            source.AddOrUpdate(new User { Id = 2, Age = 3, Name = "User1" });

            source.AddOrUpdate(new[]{
                new User { Id = 1, Age = 33, Name= "User1"  },
                new User { Id = 3, Age = 44, Name= "User2"  },
                new User { Id = 33, Age = 44, Name= "User2"  },
                new User { Id = 442, Age = 44, Name= "User442"  },
            });

            Thread.Sleep(2000);

            source.AddOrUpdate(new User { Id = 2, Age = 2, Name = "User22" });

            sub.Dispose();
             
            Console.WriteLine("----- BindItems Data Count:{0} [{1}]", result.Count, string.Join(", ", result.Select(i => i.item.Id)));

            Thread.Sleep(2000);
            var myList = new SourceList<User>(); 

            //// var myList = new SourceList<User>();
            //// var oldPeople = myList.Connect()
            ////     .Filter(person => person.Age > 65)
            ////     .AsObservableList();
            //// var items = oldPeople.Items;
            //// 
            //// var oldPeople2 =  myList.Connect() 
            ////     .Transform(u => new DaataTracker<User>(u));
            //// 
            //// myList.Add(new User { Id = 2, Age = 3, Name= "User1"  });
            //// myList.AddRange(new[]{
            ////     new User { Id = 2, Age = 3, Name= "User1"  },
            ////     new User { Id = 3, Age = 4, Name= "User2"  },
            //// });

        }

        public Task<List<TResult>> GetItemsAsync<TResult>(
            IObservable<IChangeSet<TResult>> source,
            TimeSpan? timeout = null,
            CancellationToken? cancellationToken = null,
            Func<IChangeSet<TResult>, bool> onNext = null)
        {
            return Task.Run(() =>
            {
                if (onNext == null)
                {
                    onNext = (u) => true;
                }

                using (var waitData = new ManualResetEvent(false))
                using (source.Bind(out var dataResult).Subscribe(u => { if (onNext(u)) { waitData.Set(); } }))
                {
                    waitData.WaitOne(timeout ?? TimeSpan.FromMilliseconds(-1));
                    return dataResult.ToList();
                }
            }, cancellationToken ?? CancellationToken.None);
        }

        public Task<List<TResult>> GetItemsAsync<TResult, TKey>(
            IObservable<IChangeSet<TResult, TKey>> source,
            TimeSpan? timeout = null,
            CancellationToken? cancellationToken = null,
            Func<IChangeSet<TResult, TKey>, bool> onNext = null)
        {
            return Task.Run(() =>
            {
                if (onNext == null)
                {
                    onNext = (u) => true;
                }

                using (var waitData = new ManualResetEvent(false))
                using (source.Bind(out var dataResult).Subscribe(u => { if (onNext(u)) { waitData.Set(); } }))
                {
                    waitData.WaitOne(timeout ?? TimeSpan.FromMilliseconds(-1));
                    return dataResult.ToList();
                }
            }, cancellationToken ?? CancellationToken.None);
        }

        public void Prueba3()
        {
            List<DataTracker<User>> cache = new List<DataTracker<User>>();

            cache.Add(new DataTracker<User>(new User { Id = 1, Name = "Pepe 1" }));

            var observable = cache.ToObservable();
            observable.Do(i =>
            {
                var tracker = i;
            }).Subscribe()  ;

            // Esto de abajo no funciona
            cache.Add(new DataTracker<User>(new User { Id = 2, Name = "Pepe 2" }));
            cache.Add(new DataTracker<User>(new User { Id = 3, Name = "Pepe 3" }));

            observable.Append(new DataTracker<User>(new User { Id = 31, Name = "Pepe 31" }));

        }

        public class Observer<T> : IObserver<T>
        {
            public List<T> Results = new List<T>();

            public void OnCompleted() { }

            public void OnError(Exception error) { }

            public void OnNext(T value)
            {
                Results.Add(value);
            }
        }

        public Task<List<DataTracker<T>>> WaitResultsAsunc<T>(
            ObservableCollection<IEnumerable<DataTracker<T>>> cache,
            Func<DataTracker<T>, bool> filter,
            CancellationToken? token = null)
        {
            token ??= CancellationToken.None;

            return Task.Run(() =>
            {
                var resetEvent = new ManualResetEventSlim();
                var results = cache.SelectMany(e => e.Where(filter)).ToList();
                results ??= new List<DataTracker<T>>();

                if (results.Any())
                {
                    return results;
                }

                void Cache_CollectionChanged(object s, NotifyCollectionChangedEventArgs e)
                {
                    if (e.NewItems != null)
                    {
                        foreach (IEnumerable<DataTracker<T>> i in e.NewItems)
                        {
                            if (i.Where(filter)?.Any() == true)
                            {
                                results.Add(i);
                            }
                        }

                        if (results.Any())
                        {
                            resetEvent.Set();
                        }
                    }
                }

                cache.CollectionChanged +=  Cache_CollectionChanged;
                resetEvent.Wait(TimeSpan.FromSeconds(20), token.Value);
                cache.CollectionChanged -= Cache_CollectionChanged;

                return results;
            }, token.Value);
        } 

        public void Prueba5()
        {
            var cache = new ObservableCollection<IEnumerable<DataTracker<User>>>();

            var resultTask = WaitResultsAsunc(cache, (e) => true);

            cache.Add(new[] { new DataTracker<User>(new User { Id = 2, Name = "Pepe devuelve 1 " }) });
            cache.Add(new[] {
                new DataTracker<User>(new User { Id = 32, Name = "Pepe devuelve 2 1" }),
                new DataTracker<User>(new User { Id = 33, Name = "Pepe devuelve 2 2" })
            });
             
            cache.Add(new[] { new DataTracker<User>(new User { Id = 3, Name = "Pepe 3" }) });
            cache.Add(new[] { new DataTracker<User>(new User { Id = 3, Name = "Pepe 3" }) });
        }

        public void Prueba4()
        {
            var observer = new Observer<DataTracker<User>>();
            var cache = new ObservableCollection<DataTracker<User>>();
            // cache.Add(new DataTracker<User>(new User { Id = 1, Name = "Pepe 1" }));
            
            var ev = new ManualResetEventSlim();
            var result = new List<DataTracker<User>>();

            var d1 = cache.ToObservable().Subscribe(observer);

            lock (cache)
            {
                Task.Run(() =>
                {
                    Func<DataTracker<User>, bool> func = (i) => true;
                    var resetEvent = new ManualResetEventSlim();
                    var elements = cache.Where(func);

                    if (elements?.Count() == 0)
                    {
                        cache.CollectionChanged += (a, e) =>
                        {
                            if (e.NewItems != null)
                            {
                                foreach (DataTracker<User> i in e.NewItems)
                                {
                                    if (func(i))
                                    {
                                        resetEvent.Set();
                                    }
                                } 
                            }
                        };

                        if(!resetEvent.Wait(TimeSpan.FromSeconds(20)))
                        {
                              elements = cache.Where(func);
                        }  
                    }

                    var result = elements;

                });



                /// var d = cache.ToObservable()
                ///     .Select(i=>i)
                ///     .Subscribe((i) =>
                /// {
                ///     result.Add(i);
                ///     ev.Set();
                /// });
                /// cache.CollectionChanged += (a, e) =>
                /// {
                ///     if (e.NewItems != null)
                ///     {
                ///         foreach (DataTracker<User> i in e.NewItems)
                ///         {
                ///             result.Add(i);
                ///         }
                ///         ev.Set();
                ///     }
                /// };
            }

            Thread.Sleep(2000);
            cache.AddRange(new[] {
                new DataTracker<User>(new User { Id = 32, Name = "Pepe 32" }),
                new DataTracker<User>(new User { Id = 33, Name = "Pepe 33" })
            });

            cache.Add(new DataTracker<User>(new User { Id = 2, Name = "Pepe 2" }));
            cache.Add(new DataTracker<User>(new User { Id = 3, Name = "Pepe 3" }));
            cache.Add(new DataTracker<User>(new User { Id = 3, Name = "Pepe 3" }));
        }

        private void asdf(DataTracker<User> obj)
        {
            throw new NotImplementedException();
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var pruebas = new Pruebas();
            pruebas.Prueba5();
            Console.ReadLine();
        }
    }
}
