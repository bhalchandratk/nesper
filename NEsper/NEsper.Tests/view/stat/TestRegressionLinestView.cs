///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2017 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.client;
using com.espertech.esper.compat;
using com.espertech.esper.compat.collections;
using com.espertech.esper.core.support;
using com.espertech.esper.support.bean;
using com.espertech.esper.support.epl;
using com.espertech.esper.support.events;
using com.espertech.esper.support.util;
using com.espertech.esper.support.view;

using NUnit.Framework;

namespace com.espertech.esper.view.stat
{
    [TestFixture]
    public class TestRegressionLinestView 
    {
        RegressionLinestView _myView;
        SupportBeanClassView _childView;
    
        [SetUp]
        public void SetUp()
        {
            // Set up sum view and a test child view
            EventType type = RegressionLinestView.CreateEventType(SupportStatementContextFactory.MakeContext(), null, 1);

            RegressionLinestViewFactory viewFactory = new RegressionLinestViewFactory();
            _myView = new RegressionLinestView(viewFactory, SupportStatementContextFactory.MakeAgentInstanceContext(), SupportExprNodeFactory.MakeIdentNodeMD("Price"), SupportExprNodeFactory.MakeIdentNodeMD("Volume"), type, null);
    
            _childView = new SupportBeanClassView(typeof(SupportMarketDataBean));
            _myView.AddView(_childView);
        }
    
        // Check values against Microsoft Excel computed values
        [Test]
        public void TestViewComputedValues()
        {
            // Set up feed for sum view
            SupportStreamImpl stream = new SupportStreamImpl(typeof(SupportMarketDataBean), 3);
            stream.AddView(_myView);
    
            // Send a first event, checkNew values
            EventBean marketData = MakeBean("IBM", 70, 1000);
            stream.Insert(marketData);
            CheckOld(Double.NaN, Double.NaN);
            CheckNew(Double.NaN, Double.NaN);
    
            // Send a second event, checkNew values
            marketData = MakeBean("IBM", 70.5, 1500);
            stream.Insert(marketData);
            CheckOld(Double.NaN, Double.NaN);
            CheckNew(1000, -69000);
    
            // Send a third event, checkNew values
            marketData = MakeBean("IBM", 70.1, 1200);
            stream.Insert(marketData);
            CheckOld(1000, -69000);
            CheckNew(928.5714286, -63952.380953);
    
            // Send a 4th event, this time the first event should be gone, checkNew values
            marketData = MakeBean("IBM", 70.25, 1000);
            stream.Insert(marketData);
            CheckOld(928.5714286, -63952.380953);
            CheckNew(877.5510204, -60443.877555);
        }
    
        [Test]
        public void TestGetSchema()
        {
            Assert.IsTrue(_myView.EventType.GetPropertyType(ViewFieldEnum.REGRESSION__SLOPE.GetName()) == typeof(double?));
            Assert.IsTrue(_myView.EventType.GetPropertyType(ViewFieldEnum.REGRESSION__YINTERCEPT.GetName()) == typeof(double?));
        }
    
        [Test]
        public void TestCopyView()
        {
            RegressionLinestView copied = (RegressionLinestView) _myView.CloneView();
            Assert.IsTrue(_myView.ExpressionX.Equals(copied.ExpressionX));
            Assert.IsTrue(_myView.ExpressionY.Equals(copied.ExpressionY));
        }
    
        private void CheckNew(double slopeE, double yinterceptE)
        {
            IEnumerator<EventBean> iterator = _myView.GetEnumerator();
            CheckValues(iterator.Advance(), slopeE, yinterceptE);
            Assert.IsTrue(iterator.MoveNext() == false);
    
            Assert.IsTrue(_childView.LastNewData.Length == 1);
            EventBean childViewValues = _childView.LastNewData[0];
            CheckValues(childViewValues, slopeE, yinterceptE);
        }
    
        private void CheckOld(double slopeE, double yinterceptE)
        {
            Assert.IsTrue(_childView.LastOldData.Length == 1);
            EventBean childViewValues = _childView.LastOldData[0];
            CheckValues(childViewValues, slopeE, yinterceptE);
        }
    
        private void CheckValues(EventBean eventBean, double slopeE, double yinterceptE)
        {
            double slope = GetDoubleValue(ViewFieldEnum.REGRESSION__SLOPE, eventBean);
            double yintercept = GetDoubleValue(ViewFieldEnum.REGRESSION__YINTERCEPT, eventBean);
            Assert.IsTrue(DoubleValueAssertionUtil.Equals(slope,  slopeE, 6));
            Assert.IsTrue(DoubleValueAssertionUtil.Equals(yintercept,  yinterceptE, 6));
        }
    
        private double GetDoubleValue(ViewFieldEnum field, EventBean theEvent)
        {
            return theEvent.Get(field.GetName()).AsDouble();
        }
    
        private EventBean MakeBean(String symbol, double price, long volume)
        {
            SupportMarketDataBean bean = new SupportMarketDataBean(symbol, price, volume, "");
            return SupportEventBeanFactory.CreateObject(bean);
        }
    }
}
