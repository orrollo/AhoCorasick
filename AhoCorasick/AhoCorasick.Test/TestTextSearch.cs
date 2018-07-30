using System;
using System.Collections.Generic;
using System.Text;
using AhoCorasick.Core;
using NUnit.Framework;

namespace AhoCorasick.Test
{
    [TestFixture]
    public class TestTextSearch
    {
        [Test]
        public void SingleWordTest()
        {
            var fsa = new SearchFsa<char>();
            fsa.Add("text");
            fsa.Prepare();

            var correct = false;
            var ep = fsa.GetEndPoint((index, context) =>
            {
                correct = true;
            });

            var data = "the search text example";
            foreach (var ch in data) ep.ProcessIncome(ch);

            Assert.AreEqual(correct, true);
        }

        [Test]
        public void SeveralWordsTest()
        {
            var lookingWords = new List<string>() { "text", "search" };

            var fsa = new SearchFsa<char>();
            foreach (var word in lookingWords) fsa.Add(word);
            fsa.Prepare();

            var words = new List<string>();
            var ep = fsa.GetEndPoint((index, context) =>
            {
                var currentWord = context.ToString();
                words.Add(currentWord);
            });

            var data = "the search text example";
            foreach (var ch in data) ep.ProcessIncome(ch);

            foreach (var word in lookingWords) Assert.AreEqual(words.Contains(word), true);
        }

        protected static string Lorem5 = @"
lorem ipsum dolor sit amet, consectetur adipiscing elit. integer nibh metus, tempor ut condimentum at, auctor non urna. suspendisse in mi sagittis, congue ligula id, interdum libero. vivamus ornare, justo non maximus finibus, felis lacus congue eros, eu pretium quam mauris non orci. quisque finibus mollis sapien, in imperdiet ante dapibus vitae. proin metus lectus, rutrum non pharetra imperdiet, tristique sit amet purus. etiam malesuada elit suscipit erat rutrum, sit amet malesuada massa consequat. aliquam erat volutpat. maecenas commodo risus eu leo consectetur, id venenatis ligula laoreet. sed quis lorem consectetur, hendrerit purus id, ullamcorper augue. cras ut quam quis leo rhoncus elementum. cras tempus, justo auctor pulvinar efficitur, lacus lacus malesuada mauris, ac aliquet elit lacus in dolor. donec sed elit non mi vehicula ornare. vestibulum lacinia tortor eget enim condimentum eleifend. cras nisi nulla, varius vehicula aliquet eget, interdum ut massa. cras at congue neque. nam sed vulputate sapien, varius ornare enim.

vestibulum condimentum sagittis tellus, ac efficitur turpis molestie nec. maecenas in ultricies diam, et sollicitudin nulla. nunc tincidunt dolor ultricies, tempus felis vel, vulputate arcu. aenean risus ligula, laoreet quis venenatis eu, tincidunt ac quam. pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. vestibulum nulla nulla, egestas vitae efficitur in, auctor ut libero. aliquam erat volutpat. vestibulum auctor id leo id gravida. lorem ipsum dolor sit amet, consectetur adipiscing elit. maecenas placerat, tortor quis rhoncus venenatis, elit magna pulvinar justo, at bibendum lectus lectus sed eros. interdum et malesuada fames ac ante ipsum primis in faucibus. donec dignissim non sem nec elementum. vestibulum vitae velit quis nisl egestas luctus vel sed nisi.

fusce pharetra enim vestibulum, facilisis augue id, dignissim augue. suspendisse viverra nisi nunc, a maximus tellus elementum congue. phasellus finibus hendrerit massa. nulla pretium quis odio eget molestie. aliquam erat volutpat. nunc tristique libero quam, quis rhoncus urna rutrum ac. fusce ac interdum tellus, non rhoncus velit.

vivamus viverra mattis nibh, id pellentesque erat faucibus eu. sed ullamcorper, justo egestas vehicula lacinia, lorem massa finibus dui, ac rhoncus purus elit id mi. sed auctor blandit eros eu ultrices. orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. phasellus vitae ex pellentesque, suscipit augue sed, rutrum libero. sed et volutpat ante, quis auctor nulla. maecenas viverra id enim sed varius. aliquam luctus vestibulum erat, quis auctor velit pharetra accumsan. donec molestie scelerisque pulvinar. nullam sit amet diam in nibh viverra rhoncus at id arcu.

vestibulum id ex a dui fringilla ultrices. orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. integer rhoncus pretium tincidunt. morbi a risus nec eros vulputate tempus. sed vulputate massa rutrum lorem euismod vestibulum. fusce feugiat elit maximus risus venenatis aliquet. suspendisse viverra sem eget sapien bibendum iaculis. morbi sit amet nisi arcu. donec odio odio, aliquet in dolor ut, varius ornare mi. phasellus ac quam eros. etiam dapibus, tortor in tempus consequat, lacus mauris ullamcorper magna, nec blandit orci sapien eget lacus. ";

        [Test]
        public void LoremSampleTest()
        {
            var lookingWords = new List<string>() { "vitae", "magna", "tempus", "lectus", "varius", "libero", "sed", "nunc" };

            var fsa = new SearchFsa<char>();
            foreach (var word in lookingWords) fsa.Add(word);
            fsa.Prepare();

            var words = new List<string>();
            var ep = fsa.GetEndPoint((index, context) =>
            {
                var currentWord = context.ToString();
                words.Add(currentWord);
            });

            foreach (var ch in Lorem5) ep.ProcessIncome(ch);

            foreach (var word in lookingWords) Assert.AreEqual(words.Contains(word), true);
        }

        [Test]
        public void OverlappingTextTest()
        {
            var lookingWords = new List<string>() { "test", "tester", "eres", "este", "est", "ste" };

            var fsa = new SearchFsa<char>();
            foreach (var word in lookingWords) fsa.Add(word);
            fsa.Prepare();

            var words = new List<string>();
            var ep = fsa.GetEndPoint((index, context) =>
            {
                var currentWord = context.ToString();
                words.Add(currentWord);
            });

            var data = "tetetestesterest";
            foreach (var ch in data) ep.ProcessIncome(ch);

            foreach (var word in lookingWords) Assert.AreEqual(words.Contains(word), true);
        }


    }
}
