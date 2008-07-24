using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace EveMarketTool.Tests
{
    [TestFixture]
    public class SolarSystemTest
    {
        SolarSystem one;
        SolarSystem two;
        SolarSystem three;
        SolarSystem zero;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
        }

        [SetUp]
        public void TestCaseSetUp()
        {
            one = new SolarSystem(1, "One", 1.0f);
            two = new SolarSystem(2, "Two", 0.9f);
            three = new SolarSystem(3, "Three", 0.8f);
            zero = new SolarSystem(0, "Zero", 0.0f);
            one.AddGateTo(two);
            two.AddGateTo(one);
            two.AddGateTo(three);
            three.AddGateTo(two);
            three.AddGateTo(one); // you can go from three to one, but not one to three
            // there are no gates to or from zero
        }

        [Test]
        public void TestUpdateSignpostTo()
        {
            one.UpdateSignpostTo(two);
            Assert.IsTrue(one.SignpostSecure.ContainsKey(two));
            Assert.AreSame(one.SignpostSecure[two].Direction, two);
            Assert.AreEqual(1, one.SignpostSecure[two].Distance);

            one.UpdateSignpostTo(three);
            Assert.IsTrue(one.SignpostSecure.ContainsKey(three));
            Assert.AreSame(one.SignpostSecure[three].Direction, two);
            Assert.AreEqual(2, one.SignpostSecure[three].Distance);

            three.UpdateSignpostTo(one);
            Assert.IsTrue(three.SignpostSecure.ContainsKey(one));
            Assert.AreSame(three.SignpostSecure[one].Direction, one);
            Assert.AreEqual(1, three.SignpostSecure[one].Distance);

            // Check that this hasn't affected one's route to three
            Assert.AreSame(one.SignpostSecure[three].Direction, two);
            Assert.AreEqual(2, one.SignpostSecure[three].Distance);
        }
    }
}
