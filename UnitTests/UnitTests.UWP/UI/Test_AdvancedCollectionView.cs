// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;

namespace UnitTests.UI
{
    [TestClass]
    public class Test_AdvancedCollectionView
    {
        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_ParameterlessCtor()
        {
            var a = new AdvancedCollectionView()
            {
                Filter = (x) => x.ToString().Length < 5
            };

            a.Source = new List<string>
            {
                "lorem",
                "ipsum",
                "dolor",
                "sit",
                "amet"
            };

            Assert.AreEqual(2, a.Count);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Filter()
        {
            var l = new List<string>
            {
                "lorem",
                "ipsum",
                "dolor",
                "sit",
                "amet"
            };

            var a = new AdvancedCollectionView(l)
            {
                Filter = (x) => x.ToString().Length < 5
            };

            Assert.AreEqual(2, a.Count);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Filter_Preserves_Order()
        {
            var l = new ObservableCollection<string>
            {
                "lorem",
                "ipsum",
                "dolor",
                "sit",
                "amet"
            };

            var a = new AdvancedCollectionView(l)
            {
                Filter = (x) => x.ToString().Length < 5
            };

            Assert.AreEqual(2, a.Count);
            Assert.AreEqual("sit", a[0]);
            Assert.AreEqual("amet", a[1]);

            a.Insert(4, "how");

            Assert.AreEqual(3, a.Count);
            Assert.AreEqual("sit", a[0]);
            Assert.AreEqual("how", a[1]);
            Assert.AreEqual("amet", a[2]);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Filter_Preserves_Order_When_Inserting_Duplicate()
        {
            var l = new ObservableCollection<string>
            {
                "lorem",
                "ipsum",
                "dolor",
                "sit",
                "amet"
            };

            var a = new AdvancedCollectionView(l)
            {
                Filter = (x) => x.ToString().Length >= 5
            };

            Assert.AreEqual(3, a.Count);
            Assert.AreEqual("lorem", a[0]);
            Assert.AreEqual("ipsum", a[1]);
            Assert.AreEqual("dolor", a[2]);

            a.Insert(3, "ipsum");

            Assert.AreEqual(4, a.Count);
            Assert.AreEqual("lorem", a[0]);
            Assert.AreEqual("ipsum", a[1]);
            Assert.AreEqual("dolor", a[2]);
            Assert.AreEqual("ipsum", a[3]);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Updating_Filter_Preserves_Order_With_Duplicate()
        {
            var l = new ObservableCollection<string>
            {
                "lorem",
                "ipsum",
                "dolor",
                "sit",
                "ipsum",
                "amet"
            };

            var a = new AdvancedCollectionView(l)
            {
                Filter = (x) => x.ToString().Length < 5
            };

            Assert.AreEqual(2, a.Count);
            Assert.AreEqual("sit", a[0]);
            Assert.AreEqual("amet", a[1]);

            a.Filter = (x) => x.ToString().Length >= 5;

            Assert.AreEqual(4, a.Count);
            Assert.AreEqual("lorem", a[0]);
            Assert.AreEqual("ipsum", a[1]);
            Assert.AreEqual("dolor", a[2]);
            Assert.AreEqual("ipsum", a[3]);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Filter_Preserves_Order_When_Inserting_After_Items_In_View()
        {
            var l = new ObservableCollection<string>
            {
                "lorem",
                "ipsum",
                "dolor",
                "sitter",
                "amet"
            };

            var a = new AdvancedCollectionView(l)
            {
                Filter = (x) => x.ToString().Length < 5
            };

            Assert.AreEqual(1, a.Count);
            Assert.AreEqual(a[0], "amet");

            a.Insert(0, "how");

            Assert.AreEqual(2, a.Count);
            Assert.AreEqual(a[0], "how");
            Assert.AreEqual(a[1], "amet");
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Updating()
        {
            var l = new ObservableCollection<string>
            {
                "lorem",
                "ipsum",
                "dolor",
                "sit",
                "amet"
            };

            var a = new AdvancedCollectionView(l);

            Assert.AreEqual(5, a.Count);

            l.Add("new item");

            Assert.AreEqual(6, a.Count);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Sorting()
        {
            var l = new ObservableCollection<Person>
            {
                new Person()
                {
                    Name = "lorem",
                    Age = 4
                },
                new Person()
                {
                    Name = "imsum",
                    Age = 8
                },
                new Person()
                {
                    Name = "dolor",
                    Age = 15
                },
                new Person()
                {
                    Name = "sit",
                    Age = 16
                },
                new Person()
                {
                    Name = "amet",
                    Age = 23
                },
                new Person()
                {
                    Name = "consectetur",
                    Age = 42
                },
            };

            var a = new AdvancedCollectionView(l)
            {
                SortDescriptions =
                {
                    new SortDescription(nameof(Person.Age), SortDirection.Descending)
                }
            };

            Assert.AreEqual(42, ((Person)a.First()).Age);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Combined()
        {
            var l = new ObservableCollection<Person>
            {
                new Person()
                {
                    Name = "lorem",
                    Age = 4
                },
                new Person()
                {
                    Name = "imsum",
                    Age = 8
                },
                new Person()
                {
                    Name = "dolor",
                    Age = 15
                },
                new Person()
                {
                    Name = "sit",
                    Age = 16
                },
                new Person()
                {
                    Name = "amet",
                    Age = 23
                },
                new Person()
                {
                    Name = "consectetur",
                    Age = 42
                },
            };

            var a = new AdvancedCollectionView(l)
            {
                SortDescriptions =
                {
                    new SortDescription(nameof(Person.Age), SortDirection.Descending)
                },
                Filter = (x) => ((Person)x).Name.Length > 5
            };

            Assert.AreEqual(42, ((Person)a.First()).Age);
            Assert.AreEqual(1, a.Count);

            l.Add(new Person
            {
                Name = "foo",
                Age = 50
            });

            l.Add(new Person
            {
                Name = "Person McPersonface",
                Age = 10
            });

            Assert.AreEqual(42, ((Person)a.First()).Age);
            Assert.AreEqual(2, a.Count);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Sorting_OnSelf()
        {
            var l = new ObservableCollection<Person>
            {
                new Person()
                {
                    Name = "lorem",
                    Age = 4
                },
                new Person()
                {
                    Name = "imsum",
                    Age = 8
                },
                new Person()
                {
                    Name = "dolor",
                    Age = 15
                },
                new Person()
                {
                    Name = "sit",
                    Age = 16
                },
                new Person()
                {
                    Name = "amet",
                    Age = 23
                },
                new Person()
                {
                    Name = "consectetur",
                    Age = 42
                },
            };

            var a = new AdvancedCollectionView(l)
            {
                SortDescriptions =
                {
                    new SortDescription(SortDirection.Descending)
                }
            };

            Assert.AreEqual(42, ((Person)a.First()).Age);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Sorting_OnSelf_CustomComparable()
        {
            var l = new ObservableCollection<Person>
            {
                new Person()
                {
                    Name = "lorem",
                    Age = 4
                },
                new Person()
                {
                    Name = "imsum",
                    Age = 8
                },
                new Person()
                {
                    Name = "dolor",
                    Age = 15
                },
                new Person()
                {
                    Name = "sit",
                    Age = 16
                },
                new Person()
                {
                    Name = "amet",
                    Age = 23
                },
                new Person()
                {
                    Name = "consectetur",
                    Age = 42
                },
            };

            var a = new AdvancedCollectionView(l)
            {
                SortDescriptions =
                {
                    new SortDescription(SortDirection.Ascending, new DelegateComparable((x, y) => -((Person)x).Age.CompareTo(((Person)y).Age)))
                }
            };

            Assert.AreEqual(42, ((Person)a.First()).Age);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Sorting_CustomComparable()
        {
            var l = new ObservableCollection<Person>
            {
                new Person()
                {
                    Name = "lorem",
                    Age = 4
                },
                new Person()
                {
                    Name = "imsum",
                    Age = 8
                },
                new Person()
                {
                    Name = "dolor",
                    Age = 15
                },
                new Person()
                {
                    Name = "sit",
                    Age = 16
                },
                new Person()
                {
                    Name = "amet",
                    Age = 23
                },
                new Person()
                {
                    Name = "consectetur",
                    Age = 42
                },
            };

            var a = new AdvancedCollectionView(l)
            {
                SortDescriptions =
                {
                    new SortDescription(nameof(Person.Age), SortDirection.Ascending, new DelegateComparable((x, y) => -((int)x).CompareTo((int)y)))
                }
            };

            Assert.AreEqual(42, ((Person)a.First()).Age);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Filter_With_Shaping()
        {
            var l = new List<string>
            {
                "lorem",
                "ipsum",
                "dolor",
                "sit",
                "amet"
            };

            var a = new AdvancedCollectionView(l, true)
            {
                Filter = (x) => x.ToString().Length < 5
            };

            Assert.AreEqual(2, a.Count);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Filter_With_Shaping_And_Refresh_Call()
        {
            var l = new List<string>
            {
                "lorem",
                "ipsum",
                "dolor",
                "sit",
                "amet"
            };

            var a = new AdvancedCollectionView(l, true);

            a.Filter = (x) => x.ToString().Length < 5;

            Assert.AreEqual(2, a.Count);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Updating_With_Shaping()
        {
            var l = new ObservableCollection<string>
            {
                "lorem",
                "ipsum",
                "dolor",
                "sit",
                "amet"
            };

            var a = new AdvancedCollectionView(l, true);

            Assert.AreEqual(5, a.Count);

            l.Add("new item");

            Assert.AreEqual(6, a.Count);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Sorting_With_Shaping()
        {
            var l = new ObservableCollection<Person>
            {
                new Person()
                {
                    Name = "lorem",
                    Age = 4
                },
                new Person()
                {
                    Name = "imsum",
                    Age = 8
                },
                new Person()
                {
                    Name = "dolor",
                    Age = 15
                },
                new Person()
                {
                    Name = "sit",
                    Age = 16
                },
                new Person()
                {
                    Name = "amet",
                    Age = 23
                },
                new Person()
                {
                    Name = "consectetur",
                    Age = 42
                },
            };

            var a = new AdvancedCollectionView(l, true)
            {
                SortDescriptions =
                {
                    new SortDescription(nameof(Person.Age), SortDirection.Descending)
                }
            };

            Assert.AreEqual(42, ((Person)a.First()).Age);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Using_Shaping()
        {
            var myPerson = new Person()
            {
                Name = "lorem",
                Age = 4
            };
            var l = new ObservableCollection<Person>
            {
                myPerson,
                new Person()
                {
                    Name = "imsum",
                    Age = 8
                },
            };

            var a = new AdvancedCollectionView(l, true);

            myPerson.Name = "myName";

            Assert.AreEqual("myName", ((Person)a.First()).Name);
            Assert.AreEqual(2, a.Count);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Sorting_Using_Shaping()
        {
            var l = new ObservableCollection<Person>
            {
                new Person()
                {
                    Name = "lorem",
                    Age = 4
                },
                new Person()
                {
                    Name = "imsum",
                    Age = 8
                },
                new Person()
                {
                    Name = "dolor",
                    Age = 15
                },
                new Person()
                {
                    Name = "sit",
                    Age = 16
                },
                new Person()
                {
                    Name = "amet",
                    Age = 23
                },
                new Person()
                {
                    Name = "consectetur",
                    Age = 42
                },
            };

            var a = new AdvancedCollectionView(l, true);

            a.SortDescriptions.Add(new SortDescription(nameof(Person.Age), SortDirection.Descending));

            Assert.AreEqual(42, ((Person)a.First()).Age);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Combined_With_Shaping()
        {
            var l = new ObservableCollection<Person>
            {
                new Person()
                {
                    Name = "lorem",
                    Age = 4
                },
                new Person()
                {
                    Name = "imsum",
                    Age = 8
                },
                new Person()
                {
                    Name = "dolor",
                    Age = 15
                },
                new Person()
                {
                    Name = "sit",
                    Age = 16
                },
                new Person()
                {
                    Name = "amet",
                    Age = 23
                },
                new Person()
                {
                    Name = "consectetur",
                    Age = 42
                },
            };

            var a = new AdvancedCollectionView(l, true)
            {
                SortDescriptions =
                {
                    new SortDescription(nameof(Person.Age), SortDirection.Descending)
                },
                Filter = (x) => ((Person)x).Name.Length > 5
            };

            Assert.AreEqual(42, ((Person)a.First()).Age);
            Assert.AreEqual(1, a.Count);

            l.Add(new Person
            {
                Name = "foo",
                Age = 50
            });

            l.Add(new Person
            {
                Name = "Person McPersonface",
                Age = 10
            });

            Assert.AreEqual(42, ((Person)a.First()).Age);
            Assert.AreEqual(2, a.Count);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Combined_Using_Shaping_Changing_Properties()
        {
            var personLorem = new Person()
            {
                Name = "lorem",
                Age = 4
            };

            var l = new ObservableCollection<Person>
            {
                personLorem,
                new Person()
                {
                    Name = "imsum",
                    Age = 8
                },
                new Person()
                {
                    Name = "dolor",
                    Age = 15
                },
                new Person()
                {
                    Name = "sit",
                    Age = 16
                },
                new Person()
                {
                    Name = "amet",
                    Age = 23
                },
                new Person()
                {
                    Name = "consectetur",
                    Age = 42
                },
            };

            var a = new AdvancedCollectionView(l, true)
            {
                SortDescriptions =
                {
                    new SortDescription(nameof(Person.Age), SortDirection.Descending)
                },
                Filter = (x) => ((Person)x).Name.Length > 5
            };

            a.ObserveFilterProperty(nameof(Person.Name));

            Assert.AreEqual(42, ((Person)a.First()).Age);
            Assert.AreEqual(1, a.Count);

            personLorem.Name = "lorems";
            personLorem.Age = 96;

            Assert.AreEqual(96, ((Person)a.First()).Age);
            Assert.AreEqual(2, a.Count);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Combined_Using_Shaping()
        {
            var l = new ObservableCollection<Person>
            {
                new Person()
                {
                    Name = "lorem",
                    Age = 4
                },
                new Person()
                {
                    Name = "imsum",
                    Age = 8
                },
                new Person()
                {
                    Name = "dolor",
                    Age = 15
                },
                new Person()
                {
                    Name = "sit",
                    Age = 16
                },
                new Person()
                {
                    Name = "amet",
                    Age = 23
                },
                new Person()
                {
                    Name = "consectetur",
                    Age = 42
                },
            };

            var a = new AdvancedCollectionView(l, true);

            a.Filter = (x) => ((Person)x).Name.Length > 5;
            a.RefreshFilter();

            a.SortDescriptions.Add(new SortDescription(nameof(Person.Age), SortDirection.Descending));

            Assert.AreEqual(42, ((Person)a.First()).Age);
            Assert.AreEqual(1, a.Count);

            l.Add(new Person
            {
                Name = "foo",
                Age = 50
            });

            l.Add(new Person
            {
                Name = "Person McPersonface",
                Age = 10
            });

            Assert.AreEqual(42, ((Person)a.First()).Age);
            Assert.AreEqual(2, a.Count);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Combined_Using_Shaping_Filter_Back_In()
        {
            var l = new ObservableCollection<Person>
            {
                new Person()
                {
                    Name = "lorem",
                    Age = 4
                },
                new Person()
                {
                    Name = "imsum",
                    Age = 8
                },
                new Person()
                {
                    Name = "dolor",
                    Age = 15
                },
                new Person()
                {
                    Name = "sit",
                    Age = 16
                },
                new Person()
                {
                    Name = "amet",
                    Age = 23
                },
                new Person()
                {
                    Name = "consectetur",
                    Age = 42
                },
            };

            var a = new AdvancedCollectionView(l, true);

            a.Filter = (x) => ((Person)x).Name.Length > 5;
            a.RefreshFilter();

            a.Filter = (x) => ((Person)x).Name.Length > 4;
            a.RefreshFilter();

            a.SortDescriptions.Add(new SortDescription(nameof(Person.Age), SortDirection.Descending));

            Assert.AreEqual(42, ((Person)a.First()).Age);
            Assert.AreEqual(4, a.Count);

            l.Add(new Person
            {
                Name = "foo",
                Age = 50
            });

            l.Add(new Person
            {
                Name = "Person McPersonface",
                Age = 10
            });

            Assert.AreEqual(42, ((Person)a.First()).Age);
            Assert.AreEqual(5, a.Count);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Sorting_OnSelf_With_Shaping()
        {
            var l = new ObservableCollection<Person>
            {
                new Person()
                {
                    Name = "lorem",
                    Age = 4
                },
                new Person()
                {
                    Name = "imsum",
                    Age = 8
                },
                new Person()
                {
                    Name = "dolor",
                    Age = 15
                },
                new Person()
                {
                    Name = "sit",
                    Age = 16
                },
                new Person()
                {
                    Name = "amet",
                    Age = 23
                },
                new Person()
                {
                    Name = "consectetur",
                    Age = 42
                },
            };

            var a = new AdvancedCollectionView(l, true)
            {
                SortDescriptions =
                {
                    new SortDescription(SortDirection.Descending)
                }
            };

            Assert.AreEqual(42, ((Person)a.First()).Age);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Sorting_OnSelf_CustomComparable_With_Shaping()
        {
            var l = new ObservableCollection<Person>
            {
                new Person()
                {
                    Name = "lorem",
                    Age = 4
                },
                new Person()
                {
                    Name = "imsum",
                    Age = 8
                },
                new Person()
                {
                    Name = "dolor",
                    Age = 15
                },
                new Person()
                {
                    Name = "sit",
                    Age = 16
                },
                new Person()
                {
                    Name = "amet",
                    Age = 23
                },
                new Person()
                {
                    Name = "consectetur",
                    Age = 42
                },
            };

            var a = new AdvancedCollectionView(l, true)
            {
                SortDescriptions =
                {
                    new SortDescription(SortDirection.Ascending, new DelegateComparable((x, y) => -((Person)x).Age.CompareTo(((Person)y).Age)))
                }
            };

            Assert.AreEqual(42, ((Person)a.First()).Age);
        }

        [TestCategory("AdvancedCollectionView")]
        [UITestMethod]
        public void Test_AdvancedCollectionView_Sorting_CustomComparable_With_Shaping()
        {
            var l = new ObservableCollection<Person>
            {
                new Person()
                {
                    Name = "lorem",
                    Age = 4
                },
                new Person()
                {
                    Name = "imsum",
                    Age = 8
                },
                new Person()
                {
                    Name = "dolor",
                    Age = 15
                },
                new Person()
                {
                    Name = "sit",
                    Age = 16
                },
                new Person()
                {
                    Name = "amet",
                    Age = 23
                },
                new Person()
                {
                    Name = "consectetur",
                    Age = 42
                },
            };

            var a = new AdvancedCollectionView(l, true)
            {
                SortDescriptions =
                {
                    new SortDescription(nameof(Person.Age), SortDirection.Ascending, new DelegateComparable((x, y) => -((int)x).CompareTo((int)y)))
                }
            };

            Assert.AreEqual(42, ((Person)a.First()).Age);
        }

        private class DelegateComparable : IComparer
        {
            private Func<object, object, int> _func;

            public DelegateComparable(Func<object, object, int> func)
            {
                _func = func;
            }

            public int Compare(object x, object y) => _func(x, y);
        }
    }
}