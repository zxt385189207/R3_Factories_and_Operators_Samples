using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;
using UnityEngine;

namespace R3_UniRx.Tests.Operators
{
    public sealed class SubscribeAwaitTest
    {
        [Test]
        public void R3_SubscribeAwait_Observableの値を非同期処理と組み合わせて処理する_Sequential()
        {
            var fakeFrameProvider = new FakeFrameProvider();

            using var subject = new R3.Subject<int>();
            var result = new List<int>();

            // Sequentialはキューにつめて直列に１つずつ処理
            subject.SubscribeAwait(async (x, ct) =>
            {
                // フレームが進むのを待つ
                await fakeFrameProvider.WaitAsync(ct: ct);
                result.Add(x);
            }, AwaitOperation.Sequential);

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();

            // まだ結果が出力されていない
            CollectionAssert.IsEmpty(result);

            // 1F進む
            fakeFrameProvider.Advance();

            // [1]が完了して結果が出力される
            // [2]が実行中
            CollectionAssert.AreEqual(new[]
            {
                1
            }, result);

            // 1F進む
            fakeFrameProvider.Advance();

            // [2]が完了して結果が出力される
            // [3]が実行中
            CollectionAssert.AreEqual(new[]
            {
                1,
                2
            }, result);

            // 1F進む
            fakeFrameProvider.Advance();

            // [3]が完了して結果が出力される
            CollectionAssert.AreEqual(new[]
            {
                1,
                2,
                3
            }, result);
        }

        [Test]
        public void R3_SubscribeAwait_Observableの値を非同期処理と組み合わせて処理する_Parallel()
        {
            var fakeFrameProvider = new FakeFrameProvider();

            using var subject = new R3.Subject<int>();
            var result = new List<int>();

            // Parallelは到着したものをそのまま並列で実行
            subject.SubscribeAwait(async (x, ct) =>
            {
                // フレームが進むのを待つ
                await fakeFrameProvider.WaitAsync(ct: ct);
                result.Add(x);
            }, AwaitOperation.Parallel);

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnCompleted();

            // まだ結果が出力されていない
            CollectionAssert.IsEmpty(result);

            // 1F進む
            fakeFrameProvider.Advance();

            // まとめて3つの非同期処理が実行されて完了している
            CollectionAssert.AreEqual(new[]
            {
                1,
                2,
                3
            }, result);
        }

        [Test]
        public void R3_SubscribeAwait_SequentialParallelは使用できず例外()
        {
            Assert.Throws<ArgumentException>(() =>
                Observable.Return(1)
                    .SubscribeAwait((_, _) => UniTask.CompletedTask, AwaitOperation.SequentialParallel));
        }

        [Test]
        public void R3_SubscribeAwait_Observableの値を非同期処理と組み合わせて処理する_Switch()
        {
            var fakeFrameProvider = new FakeFrameProvider();

            using var subject = new R3.Subject<int>();
            var result = new List<int>();

            // Switchは現在実行中の処理をキャンセルして新しい処理を開始
            subject.SubscribeAwait(async (x, ct) =>
            {
                // 2F待つ
                await fakeFrameProvider.WaitAsync(count: 2, ct: ct);
                result.Add(x);
            }, AwaitOperation.Switch);

            subject.OnNext(1);
            
            fakeFrameProvider.Advance();
            
            // [1]がキャンセルされ[2]が実行される
            subject.OnNext(2);
            
            fakeFrameProvider.Advance();

            // [2]がキャンセルされ[3]が実行される
            subject.OnNext(3);
            
            subject.OnCompleted();

            CollectionAssert.IsEmpty(result);
            
            // 2F待つ
            fakeFrameProvider.Advance();
            fakeFrameProvider.Advance();

            // [3] が完了して結果が出力される
            CollectionAssert.AreEqual(new[]
            {
                3
            }, result);
        }

        [Test]
        public void R3_SubscribeAwait_Observableの値を非同期処理と組み合わせて処理する_Drop()
        {
            var fakeFrameProvider = new FakeFrameProvider();

            using var subject = new R3.Subject<int>();
            var result = new List<int>();

            // Dropは現在の非同期処理を優先し、それが終わるまで新しい入力を無視する
            subject.SubscribeAwait(async (x, ct) =>
            {
                // 1F待つ
                await fakeFrameProvider.WaitAsync(count: 1, ct: ct);
                result.Add(x);
            }, AwaitOperation.Drop);

            subject.OnNext(1); // これが優先される
            subject.OnNext(2);
            subject.OnNext(3);
            CollectionAssert.IsEmpty(result);

            fakeFrameProvider.Advance();
            subject.OnCompleted();

            CollectionAssert.AreEqual(new[]
            {
                1
            }, result);
        }

        [Test]
        public void R3_SubscribeAwait_Observableの値を非同期処理と組み合わせて処理する_ThrottleFirstLast()
        {
            var fakeFrameProvider = new FakeFrameProvider();

            using var subject = new R3.Subject<int>();
            var result = new List<int>();

            // ThrottleFirstLast先着したものは非同期処理を確定で実行
            // 非同期処理中に複数届いた場合はその最後のものだけを実行
            subject.SubscribeAwait(async (x, ct) =>
            {
                // 1F待つ
                await fakeFrameProvider.WaitAsync(count: 1, ct: ct);
                result.Add(x);
            }, AwaitOperation.ThrottleFirstLast);

            subject.OnNext(1); // これと
            subject.OnNext(2);
            subject.OnNext(3); // これだけ実行される
            
            CollectionAssert.IsEmpty(result);
            
            // 1F進むので[1]が完了
            fakeFrameProvider.Advance();
            
            CollectionAssert.AreEqual(new[]
            {
                1
            }, result);

            // もう1F進むので[3]が完了、[2]は無視
            fakeFrameProvider.Advance();
            
            CollectionAssert.AreEqual(new[]
            {
                1,
                3
            }, result);
        }

        [Test]
        public void UniRx_SubscribeAwaitは存在しない()
        {
            Assert.Ignore();
        }
    }
}