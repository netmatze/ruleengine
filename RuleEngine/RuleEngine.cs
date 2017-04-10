using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RuleEngine
{
    public class RuleEngine
    {
        public Func<T, bool> CompileRule<T>(Operator ruleOperator, object value)
        {
            ExpressionBuilder expressionBuilder = new ExpressionBuilder();
            var param = Expression.Parameter(typeof(T));
            Expression expression = expressionBuilder.BuildExpression<T>(ruleOperator, value, param);
            Func<T, bool> func = Expression.Lambda<Func<T, bool>>(expression, param).Compile();
            return func;
        }

        public Func<T, bool> CompileRule<T>(string propertyName, Operator ruleOperator, object value)
        {
            ExpressionBuilder expressionBuilder = new ExpressionBuilder();
            var param = Expression.Parameter(typeof(T));
            Expression expression = expressionBuilder.BuildExpression<T>(propertyName, ruleOperator, value, param);
            Func<T, bool> func = Expression.Lambda<Func<T, bool>>(expression, param).Compile();
            return func;
        }

        public Func<T, bool> CompileRule<T>(string propertyName, 
            Operator ruleOperator, List<T> values)
        {
            ExpressionBuilder expressionBuilder = new ExpressionBuilder();
            var param = Expression.Parameter(typeof(T));
            Tuple<Expression, ParameterExpression> expression =
                expressionBuilder.BuildExpression<T>(propertyName, ruleOperator, param, values);
            Func<T, bool> compiledExpression = Expression.Lambda<Func<T, bool>>(
                expression.Item1, expression.Item2).Compile();
            return compiledExpression;
        }

        public Func<T, bool> CompileRule<T>(Rule<T> rule)
        {            
            ExpressionBuilder expressionBuilder = new ExpressionBuilder();
            var param = Expression.Parameter(typeof(T));           
            Tuple<Expression, ParameterExpression> expression =
                expressionBuilder.BuildExpression<T>(rule.PropertyName, rule.Operator_, param, rule.Values);                
            Func<T, bool> compiledExpression = Expression.Lambda<Func<T, bool>>(
                expression.Item1, expression.Item2).Compile();
            return compiledExpression;            
        }

        public Func<T, bool> CompileRule<T>(Rule rule)
        {
            if (string.IsNullOrEmpty(rule.PropertyName))
            {
                ExpressionBuilder expressionBuilder = new ExpressionBuilder();
                var param = Expression.Parameter(typeof(T));
                Expression expression = expressionBuilder.BuildExpression<T>(rule.Operator_, rule.Value, param);
                Func<T, bool> func = Expression.Lambda<Func<T, bool>>(expression, param).Compile();
                return func;
            }            
            else
            {
                ExpressionBuilder expressionBuilder = new ExpressionBuilder();
                var param = Expression.Parameter(typeof(T));
                Expression expression = expressionBuilder.BuildExpression<T>(rule.PropertyName, rule.Operator_, rule.Value, param);
                Func<T, bool> func = Expression.Lambda<Func<T, bool>>(expression, param).Compile();
                return func;
            }
        }

        public bool ValidateRules<T>(T value, Func<T, bool>[] rules)
        {
            foreach (var rule in rules)
            {
                if (!rule(value))
                    return false;
            }
            return true;
        }

        public bool ValidateRules<T>(T[] values, Func<T, bool>[] rules)
        {
            foreach (var value in values)
            {
                foreach (var rule in rules)
                {
                    if (!rule(value))
                        return false;
                }
            }
            return true;
        }

        public Func<T, bool>[] CombineRules<T>(Rule[] rules)
        {
            List<Func<T, bool>> list = new List<Func<T, bool>>();
            foreach (Rule rule in rules)
            {
                if (string.IsNullOrEmpty(rule.PropertyName))
                {
                    ExpressionBuilder expressionBuilder = new ExpressionBuilder();
                    var param = Expression.Parameter(typeof(T));
                    Expression expression = expressionBuilder.BuildExpression<T>(rule.Operator_, rule.Value, param);
                    Func<T, bool> func = Expression.Lambda<Func<T, bool>>(expression, param).Compile();
                    list.Add(func);
                }
                else
                {
                    ExpressionBuilder expressionBuilder = new ExpressionBuilder();
                    var param = Expression.Parameter(typeof(T));
                    Expression expression = expressionBuilder.BuildExpression<T>(rule.PropertyName, rule.Operator_, rule.Value, param);
                    Func<T, bool> func = Expression.Lambda<Func<T, bool>>(expression, param).Compile();
                    list.Add(func);
                }
            }
            return list.ToArray();
        }

        public Func<dynamic, bool> CompileRule(Operator ruleOperator, object value)
        {
            ExpressionBuilder expressionBuilder = new ExpressionBuilder();
            var param = Expression.Parameter(typeof(object));
            Expression expression = expressionBuilder.BuildExpression<dynamic>(ruleOperator, value, param);
            Func<dynamic, bool> func = Expression.Lambda<Func<dynamic, bool>>(expression, param).Compile();
            return func;
        }

        public Func<dynamic, bool> CompileRule(Rule rule)
        {
            ExpressionBuilder expressionBuilder = new ExpressionBuilder();
            var param = Expression.Parameter(typeof(object));
            Expression expression = expressionBuilder.BuildExpression<dynamic>(rule.Operator_, rule.Value, param);
            Func<dynamic, bool> func = Expression.Lambda<Func<dynamic, bool>>(expression, param).Compile();
            return func;
        }
    }
}
