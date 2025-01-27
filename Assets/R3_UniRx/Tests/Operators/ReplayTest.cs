using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class ReplayTest
    {
        [Test]
        public void R3_Replay_Observableを別のReplaySubjectへ流し込む()
        {
            // もとになる方
            using var parentSubject = new R3.Subject<int>();

            // Replayを挟む、この時点では接続していない
            // ConnectableObservable<int>である
            ConnectableObservable<int> connectableObservable = parentSubject.Replay();

            // 接続前に発行する(targetSubjectには流れないはず)
            parentSubject.OnNext(1);
            parentSubject.OnNext(2);
            parentSubject.OnNext(3);

            // Connectして接続する
            var disposable = connectableObservable.Connect();

            // 接続後に発行する(targetSubjectに流れる)
            parentSubject.OnNext(4);
            parentSubject.OnNext(5);
            parentSubject.OnNext(6);

            // 遅れてtargetSubjectを購読する
            // Connect以降にparentSubjectに発行された値が流れているはず
            var list = connectableObservable.ToLiveList();

            CollectionAssert.AreEqual(new[] { 4, 5, 6 }, list);

            parentSubject.OnNext(7); // 追加
            CollectionAssert.AreEqual(new[] { 4, 5, 6, 7 }, list);

            // Disposeすると接続が切れる
            disposable.Dispose();

            parentSubject.OnNext(8); // 追加

            // Disposeした後は流れない
            CollectionAssert.AreEqual(new[] { 4, 5, 6, 7 }, list);
        }

        [Test]
        public void UniRx_Replay()
        {
            // もとになる方
            using var parentSubject = new UniRx.Subject<int>();

            // Replayで繋ぎこむ、この時点では接続していない
            // IConnectableObservable<int>である
            IConnectableObservable<int> connectableObservable = parentSubject.Replay();

            // 接続前に発行する(targetSubjectには流れないはず)
            parentSubject.OnNext(1);
            parentSubject.OnNext(2);
            parentSubject.OnNext(3);

            // Connectして接続する
            var disposable = connectableObservable.Connect();

            // 接続後に発行する(targetSubjectに流れる)
            parentSubject.OnNext(4);
            parentSubject.OnNext(5);
            parentSubject.OnNext(6);

            // 遅れてtargetSubjectを購読する
            // Connect以降にparentSubjectに発行された値が流れているはず
            var list = new List<int>();
            connectableObservable.Subscribe(list.Add);

            CollectionAssert.AreEqual(new[] { 4, 5, 6 }, list);

            parentSubject.OnNext(7); // 追加
            CollectionAssert.AreEqual(new[] { 4, 5, 6, 7 }, list);

            // Disposeすると接続が切れる
            disposable.Dispose();

            parentSubject.OnNext(8); // 追加

            // Disposeした後は流れない
            CollectionAssert.AreEqual(new[] { 4, 5, 6, 7 }, list);
        }
    }
}