// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Diagnostics
{
    public partial class Test_Guard
    {
        [TestCategory("Guard")]
        [TestMethod]
        [DataRow(0, 20, 10u, false)]
        [DataRow(0, 6, 5u, false)]
        [DataRow(0, int.MaxValue, 500u, false)]
        [DataRow(-500, -530, 10u, false)]
        [DataRow(1000, 800, 100u, false)]
        [DataRow(int.MaxValue, int.MaxValue - 10, 7u, false)]
        [DataRow(int.MinValue, int.MaxValue, (uint)int.MaxValue, false)]
        [DataRow(0, 5, 10u, true)]
        [DataRow(0, 5, 5u, true)]
        [DataRow(0, int.MaxValue, (uint)int.MaxValue, true)]
        [DataRow(-500, -530, 50u, true)]
        [DataRow(1000, 800, 200u, true)]
        [DataRow(int.MaxValue, int.MaxValue - 10, 10u, true)]
        public void Test_Guard_IsCloseOrNotToInt(int value, int target, uint delta, bool isClose)
        {
            void Test(int value, int target)
            {
                bool isFailed = false;

                try
                {
                    Guard.IsCloseTo(value, target, delta, nameof(Test_Guard_IsCloseOrNotToInt));
                }
                catch (ArgumentException)
                {
                    isFailed = true;
                }

                Assert.AreEqual(isClose, !isFailed);

                isFailed = false;

                try
                {
                    Guard.IsNotCloseTo(value, target, delta, nameof(Test_Guard_IsCloseOrNotToInt));
                }
                catch (ArgumentException)
                {
                    isFailed = true;
                }

                Assert.AreEqual(isClose, isFailed);
            }

            Test(value, target);
            Test(target, value);
        }

        [TestCategory("Guard")]
        [TestMethod]
        [DataRow(0f, 20f, 10f, false)]
        [DataRow(0f, 6f, 5f, false)]
        [DataRow(0f, float.MaxValue, 500f, false)]
        [DataRow(-500f, -530f, 10f, false)]
        [DataRow(1000f, 800f, 100f, false)]
        [DataRow(float.MaxValue, float.MaxValue / 2, 7f, false)]
        [DataRow(float.MinValue, float.MaxValue, float.MaxValue, false)]
        [DataRow(0f, 5f, 10f, true)]
        [DataRow(0f, 5f, 5f, true)]
        [DataRow(0f, float.MaxValue, float.MaxValue, true)]
        [DataRow(-500f, -530f, 50f, true)]
        [DataRow(1000f, 800f, 200f, true)]
        [DataRow(float.MaxValue, float.MaxValue - 10, 10f, true)]
        public void Test_Guard_IsCloseToFloat(float value, float target, float delta, bool isClose)
        {
            void Test(float value, float target)
            {
                bool isFailed = false;

                try
                {
                    Guard.IsCloseTo(value, target, delta, nameof(Test_Guard_IsCloseToFloat));
                }
                catch (ArgumentException)
                {
                    isFailed = true;
                }

                Assert.AreEqual(isClose, !isFailed);

                isFailed = false;

                try
                {
                    Guard.IsNotCloseTo(value, target, delta, nameof(Test_Guard_IsCloseToFloat));
                }
                catch (ArgumentException)
                {
                    isFailed = true;
                }

                Assert.AreEqual(isClose, isFailed);
            }

            Test(value, target);
            Test(target, value);
        }
    }
}