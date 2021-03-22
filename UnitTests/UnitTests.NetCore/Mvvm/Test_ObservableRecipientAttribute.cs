// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Mvvm
{
    [TestClass]
    public partial class Test_ObservableRecipientAttribute
    {
        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_ObservableRecipientAttribute_Events()
        {
            var model = new Person();
            var args = new List<PropertyChangedEventArgs>();

            model.PropertyChanged += (s, e) => args.Add(e);

            Assert.IsFalse(model.HasErrors);

            model.Name = "No";

            Assert.IsTrue(model.HasErrors);
            Assert.AreEqual(args.Count, 2);
            Assert.AreEqual(args[0].PropertyName, nameof(Person.Name));
            Assert.AreEqual(args[1].PropertyName, nameof(INotifyDataErrorInfo.HasErrors));

            model.Name = "Valid";

            Assert.IsFalse(model.HasErrors);
            Assert.AreEqual(args.Count, 4);
            Assert.AreEqual(args[2].PropertyName, nameof(Person.Name));
            Assert.AreEqual(args[3].PropertyName, nameof(INotifyDataErrorInfo.HasErrors));

            Assert.IsNotNull(typeof(Person).GetProperty("Messenger", BindingFlags.Instance | BindingFlags.NonPublic));
            Assert.AreEqual(typeof(Person).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Length, 0);
        }

        [ObservableRecipient]
        public partial class Person : ObservableValidator
        {
            public Person()
            {
                Messenger = WeakReferenceMessenger.Default;
            }

            private string name;

            [MinLength(4)]
            [MaxLength(20)]
            [Required]
            public string Name
            {
                get => this.name;
                set => SetProperty(ref this.name, value, true);
            }

            public void TestCompile()
            {
                // Validates that the method Broadcast is correctly being generated
                Broadcast(0, 1, nameof(TestCompile));
            }
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_ObservableRecipientAttribute_AbstractConstructors()
        {
            var ctors = typeof(AbstractPerson).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.AreEqual(ctors.Length, 2);
            Assert.IsTrue(ctors.All(static ctor => ctor.IsFamily));
        }

        [ObservableRecipient]
        public abstract partial class AbstractPerson : ObservableObject
        {
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_ObservableRecipientAttribute_NonAbstractConstructors()
        {
            var ctors = typeof(NonAbstractPerson).GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            Assert.AreEqual(ctors.Length, 2);
            Assert.IsTrue(ctors.All(static ctor => ctor.IsPublic));
        }

        [ObservableRecipient]
        public partial class NonAbstractPerson : ObservableObject
        {
        }
    }
}
