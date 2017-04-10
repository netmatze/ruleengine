using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using SimpleExpressionEvaluator;

namespace RuleEngine
{
    public class RuleValidator
    {
        public bool ValidateExpressionRules<T>(T value, Rule rule)
        {
            if (rule.ProcessingRule != string.Empty)
            {
                Evaluator evaluator = new Evaluator();
                return evaluator.Evaluate<T>(rule.ProcessingRule, value);
            }
            return false;
        }

        public bool ValidateExpressionRulesDynamic(dynamic value, Rule rule)
        {
            if (rule.ProcessingRule != string.Empty)
            {
                Evaluator evaluator = new Evaluator();
                return evaluator.EvaluateDynamic(rule.ProcessingRule, value);
            }
            return false;
        }

        public bool ValidateExpressionRules(dynamic value, dynamic[] collection, Rule rule)
        {
            if (rule.ProcessingRule != string.Empty)
            {
                Evaluator evaluator = new Evaluator();
                return evaluator.EvaluateDynamic(rule.ProcessingRule, value, collection);
            }
            return false;
        }

        public bool ValidateExpressionRulesAll<T>(T value, Rule[] rules)
        {
            Evaluator evaluator = new Evaluator();
            foreach (var rule in rules)
            {
                if (rule.ProcessingRule != string.Empty)
                {
                    if (!evaluator.Evaluate<T>(rule.ProcessingRule, value))
                        return false;
                }
            }
            return true;
        }

        public bool ValidateExpressionRulesAll(dynamic value, Rule[] rules)
        {
            Evaluator evaluator = new Evaluator();
            foreach (var rule in rules)
            {
                if (rule.ProcessingRule != string.Empty)
                {
                    if (!evaluator.EvaluateDynamic(rule.ProcessingRule, value))
                        return false;
                }
            }
            return true;
        }


        public bool ValidateExpressionRulesAny<T>(T value, Rule[] rules)
        {
            Evaluator evaluator = new Evaluator();
            foreach (var rule in rules)
            {
                if (rule.ProcessingRule != string.Empty)
                {
                    if (evaluator.Evaluate<T>(rule.ProcessingRule, value))
                        return true;
                }
            }
            return false;
        }

        public bool ValidateExpressionRulesAny(dynamic value, Rule[] rules)
        {
            Evaluator evaluator = new Evaluator();
            foreach (var rule in rules)
            {
                if (rule.ProcessingRule != string.Empty)
                {
                    if (evaluator.EvaluateDynamic(rule.ProcessingRule, value))
                        return true;
                }
            }
            return false;
        }

        public bool ValidateExpressionRulesAll<T>(T[] values, Rule[] rules)
        {
            Evaluator evaluator = new Evaluator();
            foreach (var value in values)
            {
                foreach (var rule in rules)
                {
                    if (rule.ProcessingRule != string.Empty)
                    {
                        if (!evaluator.Evaluate<T>(rule.ProcessingRule, value))
                            return false;
                    }
                }
            }
            return true;
        }

        public bool ValidateExpressionRulesAll(dynamic[] values, Rule[] rules)
        {
            Evaluator evaluator = new Evaluator();
            foreach (var value in values)
            {
                foreach (var rule in rules)
                {
                    if (rule.ProcessingRule != string.Empty)
                    {
                        if (!evaluator.EvaluateDynamic(rule.ProcessingRule, value))
                            return false;
                    }
                }
            }
            return true;
        }

        public bool ValidateExpressionRulesAll(DynamicBaseClass value, Rule[] rules)
        {
            Evaluator evaluator = new Evaluator();            
            foreach (var rule in rules)
            {
                if (rule.ProcessingRule != string.Empty)
                {
                    if (!evaluator.EvaluateDynamic(rule.ProcessingRule, value))
                        return false;
                }
            }
            return true;
        }

        public bool ValidateExpressionRulesAll(DynamicBaseClass[] values, Rule[] rules)
        {
            Evaluator evaluator = new Evaluator();            
            foreach (var rule in rules)
            {
                if (rule.ProcessingRule != string.Empty)
                {
                    if (!evaluator.EvaluateDynamic(rule.ProcessingRule, values))
                        return false;
                }
            }
            return true;
        }

