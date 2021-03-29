// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Mvvm
{
    [TestClass]
    public partial class Test_ObservableObjectAttribute
    {
        [TestCategory("Mvvm")]
        [TestMethod]
        public void Test_ObservableObjectAttribute_Events()
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
        public void Test_ObservableObjectAttribute_OnSealedClass_Events()
        {
            var model = new SampleModelSealed();

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

            Assert.AreEqual(changing.Item1?.PropertyName, nameof(SampleModelSealed.Data));
            Assert.AreEqual(changing.Item2, 0);
            Assert.AreEqual(changed.Item1?.PropertyName, nameof(SampleModelSealed.Data));
            Assert.AreEqual(changed.Item2, 42);
        }

        [ObservableObject]
        public partial class SampleModel
        {
            private int data;

            public int Data
            {
                get => data;
                set => SetProperty(ref data, value);
            }
        }

        [ObservableObject]
        public sealed partial class SampleModelSealed
        {
            private int data;

            public int Data
            {
                get => data;
                set => SetProperty(ref data, value);
            }
        }
    }
}
