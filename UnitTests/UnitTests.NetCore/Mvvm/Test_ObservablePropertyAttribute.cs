// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Mvvm
{
    [TestClass]
    public partial class Test_ObservablePropertyAttribute
    {
        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_ObservablePropertyAttribute_Events()
        {
            var model = new SampleModel();

            (PropertyChangingEventArgs, int) changing = default;
            (PropertyChangedEventArgs, int) changed = default;

            model.PropertyChanging += (s, e) =>
            {
                Assert.IsNull(changing.Item1);
                Assert.IsNull(changed.Item1);
                Assert.AreSame(model, s);
                Assert.IsNotNull(s);
                Assert.IsNotNull(e);

                changing = (e, model.Data);
            };

            model.PropertyChanged += (s, e) =>
            {
                Assert.IsNotNull(changing.Item1);
                Assert.IsNull(changed.Item1);
                Assert.AreSame(model, s);
                Assert.IsNotNull(s);
                Assert.IsNotNull(e);

                changed = (e, model.Data);
            };

            model.Data = 42;

            Assert.AreEqual(changing.Item1?.PropertyName, nameof(SampleModel.Data));
            Assert.AreEqual(changing.Item2, 0);
            Assert.AreEqual(changed.Item1?.PropertyName, nameof(SampleModel.Data));
            Assert.AreEqual(changed.Item2, 42);
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_AlsoNotifyForAttribute_Events()
        {
            var model = new DependentPropertyModel();

            (PropertyChangedEventArgs, int) changed = default;
            List<string> propertyNames = new();

            model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);

            model.Name = "Bob";
            model.Surname = "Ross";

            CollectionAssert.AreEqual(new[] { nameof(model.Name), nameof(model.FullName), nameof(model.Surname), nameof(model.FullName) }, propertyNames);
        }

        public partial class SampleModel : ObservableObject
        {
            /// <summary>
            /// This is a sample data field within <see cref="SampleModel"/> of type <see cref="int"/>.
            /// </summary>
            [ObservableProperty]
            private int data;
        }

        [INotifyPropertyChanged]
        public sealed partial class DependentPropertyModel
        {
            [ObservableProperty]
            [AlsoNotifyFor(nameof(FullName))]
            private string name;

            [ObservableProperty]
            [AlsoNotifyFor(nameof(FullName))]
            private string surname;

            public string FullName => $"{Name} {Surname}";
        }
    }
}