        public List<Tuple<T, Rule, bool>> ValidateExpressionRules<T>(T[] values, Rule[] rules)
        {
            Evaluator evaluator = new Evaluator();
            List<Tuple<T, Rule, bool>> list = new List<Tuple<T, Rule, bool>>();
            foreach (var value in values)
            {
                foreach (var rule in rules)
                {
                    if (rule.ProcessingRule != string.Empty)
                    {
                        if (evaluator.Evaluate<T>(rule.ProcessingRule, value))
                        {
                            list.Add(new Tuple<T, Rule, bool>(value, rule, true));
                        }                            
                        else
                        {
                            list.Add(new Tuple<T, Rule, bool>(value, rule, false));
                        }
                    }
                }
            }
            return list;
        }

        public List<Tuple<dynamic, Rule, bool>> ValidateExpressionRules(dynamic[] values, Rule[] rules)
        {
            Evaluator evaluator = new Evaluator();
            List<Tuple<dynamic, Rule, bool>> list = new List<Tuple<dynamic, Rule, bool>>();
            foreach (var value in values)
            {
                foreach (var rule in rules)
                {
                    if (rule.ProcessingRule != string.Empty)
                    {
                        if (evaluator.EvaluateDynamic(rule.ProcessingRule, value))
                        {
                            list.Add(new Tuple<dynamic, Rule, bool>(value, rule, true));
                        }
                        else
                        {
                            list.Add(new Tuple<dynamic, Rule, bool>(value, rule, false));
                        }
                    }
                }
            }
            return list;
        }

        public bool ValidateExpressionRulesAll<T>(T[] values, string evaluatorExpression)
        {
            Evaluator evaluator = new Evaluator();
            var tuple = evaluator.PreEvaluate<T>(evaluatorExpression);
            foreach (var value in values)
            {
                if (!evaluator.ExecuteEvaluate<T>(tuple, value))
                    return false;             
            }
            return true;
        }

        public bool ValidateExpressionRulesAll(dynamic[] values, string evaluatorExpression)
        {
            Evaluator evaluator = new Evaluator();
            var tuple = evaluator.PreEvaluateDynamic(evaluatorExpression);
            foreach (var value in values)
            {
                if (!evaluator.ExecuteEvaluateDynamic(tuple, value))
                    return false;
            }
            return true;
        }

        public int ValidateExpressionRulesCount<T>(T[] values, Rule[] rules)
        {
            var count = 0;
            Evaluator evaluator = new Evaluator();
            foreach (var value in values)
            {
                var passedAllRules = true;
                foreach (var rule in rules)
                {
                    if (rule.ProcessingRule != string.Empty)
                    {
                        if (!evaluator.Evaluate<T>(rule.ProcessingRule, value))
                            passedAllRules = false;
                    }
                }
                if (passedAllRules)
                    count++;
            }
            return count;
        }

        public int ValidateExpressionRulesCount(dynamic[] values, Rule[] rules)
        {
            var count = 0;
            Evaluator evaluator = new Evaluator();
            foreach (var value in values)
            {
                var passedAllRules = true;
                foreach (var rule in rules)
                {
                    if (rule.ProcessingRule != string.Empty)
                    {
                        if (!evaluator.EvaluateDynamic(rule.ProcessingRule, value))
                            passedAllRules = false;
                    }
                }
                if (passedAllRules)
                    count++;
            }
            return count;
        }

        public bool ValidateExpressionRulesAny<T>(T[] values, Rule[] rules)
        {
            Evaluator evaluator = new Evaluator();
            foreach (var value in values)
            {
                bool validated = false;
                foreach (var rule in rules)
                {
                    if (rule.ProcessingRule != string.Empty)
                    {
                        if (evaluator.Evaluate<T>(rule.ProcessingRule, value))
                        {
                            validated = true;
                            break;
                        }
                    }    
                }
                if (!validated)
                    return false;
            }
            return true;
        }

