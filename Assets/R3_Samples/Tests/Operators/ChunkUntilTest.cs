using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using R3;
using UniRx;

namespace R3_Samples.Tests.Operators
{
    public class ChunkUntilTest
    {
        [Test]
        public void R3_ChunkUntil_条件を満たすまで値をまとめる()
        {
            using var subject = new R3.Subject<string>();

            // "."が出現するまでの値をまとめる
            using var liveList = subject.ChunkUntil(x => x == ".").ToLiveList();

            subject.OnNext("Hello, ");
            subject.OnNext("World");
            subject.OnNext(".");
            subject.OnNext("This ");
            subject.OnNext("is ");
            subject.OnNext("R3");
            subject.OnNext(".");
            subject.OnCompleted();

            Assert.AreEqual(2, liveList.Count);

            var firstChunk = liveList[0].ToArray();
            Assert.AreEqual(3, firstChunk.Length);
            Assert.AreEqual("Hello, ", firstChunk[0]);
            Assert.AreEqual("World", firstChunk[1]);
            Assert.AreEqual(".", firstChunk[2]);

            var secondChunk = liveList[1].ToArray();
            Assert.AreEqual(4, secondChunk.Length);
            Assert.AreEqual("This ", secondChunk[0]);
            Assert.AreEqual("is ", secondChunk[1]);
            Assert.AreEqual("R3", secondChunk[2]);
            Assert.AreEqual(".", secondChunk[3]);
        }


        [Test]
        public void UniRx_ChunkUntil相当は存在しないでのBufferで再現する()
        {
            // Bufferでがんばれば再現はできるが、実用的では無さそう

            using var subject = new UniRx.Subject<string>();

            // 今回のケースではShare()は不要だが、状況によっては挟んだほうがいい
            var sharedObservable = subject.Share();

            var results = new List<IList<string>>();
            
            sharedObservable
                .Buffer(sharedObservable.Where(x => x == "."))
                .Subscribe(x => results.Add(x));
            
            subject.OnNext("Hello, ");
            subject.OnNext("World");
            subject.OnNext(".");
            subject.OnNext("This ");
            subject.OnNext("is ");
            subject.OnNext("R3");
            subject.OnNext(".");
            
            Assert.AreEqual(2, results.Count);
            
            var firstChunk = results[0];
            Assert.AreEqual(3, firstChunk.Count);
            Assert.AreEqual("Hello, ", firstChunk[0]);
            Assert.AreEqual("World", firstChunk[1]);
            Assert.AreEqual(".", firstChunk[2]);
            
            var secondChunk = results[1];
            Assert.AreEqual(4, secondChunk.Count);
            Assert.AreEqual("This ", secondChunk[0]);
            Assert.AreEqual("is ", secondChunk[1]);
            Assert.AreEqual("R3", secondChunk[2]);
            Assert.AreEqual(".", secondChunk[3]);
            
        }
    }
}