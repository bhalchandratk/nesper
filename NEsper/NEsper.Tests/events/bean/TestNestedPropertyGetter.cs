///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2017 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using XLR8.CGLib;

using com.espertech.esper.client;
using com.espertech.esper.support.bean;
using com.espertech.esper.support.events;

using NUnit.Framework;

namespace com.espertech.esper.events.bean
{
    [TestFixture]
    public class TestNestedPropertyGetter 
    {
        private NestedPropertyGetter _getter;
        private NestedPropertyGetter _getterNull;
        private EventBean _theEvent;
        private SupportBeanCombinedProps _bean;
        private BeanEventTypeFactory _beanEventTypeFactory;
    
        [SetUp]
        public void SetUp()
        {
            _beanEventTypeFactory = new BeanEventAdapter(new ConcurrentDictionary<Type, BeanEventType>(), SupportEventAdapterService.Service, new EventTypeIdGeneratorImpl());
            _bean = SupportBeanCombinedProps.MakeDefaultBean();
            _theEvent = SupportEventBeanFactory.CreateObject(_bean);
    
            List<EventPropertyGetter> getters = new List<EventPropertyGetter>();
            getters.Add(MakeGetterOne(0));
            getters.Add(MakeGetterTwo("0ma"));
            _getter = new NestedPropertyGetter(getters, SupportEventAdapterService.Service, typeof(IDictionary<string,object>), null);
    
            getters = new List<EventPropertyGetter>();
            getters.Add(MakeGetterOne(2));
            getters.Add(MakeGetterTwo("0ma"));
            _getterNull = new NestedPropertyGetter(getters, SupportEventAdapterService.Service, typeof(IDictionary<string, object>), null);
        }
    
        [Test]
        public void TestGet()
        {
            Assert.AreEqual(_bean.GetIndexed(0).GetMapped("0ma"), _getter.Get(_theEvent));
    
            // test null value returned
            Assert.IsNull(_getterNull.Get(_theEvent));
    
            try
            {
                _getter.Get(SupportEventBeanFactory.CreateObject(""));
                Assert.Fail();
            }
            catch (PropertyAccessException ex)
            {
                // expected
            }
        }
    
        private KeyedFastPropertyGetter MakeGetterOne(int index)
        {
            FastClass fastClassOne = FastClass.Create(typeof(SupportBeanCombinedProps));
            FastMethod methodOne = fastClassOne.GetMethod("GetIndexed", new[] {typeof(int)});
            return new KeyedFastPropertyGetter(methodOne, index, SupportEventAdapterService.Service);
        }
    
        private KeyedFastPropertyGetter MakeGetterTwo(String key)
        {
            FastClass fastClassTwo = FastClass.Create(typeof(SupportBeanCombinedProps.NestedLevOne));
            FastMethod methodTwo = fastClassTwo.GetMethod("GetMapped", new[] {typeof(string)});
            return new KeyedFastPropertyGetter(methodTwo, key, SupportEventAdapterService.Service);
        }
    }
}