        public bool ValidateExpressionRulesAny(dynamic[] values, Rule[] rules)
        {
            Evaluator evaluator = new Evaluator();
            foreach (var value in values)
            {
                bool validated = false;
                foreach (var rule in rules)
                {
                    if (rule.ProcessingRule != string.Empty)
                    {
                        if (evaluator.EvaluateDynamic(rule.ProcessingRule, value))
                        {
                            validated = true;
                            break;
                        }
                    }
                }
                if (!validated)
                    return false;
            }
            return true;
        }

        public bool ValidateExpressionRulesAny<T>(T[] values, List<Rule> rules)
        {
            Evaluator evaluator = new Evaluator();
            foreach (var value in values)
            {
                bool validated = false;
                foreach (var rule in rules)
                {
                    if (rule.ProcessingRule != string.Empty)
                    {
                        if (evaluator.Evaluate<T>(rule.ProcessingRule, value))
                        {
                            validated = true;
                            break;
                        }
                    }
                }
                if (!validated)
                    return false;
            }
            return true;
        }

        public bool ValidateExpressionRulesAny(dynamic[] values, List<Rule> rules)
        {
            Evaluator evaluator = new Evaluator();
            foreach (var value in values)
            {
                bool validated = false;
                foreach (var rule in rules)
                {
                    if (rule.ProcessingRule != string.Empty)
                    {
                        if (evaluator.EvaluateDynamic(rule.ProcessingRule, value))
                        {
                            validated = true;
                            break;
                        }
                    }
                }
                if (!validated)
                    return false;
            }
            return true;
        }

        public bool ValidateRules<T>(T value, Func<T, bool> rule)
        {
            return rule(value);
        }

        public bool ValidateRules<T>(T[] values, Func<T, bool> rule)
        {
            foreach (var value in values)
            {
                if (!rule(value))
                    return false;
            }
            return true;
        }

        public bool ValidateRuleSum<T>(T[] values, Rule rule)
        {
            dynamic sum = Activator.CreateInstance(values.GetType().GetElementType().GetProperty(rule.PropertyName).PropertyType);
            foreach (var value in values)
            {
                dynamic innerValue = value.GetType().GetProperty(rule.PropertyName).GetValue(value, null);
                sum += innerValue;                
            }            
            dynamic func = BuildGenericFunction(rule, sum);
            if (!func(sum))
               return false;
            return true;
        }

        public int ValidateRuleCount<T>(T[] values, Rule rule)
        {
            var count = 0;
            Evaluator evaluator = new Evaluator();
            foreach (var value in values)
            {
                if (rule.ProcessingRule != string.Empty)
                {
                    if (evaluator.Evaluate<T>(rule.ProcessingRule, value))
                        count++;
                }
            }
            return count;
        }

        public bool ValidateRulesSum<T>(IEnumerable<T> values, IEnumerable<Rule> rules)
        {
            foreach (var rule in rules)
            {
                dynamic sum = Activator.CreateInstance(
                    values.GetType().GetElementType().GetProperty(rule.PropertyName).PropertyType);
                foreach (var value in values)
                {
                    dynamic innerValue = value.GetType().
                        GetProperty(rule.PropertyName).GetValue(value, null);
                    sum += innerValue;
                }
                dynamic func = BuildGenericFunction(rule, sum);
                if (!func(sum))
                    return false;                
            }
            return true;
        }

        public bool ValidateRulesSumAny<T>(IEnumerable<T> values, IEnumerable<Rule> rules)
        {
            bool validated = false;
            foreach (var rule in rules)
            {
                dynamic sum = Activator.CreateInstance(
                    values.GetType().GetElementType().GetProperty(rule.PropertyName).PropertyType);
                foreach (var value in values)
                {
                    dynamic innerValue = value.GetType().
                        GetProperty(rule.PropertyName).GetValue(value, null);
                    sum += innerValue;
                }
                dynamic func = BuildGenericFunction(rule, sum);
                if (func(sum))
                    validated = true;
                break;
            }
            if (!validated)
               return false;
            return true;
        }        

