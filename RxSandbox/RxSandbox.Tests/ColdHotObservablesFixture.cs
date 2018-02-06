using NUnit.Framework;
using System;
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

            var coldObservable = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5);

            coldObservable
                .Subscribe(i => TestContext.Out.WriteLine($"First subscription --> {i}"), 
                () => firstSubscription.SetResult());

            await Task.Delay(TimeSpan.FromSeconds(3));

            coldObservable
                .Subscribe(i => TestContext.Out.WriteLine($"Second subscription --> {i}"),
                () => secondSubscription.SetResult());

            await Task.WhenAll(firstSubscription.Task, secondSubscription.Task);
        }

        [Test]
        public async Task PublishAndConnect_TurnsColdObservableIntoHot()
        {
            var firstSubscription = new TaskCompletionSource();
            var secondSubscription = new TaskCompletionSource();

            var hotObservable = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5).Publish();
            hotObservable.Connect();

            hotObservable
                .Subscribe(i => TestContext.Out.WriteLine($"First subscription --> {i}"),
                () => firstSubscription.SetResult());

            await Task.Delay(TimeSpan.FromSeconds(3));

            hotObservable
                .Subscribe(i => TestContext.Out.WriteLine($"Second subscription --> {i}"),
                () => secondSubscription.SetResult());

            await Task.WhenAll(firstSubscription.Task, secondSubscription.Task);
        }

        [Test]
        public async Task Replay_TurnsHotObservableIntoCold()
        {
            var firstSubscription = new TaskCompletionSource();
            var secondSubscription = new TaskCompletionSource();

            var hotObservable = Observable.Interval(TimeSpan.FromSeconds(1)).Take(5).Publish();
            hotObservable.Connect();

            var coldObservable = hotObservable.Replay();
            coldObservable.Connect();

            coldObservable
                .Subscribe(i => TestContext.Out.WriteLine($"First subscription --> {i}"),
                () => firstSubscription.SetResult());

            await Task.Delay(TimeSpan.FromSeconds(3));

            coldObservable
                .Subscribe(i => TestContext.Out.WriteLine($"Second subscription --> {i}"),
                () => secondSubscription.SetResult());

            await Task.WhenAll(firstSubscription.Task, secondSubscription.Task);
        }
    }
}
