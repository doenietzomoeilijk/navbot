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
            one = new SolarSystem(1, "One");
            two = new SolarSystem(2, "Two");
            three = new SolarSystem(3, "Three");
            zero = new SolarSystem(0, "Zero");
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
            Assert.IsTrue(one.Signpost.ContainsKey(two));
            Assert.AreSame(one.Signpost[two].Direction, two);
            Assert.AreEqual(1, one.Signpost[two].Distance);

            one.UpdateSignpostTo(three);
            Assert.IsTrue(one.Signpost.ContainsKey(three));
            Assert.AreSame(one.Signpost[three].Direction, two);
            Assert.AreEqual(2, one.Signpost[three].Distance);

            three.UpdateSignpostTo(one);
            Assert.IsTrue(three.Signpost.ContainsKey(one));
            Assert.AreSame(three.Signpost[one].Direction, one);
            Assert.AreEqual(1, three.Signpost[one].Distance);

            // Check that this hasn't affected one's route to three
            Assert.AreSame(one.Signpost[three].Direction, two);
            Assert.AreEqual(2, one.Signpost[three].Distance);
        }
    }
}
