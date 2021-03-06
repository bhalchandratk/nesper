///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2017 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////


using System;

using com.espertech.esper.client;
using com.espertech.esper.events.bean;
using com.espertech.esper.support.bean;
using com.espertech.esper.support.events;

using NUnit.Framework;

namespace com.espertech.esper.events.property
{
    [TestFixture]
    public class TestIndexedProperty 
    {
        private IndexedProperty[] _indexed;
        private EventBean _theEvent;
        private BeanEventType _eventType;
    
        [SetUp]
        public void SetUp()
        {
            _indexed = new IndexedProperty[4];
            _indexed[0] = new IndexedProperty("Indexed", 0);
            _indexed[1] = new IndexedProperty("Indexed", 1);
            _indexed[2] = new IndexedProperty("ArrayProperty", 0);
            _indexed[3] = new IndexedProperty("ArrayProperty", 1);
    
            _theEvent = SupportEventBeanFactory.CreateObject(SupportBeanComplexProps.MakeDefaultBean());
            _eventType = (BeanEventType)_theEvent.EventType;
        }
    
        [Test]
        public void TestGetGetter()
        {
            int[] expected = new int[] {1, 2, 10, 20};
            for (int i = 0; i < _indexed.Length; i++)
            {
                EventPropertyGetter getter = _indexed[i].GetGetter(_eventType, SupportEventAdapterService.Service);
                Assert.AreEqual(expected[i], getter.Get(_theEvent));
            }
    
            // try invalid case
            IndexedProperty ind = new IndexedProperty("Dummy", 0);
            Assert.IsNull(ind.GetGetter(_eventType, SupportEventAdapterService.Service));
        }
    
        [Test]
        public void TestGetPropertyType()
        {
            Type[] expected = { typeof(int), typeof(int), typeof(int), typeof(int) };
            for (int i = 0; i < _indexed.Length; i++)
            {
                Assert.AreEqual(expected[i], _indexed[i].GetPropertyType(_eventType, SupportEventAdapterService.Service));
            }
    
            // try invalid case
            IndexedProperty ind = new IndexedProperty("Dummy", 0);
            Assert.IsNull(ind.GetPropertyType(_eventType, SupportEventAdapterService.Service));
        }
    }
}
