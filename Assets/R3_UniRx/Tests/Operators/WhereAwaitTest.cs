using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using R3;

namespace R3_UniRx.Tests.Operators
{
    public sealed class WhereAwaitTest
    {
        [Test]
        public void R3_WhereAwait_入力値を非同期処理と組み合わせてフィルタリングする_Sequential()
        {
            using var subject = new R3.Subject<int>();

            var fakeFrameProvider = new FakeFrameProvider();

            // 入力値が偶数のみを通す
            // ただし結果の判定にはその数値分だけのフレーム待機が必要
            // Sequential、1つの要素が完了してから次の要素を処理する
            var list = subject.WhereAwait(async (x, ct) =>
                {
                    await fakeFrameProvider.WaitAsync(x, ct);
                    return x % 2 == 0;
                }, AwaitOperation.Sequential)
                .ToLiveList();

            subject.OnNext(2);
            subject.OnNext(3);
            subject.OnNext(4);
            subject.OnNext(5);
            subject.OnCompleted();

            // まだ結果が出力されていない
            CollectionAssert.IsEmpty(list);

            // +1F
            fakeFrameProvider.Advance();

            // まだ結果が出力されていない
            CollectionAssert.IsEmpty(list);

            // +1F
            fakeFrameProvider.Advance();

            // [2] は条件を満たすので出力される
            CollectionAssert.AreEqual(new[] { 2 }, list);

            // [3] は条件を満たさないが、非同期処理は実行中なので3F待機
            fakeFrameProvider.Advance(3);

            // [4] の非同期処理が開始されているはず

            // +4F
            fakeFrameProvider.Advance(4);

            CollectionAssert.AreEqual(new[] { 2, 4 }, list);

            // +5F
            fakeFrameProvider.Advance(5);

            // [5] は条件を満たさないので通過していない
            CollectionAssert.AreEqual(new[] { 2, 4 }, list);
        }

        [Test]
        public void R3_WhereAwait_入力値を非同期処理と組み合わせてフィルタリングする_Parallel()
        {
            using var subject = new R3.Subject<int>();

            var fakeFrameProvider = new FakeFrameProvider();

            // 入力値が偶数のみを通す
            // ただし結果の判定にはその数値分だけのフレーム待機が必要
            // Sequential、1つの要素が完了してから次の要素を処理する
            var list = subject.WhereAwait(async (x, ct) =>
                {
                    await fakeFrameProvider.WaitAsync(x, ct);
                    return x % 2 == 0;
                }, AwaitOperation.Parallel)
                .ToLiveList();

            // 4つ連続して入力されたのでそれぞれの条件判定がまず実行
            // そのまま非同期処理が必要なものは並行に実行される
            // 結果は先に終わったものから出力される
            subject.OnNext(5); // 4番目に終わるが、条件を満たさない
            subject.OnNext(4); // 3番目に終わり、条件を満たす -> 2nd
            subject.OnNext(3); // 2番目に終わるが、条件を満たさない
            subject.OnNext(2); // 1番目に終わり、条件を満たす  -> 1st
            subject.OnCompleted();

            // まだ結果が出力されていない
            CollectionAssert.IsEmpty(list);

            // 2F待つ
            fakeFrameProvider.Advance(2);

            // [2] が先に終わっている
            CollectionAssert.AreEqual(new[] { 2 }, list);

            // さらに2F待つ
            fakeFrameProvider.Advance(2);

            // [4] が次に終わった、[3]も終わっているが条件を満たさないので無視されている
            CollectionAssert.AreEqual(new[] { 2, 4 }, list);

            // さらに1F待つ
            fakeFrameProvider.Advance();

            // [5] は条件を満たさないので無視されている
            CollectionAssert.AreEqual(new[] { 2, 4 }, list);
        }

        [Test]
        public void R3_WhereAwait_入力値を非同期処理と組み合わせてフィルタリングする_SequentialParallel()
        {
            using var subject = new R3.Subject<int>();

            var fakeFrameProvider = new FakeFrameProvider();

            // 入力値が偶数のみを通す
            // ただし結果の判定にはその数値分だけのフレーム待機が必要
            // Sequential、1つの要素が完了してから次の要素を処理する
            var list = subject.WhereAwait(async (x, ct) =>
                {
                    await fakeFrameProvider.WaitAsync(x, ct);
                    return x % 2 == 0;
                }, AwaitOperation.SequentialParallel)
                .ToLiveList();

            // 4つ連続して入力されたのでそれぞれの条件判定がまず実行
            // そのまま非同期処理が必要なものは並行に実行される
            // 結果は入力順となるように調整される
            subject.OnNext(5); // 4番目に終わるが、条件を満たさない
            subject.OnNext(4); // 3番目に終わり、条件を満たす
            subject.OnNext(3); // 2番目に終わるが、条件を満たさない
            subject.OnNext(2); // 1番目に終わり、条件を満たす 
            subject.OnCompleted();

            // まだ結果が出力されていない
            CollectionAssert.IsEmpty(list);

            // 2F待つ
            fakeFrameProvider.Advance(2);

            // [2] が先に終わっているが、SequentialParallelなので[4]が終わるのを待つ
            CollectionAssert.IsEmpty(list);

            // さらに2F待つ
            fakeFrameProvider.Advance(2);

            // [2]と[3]は完了しているが、[3]は無視されている
            // [2]は条件を満たしているが、[5]の非同期処理が実行中で結果が未確定なのでまだ出力されない
            CollectionAssert.IsEmpty(list);

            // さらに1F待つ
            fakeFrameProvider.Advance();

            // [5] が完了
            // 入力値[5][4][3][2]の結果がすべて揃ったので、その入力順を維持する形で結果を出力
            CollectionAssert.AreEqual(new[] { 4, 2 }, list);
        }

