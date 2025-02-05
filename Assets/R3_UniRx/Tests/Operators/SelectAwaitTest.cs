using System.Collections.Generic;
using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Operators
{
    public sealed class SelectAwaitTest
    {
        [Test]
        public void R3_SelectAwait_入力値を非同期で変換して出力する_Sequential()
        {
            using var subject = new R3.Subject<int>();

            var fakeFrameProvider = new FakeFrameProvider();
            
            // 入力されたフレーム数だけ待機してから、結果を出力する
            // Sequential、1つの要素が完了してから次の要素を処理する
            using var list = subject.SelectAwait(async (x, ct) =>
                {
                    // xフレーム待つ
                    await fakeFrameProvider.WaitAsync(x, ct);
                    return x;
                }, AwaitOperation.Sequential)
                .ToLiveList();

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnCompleted();

            // まだ結果が出力されていない
            CollectionAssert.IsEmpty(list);

            // 1F待つ
            fakeFrameProvider.Advance();

            // 1F経過したので1が出力される
            CollectionAssert.AreEqual(new[] { 1 }, list);

            // --このタイミングではまだ2F待機の実行中--

            // 1F目の待機
            fakeFrameProvider.Advance();

            // まだ結果が出力されていない
            CollectionAssert.AreEqual(new[] { 1 }, list);

            // 2F目の待機
            fakeFrameProvider.Advance();

            // 結果が出力される
            CollectionAssert.AreEqual(new[] { 1, 2 }, list);
        }

        [Test]
        public void R3_SelectAwait_入力値を非同期で変換して出力する_Parallel()
        {
            using var subject = new R3.Subject<int>();
            var fakeFrameProvider = new FakeFrameProvider();

            // 入力されたフレーム数だけ待機してから、結果を出力する
            // Parallel、一斉に処理を行う
            using var list = subject.SelectAwait(async (x, ct) =>
                {
                    // xフレーム待つ
                    await fakeFrameProvider.WaitAsync(x, ct);
                    return x;
                }, AwaitOperation.Parallel)
                .ToLiveList();

            // 3つまとめて発行されたので、同時に3️つの非同期処理が開始される
            // OnNextの順序に関係なく、先に終わったものから結果が出力される
            subject.OnNext(3);
            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnCompleted();

            // まだ結果が出力されていない
            CollectionAssert.IsEmpty(list);

            // 1F待つ
            fakeFrameProvider.Advance();
            
            CollectionAssert.AreEqual(new[] { 1 }, list);

            // 2F目の待機
            fakeFrameProvider.Advance();
            
            CollectionAssert.AreEqual(new[] { 1, 2 }, list);

            // 3F目の待機
            fakeFrameProvider.Advance();
            
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, list);
        }

        [Test]
        public void R3_SelectAwait_入力値を非同期で変換して出力する_SequentialParallel()
        {
            using var subject = new R3.Subject<int>();
            var fakeFrameProvider = new FakeFrameProvider();
            
            // 入力されたフレーム数だけ待機してから、結果を出力する
            // Parallel、一斉に処理を行う
            using var list = subject.SelectAwait(async (x, ct) =>
                {
                    // xフレーム待つ
                    await fakeFrameProvider.WaitAsync(x, ct);
                    return x;
                }, AwaitOperation.SequentialParallel)
                .ToLiveList();

            // 3つまとめて発行されたので、同時に3️つの非同期処理が開始される
            // SequentialParallelはOnNextの入力順を維持にして結果を出力する
            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(1);

            // まだ結果が出力されていない
            CollectionAssert.IsEmpty(list);

            // 1F待つ
            fakeFrameProvider.Advance();

            // 処理は1つ終わっているはずだが、まだ出力されない
            CollectionAssert.IsEmpty(list);

            // 2F目の待機
            fakeFrameProvider.Advance();

            // [2]の処理は先頭なので、終わったタイミングでまず出力される
            CollectionAssert.AreEqual(new[] { 2 }, list);

            // 3F目の待機
            fakeFrameProvider.Advance();

            // [3]が終わったので出力され、続いて[1]が出力される
            CollectionAssert.AreEqual(new[] { 2, 3, 1 }, list);
        }

        [Test]
        public void R3_SelectAwait_入力値を非同期で変換して出力する_Switch()
        {
            using var subject = new R3.Subject<int>();
            var fakeFrameProvider = new FakeFrameProvider();
            
            // 入力されたフレーム数だけ待機してから、結果を出力する
            // Switchは新しい入力が来たら、前の処理をキャンセルして新しい処理を開始する
            using var list = subject.SelectAwait(async (x, ct) =>
                {
                    // xフレーム待つ
                    await fakeFrameProvider.WaitAsync(x, ct);
                    return x;
                }, AwaitOperation.Switch)
                .ToLiveList();

            subject.OnNext(3);

            // まだ結果が出力されていない
            CollectionAssert.IsEmpty(list);

            // 1F待つ
            fakeFrameProvider.Advance();

            // [3]の処理中だが次の入力が来たのでキャンセルされ、[2]が処理される
            subject.OnNext(2);

            // まだ出力されない
            CollectionAssert.IsEmpty(list);

            // 2F目の待機
            fakeFrameProvider.Advance();

            // [2]の処理中だが次の入力が来たのでキャンセルされ、[1]が処理される
            subject.OnNext(1);

            // まだ出力されない
            CollectionAssert.IsEmpty(list);

            // 3F目の待機
            fakeFrameProvider.Advance();

            // [1]が終わったので出力される
            CollectionAssert.AreEqual(new[] { 1 }, list);
        }

        [Test]
        public void R3_SelectAwait_入力値を非同期で変換して出力する_Drop()
        {
            using var subject = new R3.Subject<int>();
            var fakeFrameProvider = new FakeFrameProvider();
            
            // 入力されたフレーム数だけ待機してから、結果を出力する
            // Dropは現在の非同期処理を優先し、新しい入力は無視する
            using var list = subject.SelectAwait(async (x, ct) =>
                {
                    // xフレーム待つ
                    await fakeFrameProvider.WaitAsync(x, ct);
                    return x;
                }, AwaitOperation.Drop)
                .ToLiveList();

            subject.OnNext(2); // 先着する[2]が優先される
            subject.OnNext(3);
            subject.OnNext(1);

            // まだ結果が出力されていない
            CollectionAssert.IsEmpty(list);

            // 1F待つ
            fakeFrameProvider.Advance();

            // まだ出力されない
            CollectionAssert.IsEmpty(list);

            // 2F目の待機
            fakeFrameProvider.Advance();

            // [2]が完了
            CollectionAssert.AreEqual(new[] { 2 }, list);

            // しばらく待ってみる
            fakeFrameProvider.Advance();

            // 結果は変わらず
            CollectionAssert.AreEqual(new[] { 2 }, list);
        }

        [Test]
        public void R3_SelectAwait_入力値を非同期で変換して出力する_ThrottleFirstLast()
        {
            using var subject = new R3.Subject<int>();
            var fakeFrameProvider = new FakeFrameProvider();
            
            var asyncMethodList = new List<int>();

            // 入力されたフレーム数だけ待機してから、結果を出力する
            // ThrottleFirstLastは最初の入力を優先して非同期処理を行い、結果としては最後の入力も出力する
            using var list = subject.SelectAwait(async (x, ct) =>
                {
                    // 実行された順番を記録
                    asyncMethodList.Add(x);

                    // xフレーム待つ
                    await fakeFrameProvider.WaitAsync(x, ct);
                    return x;
                }, AwaitOperation.ThrottleFirstLast)
                .ToLiveList();

            subject.OnNext(2);
            subject.OnNext(1);
            subject.OnNext(3);

            // まだ結果が出力されていない
            CollectionAssert.IsEmpty(list);

            // 1F待つ
            fakeFrameProvider.Advance();
            
            // まだ結果が出力されていない
            CollectionAssert.IsEmpty(list);

            // 2F目の待機
            fakeFrameProvider.Advance();

            // [2]
            CollectionAssert.AreEqual(new[] { 2 }, list);

            // --- ここから[3]の処理が開始される ---

            fakeFrameProvider.Advance();
            CollectionAssert.AreEqual(new[] { 2 }, list);
            fakeFrameProvider.Advance();
            CollectionAssert.AreEqual(new[] { 2 }, list);
            fakeFrameProvider.Advance();

            // [3] が終わったので出力される
            CollectionAssert.AreEqual(new[] { 2, 3 }, list);

            // [1] はそもそも実行されていない
            CollectionAssert.AreEqual(new[] { 2, 3 }, asyncMethodList);
        }

        [Test]
        public void UniRx_SelectAwaitは存在しない()
        {
        }
    }
}