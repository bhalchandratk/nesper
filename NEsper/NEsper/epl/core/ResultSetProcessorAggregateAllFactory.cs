///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.client;
using com.espertech.esper.core.context.util;
using com.espertech.esper.epl.agg.service;
using com.espertech.esper.epl.expression.core;
using com.espertech.esper.epl.expression;

namespace com.espertech.esper.epl.core
{
    /// <summary>
    /// Result set processor prototype for the case: aggregation functions used in the select 
    /// clause, and no group-by, and not all of the properties in the select clause are under 
    /// an aggregation function.
    /// </summary>
    public class ResultSetProcessorAggregateAllFactory : ResultSetProcessorFactory
    {
        private readonly SelectExprProcessor _selectExprProcessor;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="selectExprProcessor">for processing the select expression and generting the readonly output rows</param>
        /// <param name="optionalHavingNode">having clause expression node</param>
        /// <param name="isSelectRStream">true if remove stream events should be generated</param>
        /// <param name="isUnidirectional">true if unidirectional join</param>
        /// <param name="isHistoricalOnly">if set to <c>true</c> [is historical only].</param>
        public ResultSetProcessorAggregateAllFactory(
            SelectExprProcessor selectExprProcessor,
            ExprEvaluator optionalHavingNode,
            bool isSelectRStream,
            bool isUnidirectional,
            bool isHistoricalOnly)
        {
            _selectExprProcessor = selectExprProcessor;
            OptionalHavingNode = optionalHavingNode;
            IsSelectRStream = isSelectRStream;
            IsUnidirectional = isUnidirectional;
            IsHistoricalOnly = isHistoricalOnly;
        }

        public ExprEvaluator OptionalHavingNode { get; private set; }

        public bool IsSelectRStream { get; private set; }

        public bool IsUnidirectional { get; private set; }

        public bool IsHistoricalOnly { get; private set; }

        #region ResultSetProcessorFactory Members

        public ResultSetProcessor Instantiate(OrderByProcessor orderByProcessor,
                                              AggregationService aggregationService,
                                              AgentInstanceContext agentInstanceContext)
        {
            return new ResultSetProcessorAggregateAll(this, _selectExprProcessor, orderByProcessor, aggregationService,
                                                      agentInstanceContext);
        }

        public EventType ResultEventType
        {
            get { return _selectExprProcessor.ResultEventType; }
        }

        public bool HasAggregation
        {
            get { return true; }
        }

        #endregion
    }
}