        public bool ValidateRuleAvg<T>(IEnumerable<T> values, Rule rule)
        {
            dynamic sum = Activator.CreateInstance(values.GetType().GetElementType().GetProperty(rule.PropertyName).PropertyType);
            var counter = 0;
            foreach (var value in values)
            {
                dynamic innerValue = value.GetType().GetProperty(rule.PropertyName).GetValue(value, null);
                sum += innerValue;
                counter++;
            }
            dynamic avg = sum / counter;
            dynamic func = BuildGenericFunction(rule, avg);
            if (!func(avg))
                return false;
            return true;
        }

        public bool ValidateRuleAvg<T>(IEnumerable<T> values, IEnumerable<Rule> rules)
        {
            foreach (var rule in rules)
            {
                dynamic sum = Activator.CreateInstance(values.GetType().
                    GetElementType().GetProperty(rule.PropertyName).PropertyType);
                var counter = 0;
                foreach (var value in values)
                {
                    dynamic innerValue = value.GetType().
                        GetProperty(rule.PropertyName).GetValue(value, null);
                    sum += innerValue;
                    counter++;
                }
                dynamic avg = sum / counter;
                dynamic func = BuildGenericFunction(rule, avg);
                if (!func(avg))
                    return false;
            }
            return true;
        }

        private object BuildGenericFunction(Rule rule, object sum)
        {
            ExpressionBuilder expressionBuilder = new ExpressionBuilder();
            System.Type specificType = sum.GetType();
            var param = Expression.Parameter(specificType);
            Expression expression = expressionBuilder.
                BuildExpression(specificType, rule.Operator_, rule.Value, param);
            MethodInfo method = this.GetType().GetMethod("BuildLambdaFunc", 
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo generic = method.MakeGenericMethod(specificType);
            object func = generic.Invoke(this, new object[] { expression, param });
            return func;
        }

        private Func<T, bool> BuildLambdaFunc<T>(
            Expression expression, ParameterExpression param)
        {
            Func<T, bool> func = Expression.Lambda<Func<T, bool>>
                (expression, param).Compile();
            return func;
        }

        //var func = CreateGenericTypeAndMethod.MakeGenericMethod(this, "BuildLambdaFunc", specificType, new object[] { expression, param });

        private void Sum<T, T1>(T[] values, Rule rule)
        {

        }

        public bool ValidateRulesAll<T>(T value, Func<T, bool>[] rules)
        {
            foreach (var rule in rules)
            {
                if (!rule(value))
                    return false;
            }
            return true;
        }        

        public bool ValidateRulesAny<T>(T value, Func<T, bool>[] rules)
        {
            foreach (var rule in rules)
            {
                if (rule(value))
                    return true;
            }
            return false;
        }

        public bool ValidateRulesAll<T>(T[] values, Func<T, bool>[] rules)
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

        public int ValidateRulesCount<T>(T[] values, Func<T, bool>[] rules)
        {
            var count = 0;
            foreach (var value in values)
            {
                var passedAllRules = true;
                foreach (var rule in rules)
                {
                    if (!rule(value))
                        passedAllRules = false;
                }
                if (passedAllRules)
                    count++;

            }
            return count;
        }

        public int ValidateRuleCount<T>(T[] values, Func<T, bool> rule)
        {
            var count = 0;
            foreach (var value in values)
            {
                if (rule(value))
                    count++;
            }
            return count;
        }

        public bool ValidateRulesAny<T>(T[] values, Func<T, bool>[] rules)
        {
            foreach (var value in values)
            {
                bool validated = false;
                foreach (var rule in rules)
                {
                    if (rule(value))
                    {
                        validated = true;
                        break;
                    }
                }
                if (!validated)
                    return false;
            }
            return true;
        }

        public bool ValidateValuesAny<T>(List<T> values, Func<T, bool> rule)
        {
            foreach (var value in values)
            {
                if (rule(value))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ValidateValuesAny<T>(List<T> values, Func<T, bool>[] rules)
        {
            foreach (var value in values)
            {
                bool validated = false;
                foreach (var rule in rules)
                {
                    if (rule(value))
                    {
                        validated = true;
                    }
                    else
                    {
                        validated = false;
                        break;
                    }
                }
                if (validated)
                    return true;
            }
            return true;
        }
    }
}