        [Test]
        public void R3_WhereAwait_入力値を非同期処理と組み合わせてフィルタリングする_Switch()
        {
            using var subject = new R3.Subject<int>();

            var fakeFrameProvider = new FakeFrameProvider();

            // 入力値が偶数のみを通す
            // ただし結果の判定にはその数値分だけのフレーム待機が必要
            // Switch、非同期処理が実行中の場合はそれをキャンセルして次の処理にすぐ乗り換える
            var list = subject.WhereAwait(async (x, ct) =>
                {
                    await fakeFrameProvider.WaitAsync(x, ct);
                    return x % 2 == 0;
                }, AwaitOperation.Switch)
                .ToLiveList();


            // 通過するが、4F待機が発生
            subject.OnNext(4);

            // 条件を満たさないが、Switchなので[4]のキャンセルが発動する
            subject.OnNext(1);

            // 4F待機
            fakeFrameProvider.Advance(4);

            // [4]はキャンセルされているので何も出力されていない
            CollectionAssert.IsEmpty(list);

            // 通過するが、2F待機が発生
            subject.OnNext(2);

            // 1F待機
            fakeFrameProvider.Advance();

            // [2]がまだ終わっていないので[2]はキャンセル
            // 新たに[6]の非同期処理が開始
            subject.OnNext(6);

            // 6F待機
            fakeFrameProvider.Advance(6);

            // [6]が終わったので出力される
            CollectionAssert.AreEqual(new[] { 6 }, list);
        }

        [Test]
        public void R3_WhereAwait_入力値を非同期処理と組み合わせてフィルタリングする_Drop()
        {
            using var subject = new R3.Subject<int>();

            var fakeFrameProvider = new FakeFrameProvider();

            // 入力値が偶数のみを通す
            // ただし結果の判定にはその数値分だけのフレーム待機が必要
            // Drop、非同期処理が実行中は新しい入力を条件によらずに無視する
            var list = subject.WhereAwait(async (x, ct) =>
                {
                    await fakeFrameProvider.WaitAsync(x, ct);
                    return x % 2 == 0;
                }, AwaitOperation.Drop)
                .ToLiveList();

            subject.OnNext(2); // 2F待機が必要

            // まだ結果が出力されていない
            CollectionAssert.IsEmpty(list);

            // 1F待つ
            fakeFrameProvider.Advance();

            // まだ出力されない
            CollectionAssert.IsEmpty(list);

            subject.OnNext(3); // これはそもそも条件を満たさない
            subject.OnNext(4); // 条件を満たすがが、[2]の非同期処理が終わっていないので無視される

            // 2F目の待機
            fakeFrameProvider.Advance();

            // [2]が完了
            CollectionAssert.AreEqual(new[] { 2 }, list);

            // しばらく待ってみるが、[4]は出力されないはず
            fakeFrameProvider.Advance(4);

            // 結果は変わらずで[2]のみが出力されている
            CollectionAssert.AreEqual(new[] { 2 }, list);
        }

        [Test]
        public void R3_WhereAwait_入力値を非同期処理と組み合わせてフィルタリングする_ThrottleFirstLast()
        {
            var asyncMethodList = new List<int>();

            using var subject = new R3.Subject<int>();

            var fakeFrameProvider = new FakeFrameProvider();

            // 入力値が偶数のみを通す
            // ただし結果の判定にはその数値分だけのフレーム待機が必要
            // ThrottleFirstLast、最初の入力を優先して非同期処理を行い、結果としては最後の入力も出力する
            var list = subject.WhereAwait(async (x, ct) =>
                {
                    asyncMethodList.Add(x);

                    await fakeFrameProvider.WaitAsync(x, ct);
                    return x % 2 == 0;
                }, AwaitOperation.ThrottleFirstLast)
                .ToLiveList();

            subject.OnNext(4); // 条件を満たす
            subject.OnNext(1); // 条件を満たさない
            subject.OnNext(6); // 条件を満たす
            subject.OnNext(3); // 条件を満たさない
            subject.OnNext(2); // 条件を満たす

            // まだ結果が出力されていない
            CollectionAssert.IsEmpty(list);

            // 3F待つ、まだ[4]が終わっていない
            fakeFrameProvider.Advance(3);

            CollectionAssert.IsEmpty(list);

            // 実行された非同期処理は[4]のみ
            CollectionAssert.AreEqual(new[] { 4 }, asyncMethodList);

            // 1F待つ
            fakeFrameProvider.Advance(1);

            // [4]が完了した
            // 裏では[4]の実行中の最後に入力された[2]の実行が開始されている
            CollectionAssert.AreEqual(new[] { 4 }, list);

            // 非同期処理の実行リストに[2]が追加されている
            CollectionAssert.AreEqual(new[] { 4, 2 }, asyncMethodList);
            
            // 2F待つ
            fakeFrameProvider.Advance(2);

            // [2]が完了したので、先程の[4]と合わせて順番に出力される
            CollectionAssert.AreEqual(new[] { 4, 2 }, list);
            
            // 結果は変わらないが長めに待ってみる
            fakeFrameProvider.Advance(10);
            
            CollectionAssert.AreEqual(new[] { 4, 2 }, list);
            
            // 実際に条件判定された値
            // 先頭と最後のみしか評価されていない
            CollectionAssert.AreEqual(new[] { 4, 2 }, asyncMethodList);
        }

        [Test]
        public void UniRx_WhereAwaitは存在しない()
        {
        }
    }
}