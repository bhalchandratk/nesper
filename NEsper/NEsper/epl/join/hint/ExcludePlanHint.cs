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
using com.espertech.esper.client.annotation;
using com.espertech.esper.compat;
using com.espertech.esper.compat.logging;
using com.espertech.esper.core.service;
using com.espertech.esper.epl.expression.core;
using com.espertech.esper.epl.expression;
using com.espertech.esper.events;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.join.hint
{
    public class ExcludePlanHint
    {
        private static readonly ILog QueryPlanLog = LogManager.GetLogger(AuditPath.QUERYPLAN_LOG);
    
        private readonly String[] _streamNames;
        private readonly IList<ExprEvaluator> _evaluators;
        private readonly ExprEvaluatorContext _exprEvaluatorContext;
        private readonly bool _queryPlanLogging;
    
        public ExcludePlanHint(String[] streamNames, IList<ExprEvaluator> evaluators, StatementContext statementContext)
        {
            _streamNames = streamNames;
            _evaluators = evaluators;
            _exprEvaluatorContext = new ExprEvaluatorContextStatement(statementContext, false);
            _queryPlanLogging = statementContext.ConfigSnapshot.EngineDefaults.LoggingConfig.IsEnableQueryPlan;
        }
    
        public static ExcludePlanHint GetHint(String[] streamNames, StatementContext statementContext)
        {
            IList<String> hints = HintEnum.EXCLUDE_PLAN.GetHintAssignedValues(statementContext.Annotations);
            if (hints == null) {
                return null;
            }
            IList<ExprEvaluator> filters = new List<ExprEvaluator>();
            foreach (String hint in hints) {
                if (string.IsNullOrWhiteSpace(hint)) {
                    continue;
                }
                ExprEvaluator evaluator = ExcludePlanHintExprUtil.ToExpression(hint, statementContext);
                if (TypeHelper.GetBoxedType(evaluator.ReturnType) != typeof(bool?)) {
                    throw new ExprValidationException("Expression provided for hint " + HintEnum.EXCLUDE_PLAN.GetValue() + " must return a boolean value");
                }
                filters.Add(evaluator);
            }
            return new ExcludePlanHint(streamNames, filters, statementContext);
        }
    
        public bool Filter(int streamLookup, int streamIndexed, ExcludePlanFilterOperatorType opType, params ExprNode[] exprNodes)
        {
            EventBean @event = ExcludePlanHintExprUtil.ToEvent(streamLookup,
                    streamIndexed, _streamNames[streamLookup], _streamNames[streamIndexed],
                    opType.GetName().ToLower(), exprNodes);
            if (_queryPlanLogging && QueryPlanLog.IsInfoEnabled) {
                QueryPlanLog.Info("Exclude-plan-hint combination " + EventBeanUtility.PrintEvent(@event));
            }
            EventBean[] eventsPerStream = new EventBean[] {@event};

            var evaluateParams = new EvaluateParams(eventsPerStream, true, _exprEvaluatorContext);
            foreach (ExprEvaluator evaluator in _evaluators)
            {
                var pass = evaluator.Evaluate(evaluateParams);
                if (pass != null && true.Equals(pass)) {
                    if (_queryPlanLogging && QueryPlanLog.IsInfoEnabled) {
                        QueryPlanLog.Info("Exclude-plan-hint combination : true");
                    }
                    return true;
                }
            }
            if (_queryPlanLogging && QueryPlanLog.IsInfoEnabled) {
                QueryPlanLog.Info("Exclude-plan-hint combination : false");
            }
            return false;
        }
    }
}
