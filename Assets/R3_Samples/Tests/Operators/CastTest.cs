using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using R3.Collections;
using UniRx;
using Observable = UniRx.Observable;

namespace R3_Samples.Tests.Operators
{
    public class CastTest
    {
        [Test]
        public void R3_Cast_OnNextの型を指定した型にキャストする()
        {
            // object型が流れるSubject
        using var subject = new R3.Subject<object>();

            // int型にキャストする
            var castObservable = subject.Cast<object, int>();

            // 発行されたメッセージをすべて格納する
            LiveList<R3.Notification<int>> list = castObservable.Materialize().ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext("3");
            subject.OnNext(4);
            subject.OnCompleted();

            Assert.AreEqual(1, list[0].Value);
            Assert.AreEqual(2, list[1].Value);
            // キャストできない"3"はOnErrorResumeが発行されている
            Assert.AreEqual(R3.NotificationKind.OnErrorResume, list[2].Kind);
            Assert.AreEqual(4, list[3].Value);
            Assert.AreEqual(R3.NotificationKind.OnCompleted, list[4].Kind);
        }


        [Test]
        public async Task UniRx_CastTest()
        {
            // UniRxはCast失敗時にOnErrorとなりそこでObservableは停止する
            var result = await Observable.Create<object>(observer =>
                {
                    observer.OnNext(1);
                    observer.OnNext(2);
                    observer.OnNext("3");
                    observer.OnNext(4);
                    observer.OnCompleted();
                    return UniRx.Disposable.Empty;
                })
                .Cast<object, int>()
                .Materialize()
                .ToArray()
                .ToTask();

            Assert.AreEqual(1, result[0].Value);
            Assert.AreEqual(2, result[1].Value);
            Assert.AreEqual(UniRx.NotificationKind.OnError, result[2].Kind);
            Assert.AreEqual(3, result.Length); // 3以降は流れていない
        }
    }
}