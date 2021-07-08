// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.WinUI.Notifications;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Notifications
{
    [TestClass]
    public class TestToastArguments
    {
        [TestMethod]
        public void TestAddExceptions_NullName()
        {
            ToastArguments query = new ToastArguments();

            try
            {
                query.Add(null, "value");
            }
            catch (ArgumentNullException)
            {
                return;
            }

            Assert.Fail("Adding null name shouldn't be allowed.");
        }

        [TestMethod]
        public void TestParsing()
        {
            AssertParse(new ToastArguments(), string.Empty);
            AssertParse(new ToastArguments(), "   ");
            AssertParse(new ToastArguments(), "\n");
            AssertParse(new ToastArguments(), "\t \n");
            AssertParse(new ToastArguments(), null);

            AssertParse(
                new ToastArguments()
                {
                    { "isBook" }
                }, "isBook");

            AssertParse(
                new ToastArguments()
                {
                    { "isBook" },
                    { "isRead" }
                }, "isBook;isRead");

            AssertParse(
                new ToastArguments()
                {
                    { "isBook" },
                    { "isRead" },
                    { "isLiked" }
                }, "isBook;isRead;isLiked");

            AssertParse(
                new ToastArguments()
                {
                    { "name", "Andrew" }
                }, "name=Andrew");

            AssertParse(
                new ToastArguments()
                {
                    { "name", "Andrew" },
                    { "isAdult" }
                }, "name=Andrew;isAdult");

            AssertParse(
                new ToastArguments()
                {
                    { "name", "Andrew" },
                    { "isAdult" }
                }, "isAdult;name=Andrew");

            AssertParse(
                new ToastArguments()
                {
                    { "name", "Andrew" },
                    { "age", "22" }
                }, "age=22;name=Andrew");
        }

        [TestMethod]
        public void TestToString_ExactString()
        {
            Assert.AreEqual(string.Empty, new ToastArguments().ToString());

            Assert.AreEqual("isBook", new ToastArguments()
            {
                { "isBook" }
            }.ToString());

            Assert.AreEqual("name=Andrew", new ToastArguments()
            {
                { "name", "Andrew" }
            }.ToString());
        }

        [TestMethod]
        public void TestSpecialCharacters()
        {
            Assert.AreEqual("full name=Andrew Leader", new ToastArguments()
            {
                { "full name", "Andrew Leader" }
            }.ToString());

            Assert.AreEqual("name%3Bcompany=Andrew%3BMicrosoft", new ToastArguments()
            {
                { "name;company", "Andrew;Microsoft" }
            }.ToString());

            Assert.AreEqual("name/company=Andrew/Microsoft", new ToastArguments()
            {
                { "name/company", "Andrew/Microsoft" }
            }.ToString());

            Assert.AreEqual("message=Dinner?", new ToastArguments()
            {
                { "message", "Dinner?" }
            }.ToString());

            Assert.AreEqual("message=to: Andrew", new ToastArguments()
            {
                { "message", "to: Andrew" }
            }.ToString());

            Assert.AreEqual("email=andrew@live.com", new ToastArguments()
            {
                { "email", "andrew@live.com" }
            }.ToString());

            Assert.AreEqual("messsage=food%3Dyummy", new ToastArguments()
            {
                { "messsage", "food=yummy" }
            }.ToString());

            Assert.AreEqual("messsage=$$$", new ToastArguments()
            {
                { "messsage", "$$$" }
            }.ToString());

            Assert.AreEqual("messsage=-_.!~*'()", new ToastArguments()
            {
                { "messsage", "-_.!~*'()" }
            }.ToString());

            Assert.AreEqual("messsage=this & that", new ToastArguments()
            {
                { "messsage", "this & that" }
            }.ToString());

            Assert.AreEqual("messsage=20%25 off!", new ToastArguments()
            {
                { "messsage", "20% off!" }
            }.ToString());

            Assert.AreEqual("messsage=Nonsense %2526 %2525 %253D", new ToastArguments()
            {
                { "messsage", "Nonsense %26 %25 %3D" }
            }.ToString());
        }

        [TestMethod]
        public void TestDecoding()
        {
            AssertDecode("Hello world", "Hello world");

            AssertDecode(";/?:@&=+$%", "%3B/?:@&%3D+$%25");
            AssertDecode("-_.!~*'()", "-_.!~*'()");
        }

#if !WINRT
        [TestMethod]
        public void TestIndexer_NullException()
        {
            try
            {
                string val = new ToastArguments()[null];
            }
            catch (ArgumentNullException)
            {
                return;
            }

            Assert.Fail("Exception should have been thrown.");
        }

        [TestMethod]
        public void TestIndexer_NotFoundException()
        {
            try
            {
                var args = new ToastArguments()
                {
                    { "name", "Andrew" }
                };

                _ = args["Name"];
            }
            catch (KeyNotFoundException)
            {
                return;
            }

            Assert.Fail("Exception should have been thrown.");
        }

        [TestMethod]
        public void TestIndexer()
        {
            AssertIndexer(null, "isBook;name=Andrew", "isBook");

            AssertIndexer("Andrew", "isBook;name=Andrew", "name");

            AssertIndexer("Andrew", "count=2;name=Andrew", "name");
        }
#endif

        [TestMethod]
        public void TestRemove_OnlyKey()
        {
            ToastArguments qs = new ToastArguments()
            {
                { "name", "Andrew" },
                { "age", "22" }
            };

            Assert.IsTrue(qs.Remove("age"));

            AssertEqual(
                new ToastArguments()
                {
                    { "name", "Andrew" }
                }, qs);

            Assert.IsFalse(qs.Remove("age"));
        }

        [TestMethod]
        public void TestContains()
        {
            ToastArguments qs = new ToastArguments();

            Assert.IsFalse(qs.Contains("name"));
            Assert.IsFalse(qs.Contains("name", "Andrew"));

            qs.Add("isBook");

            Assert.IsFalse(qs.Contains("name"));
            Assert.IsFalse(qs.Contains("name", "Andrew"));

            Assert.IsTrue(qs.Contains("isBook"));
            Assert.IsTrue(qs.Contains("isBook", null));
            Assert.IsFalse(qs.Contains("isBook", "True"));

            qs.Add("isBook", "True");

            Assert.IsTrue(qs.Contains("isBook"));
            Assert.IsFalse(qs.Contains("isBook", null));
            Assert.IsTrue(qs.Contains("isBook", "True"));

            qs.Add("name", "Andrew");

            Assert.IsTrue(qs.Contains("name"));
            Assert.IsFalse(qs.Contains("name", null)); // Value doesn't exist
            Assert.IsTrue(qs.Contains("name", "Andrew"));
            Assert.IsFalse(qs.Contains("Name", "Andrew")); // Wrong case on name
            Assert.IsFalse(qs.Contains("name", "andrew")); // Wrong case on value
            Assert.IsFalse(qs.Contains("Name")); // Wrong case on name
        }

        [TestMethod]
        public void TestAdd()
        {
            ToastArguments qs = new ToastArguments();

            qs.Add("name", "Andrew");

            AssertEqual(
                new ToastArguments()
                {
                    { "name", "Andrew" }
                }, qs);

            qs.Add("age", "22");

            AssertEqual(
                new ToastArguments()
                {
                    { "name", "Andrew" },
                    { "age", "22" }
                }, qs);

            qs.Add("name", "Lei");

            AssertEqual(
                new ToastArguments()
                {
                    { "name", "Lei" },
                    { "age", "22" }
                }, qs);

            string nullStr = null;
            qs.Add("name", nullStr);

            AssertEqual(
                new ToastArguments()
                {
                    { "name" },
                    { "age", "22" }
                }, qs);
        }

        [TestMethod]
        public void TestEnumerator()
        {
            KeyValuePair<string, string>[] parameters = ToastArguments.Parse("name=Andrew;age=22;isOld").ToArray();

            Assert.AreEqual(3, parameters.Length);
            Assert.AreEqual(1, parameters.Count(i => i.Key.Equals("name")));
            Assert.AreEqual(1, parameters.Count(i => i.Key.Equals("age")));
            Assert.AreEqual(1, parameters.Count(i => i.Key.Equals("isOld")));
            Assert.IsTrue(parameters.Any(i => i.Key.Equals("name") && i.Value.Equals("Andrew")));
            Assert.IsTrue(parameters.Any(i => i.Key.Equals("age") && i.Value.Equals("22")));
            Assert.IsTrue(parameters.Any(i => i.Key.Equals("isOld") && i.Value == null));
        }

        [TestMethod]
        public void TestCount()
        {
            ToastArguments qs = new ToastArguments();

            Assert.AreEqual(0, qs.Count);

            qs.Add("name", "Andrew");

            Assert.AreEqual(1, qs.Count);

            qs.Add("age", "22");

            Assert.AreEqual(2, qs.Count);

            qs.Remove("age");

            Assert.AreEqual(1, qs.Count);

            qs.Remove("name");

            Assert.AreEqual(0, qs.Count);
        }

        [TestMethod]
        public void TestStronglyTyped()
        {
            ToastArguments args = new ToastArguments()
                .Add("isAdult", true)
                .Add("isPremium", false)
                .Add("age", 22)
                .Add("level", 0)
                .Add("gpa", 3.97)
                .Add("percent", 97.3f);

#if !WINRT
            args.Add("activationKind", ToastActivationType.Background);
#endif

            AssertEqual(
                new ToastArguments()
                {
                    { "isAdult", "1" },
                    { "isPremium", "0" },
                    { "age", "22" },
                    { "level", "0" },
                    { "gpa", "3.97" },
                    { "percent", "97.3" },
#if !WINRT
                    { "activationKind", "1" }
#endif
                }, args);

            Assert.AreEqual(true, args.GetBool("isAdult"));
            Assert.AreEqual("1", args.Get("isAdult"));

            Assert.AreEqual(false, args.GetBool("isPremium"));
            Assert.AreEqual("0", args.Get("isPremium"));

            Assert.AreEqual(22, args.GetInt("age"));
            Assert.AreEqual(22d, args.GetDouble("age"));
            Assert.AreEqual(22f, args.GetFloat("age"));
            Assert.AreEqual("22", args.Get("age"));

            Assert.AreEqual(0, args.GetInt("level"));

            Assert.AreEqual(3.97d, args.GetDouble("gpa"));

            Assert.AreEqual(97.3f, args.GetFloat("percent"));

#if !WINRT
            Assert.AreEqual(ToastActivationType.Background, args.GetEnum<ToastActivationType>("activationKind"));

            if (args.TryGetValue("activationKind", out ToastActivationType activationType))
            {
                Assert.AreEqual(ToastActivationType.Background, activationType);
            }
            else
            {
                Assert.Fail("TryGetValue as enum failed");
            }

            // Trying to get enum that isn't an enum should return false
            Assert.IsFalse(args.TryGetValue("percent", out activationType));
#endif

            // After serializing and deserializing, the same should work
            args = ToastArguments.Parse(args.ToString());

            Assert.AreEqual(true, args.GetBool("isAdult"));
            Assert.AreEqual("1", args.Get("isAdult"));

            Assert.AreEqual(false, args.GetBool("isPremium"));
            Assert.AreEqual("0", args.Get("isPremium"));

            Assert.AreEqual(22, args.GetInt("age"));
            Assert.AreEqual(22d, args.GetDouble("age"));
            Assert.AreEqual(22f, args.GetFloat("age"));
            Assert.AreEqual("22", args.Get("age"));

            Assert.AreEqual(0, args.GetInt("level"));

            Assert.AreEqual(3.97d, args.GetDouble("gpa"));

            Assert.AreEqual(97.3f, args.GetFloat("percent"));

#if !WINRT
            Assert.AreEqual(ToastActivationType.Background, args.GetEnum<ToastActivationType>("activationKind"));

            if (args.TryGetValue("activationKind", out activationType))
            {
                Assert.AreEqual(ToastActivationType.Background, activationType);
            }
            else
            {
                Assert.Fail("TryGetValue as enum failed");
            }

            // Trying to get enum that isn't an enum should return false
            Assert.IsFalse(args.TryGetValue("percent", out activationType));
#endif
        }

#if !WINRT
        private static void AssertIndexer(string expected, string queryString, string paramName)
        {
            ToastArguments q = ToastArguments.Parse(queryString);

            Assert.AreEqual(expected, q[paramName]);
        }
#endif

        private static void AssertDecode(string expected, string encoded)
        {
            Assert.AreEqual(expected, ToastArguments.Parse("message=" + encoded).Get("message"));
        }

        private static void AssertParse(ToastArguments expected, string inputQueryString)
        {
            Assert.IsTrue(IsSame(expected, ToastArguments.Parse(inputQueryString)), "Expected: " + expected + "\nActual: " + inputQueryString);
        }

        private static void AssertEqual(ToastArguments expected, ToastArguments actual)
        {
            Assert.IsTrue(IsSame(expected, actual), "Expected: " + expected + "\nActual: " + actual);

            Assert.IsTrue(IsSame(expected, ToastArguments.Parse(actual.ToString())), "After serializing and parsing actual, result changed.\n\nExpected: " + expected + "\nActual: " + ToastArguments.Parse(actual.ToString()));
            Assert.IsTrue(IsSame(ToastArguments.Parse(expected.ToString()), actual), "After serializing and parsing expected, result changed.\n\nExpected: " + ToastArguments.Parse(expected.ToString()) + "\nActual: " + actual);
            Assert.IsTrue(IsSame(ToastArguments.Parse(expected.ToString()), ToastArguments.Parse(actual.ToString())), "After serializing and parsing both, result changed.\n\nExpected: " + ToastArguments.Parse(expected.ToString()) + "\nActual: " + ToastArguments.Parse(actual.ToString()));
        }

        private static bool IsSame(ToastArguments expected, ToastArguments actual)
        {
            if (expected.Count != actual.Count)
            {
                return false;
            }

            foreach (var pair in expected)
            {
                if (!actual.Contains(pair.Key))
                {
                    return false;
                }

                if (actual.Get(pair.Key) != pair.Value)
                {
                    return false;
                }
            }

            return true;
        }
    }
}