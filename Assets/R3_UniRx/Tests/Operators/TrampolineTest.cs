using System.Collections.Generic;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_UniRx.Tests.Operators
{
    public sealed class TrampolineTest
    {
        [Test]
        public void R3_Trampoline_Trampolineを使わない場合は即時再帰する()
        {
            // 比較用のTrampolineを使わない場合の挙動の解説。

            var subject = new R3.Subject<int>();

            var resultA = new List<int>();
            var resultB = new List<int>();

            // Subscription A
            subject.Subscribe(x =>
            {
                // 発行結果を記録
                resultA.Add(x);

                // もしxが10未満なら10倍したものを再発行
                if (x < 10)
                {
                    subject.OnNext(x * 10);
                }
            });

            // Subscription B
            subject.Subscribe(x =>
            {
                // Bは記録するのみ
                resultB.Add(x);
            });

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            subject.OnCompleted();

            // Aは受け取った値を記録したあと、それを10倍して再発行する
            // そのため [1] -> 10 -> [2] -> 20 -> [3] -> 30 という順序になる
            CollectionAssert.AreEqual(new[] { 1, 10, 2, 20, 3, 30 }, resultA);

            // Bは受け取った値をそのまま記録するだけだが、Aより購読タイミングが遅い
            // そのためA側での再帰の発動が優先してしまい、値の割り込みが発生する
            CollectionAssert.AreEqual(new[] { 10, 1, 20, 2, 30, 3 }, resultB);
        }

        [Test]
        public void R3_Trampoline_Trampolineを使うと末尾再帰する()
        {
            var subject = new R3.Subject<int>();

            var resultA = new List<int>();
            var resultB = new List<int>();

            // Trampolineを使うと末尾再帰する
            // また正しく機能させるためにはShare()が必要
            var shareObservable = subject.Trampoline().Share();

            // Subscription A
            shareObservable.Subscribe(x =>
            {
                // 発行結果を記録
                resultA.Add(x);

                // もしxが10未満なら10倍したものを再発行
                if (x < 10)
                {
                    subject.OnNext(x * 10);
                }
            });

            // Subscription B
            shareObservable.Subscribe(x =>
            {
                // Bは記録するのみ
                resultB.Add(x);
            });

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            subject.OnCompleted();

            // Aは受け取った値を記録したあと、それを10倍して再発行する
            // そのため [1] -> 10 -> [2] -> 20 -> [3] -> 30 という順序になる
            CollectionAssert.AreEqual(new[] { 1, 10, 2, 20, 3, 30 }, resultA);

            // Trampolineを使うと末尾再帰する
            // まずは元の入力値の処理が優先され、その後に再帰した値が処理される
            // そのため順序が保持されてAと同じ  [1] -> 10 -> [2] -> 20 -> [3] -> 30 になる
            CollectionAssert.AreEqual(new[] { 1, 10, 2, 20, 3, 30 }, resultB);
        }

        [Test]
        public void UniRx_TrampolineはObserveOnで代用可能()
        {
            var subject = new UniRx.Subject<int>();

            var resultA = new List<int>();
            var resultB = new List<int>();

            // ObserveOn(Scheduler.CurrentThread)がTrampolineと同じ効果を持つ
            var shareObservable = subject.ObserveOn(Scheduler.CurrentThread).Share();

            // Subscription A
            shareObservable.Subscribe(x =>
            {
                // 発行結果を記録
                resultA.Add(x);

                // もしxが10未満なら10倍したものを再発行
                if (x < 10)
                {
                    subject.OnNext(x * 10);
                }
            });

            // Subscription B
            shareObservable.Subscribe(x =>
            {
                // Bは記録するのみ
                resultB.Add(x);
            });

            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);

            subject.OnCompleted();

            CollectionAssert.AreEqual(new[] { 1, 10, 2, 20, 3, 30 }, resultA);
            CollectionAssert.AreEqual(new[] { 1, 10, 2, 20, 3, 30 }, resultB);
        }
    }
}