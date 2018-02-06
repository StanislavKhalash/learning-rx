using NUnit.Framework;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace RxSandbox.Tests
{
    [TestFixture]
    public class ColdHotObservablesFixture
    {
        [Test]
        public async Task Interval_IsColdObservable()
        {
            var firstSubscription = new TaskCompletionSource();
            var secondSubscription = new TaskCompletionSource();

            var observable = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5);

            observable
                .Subscribe(i => TestContext.Out.WriteLine($"First subscription --> {i}"), 
                () => firstSubscription.SetResult());

            await Task.Delay(TimeSpan.FromSeconds(3));

            observable
                .Subscribe(i => TestContext.Out.WriteLine($"Second subscription --> {i}"),
                () => secondSubscription.SetResult());

            await Task.WhenAll(firstSubscription.Task, secondSubscription.Task);
        }

        [Test]
        public async Task Interval_WhenPublishedAndConnected_IsColdObservable()
        {
            var firstSubscription = new TaskCompletionSource();
            var secondSubscription = new TaskCompletionSource();

            var observable = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5).Publish();
            observable.Connect();

            observable
                .Subscribe(i => TestContext.Out.WriteLine($"First subscription --> {i}"),
                () => firstSubscription.SetResult());

            await Task.Delay(TimeSpan.FromSeconds(3));

            observable
                .Subscribe(i => TestContext.Out.WriteLine($"Second subscription --> {i}"),
                () => secondSubscription.SetResult());

            await Task.WhenAll(firstSubscription.Task, secondSubscription.Task);
        }
    }
}
