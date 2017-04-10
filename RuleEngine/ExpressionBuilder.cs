using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;
using System.Diagnostics;

namespace RuleEngine
{
    internal class ExpressionBuilder
    {
        public Expression BuildExpression<T>(Operator ruleOperator, object value, ParameterExpression parameterExpression)
        {
            ExpressionType expressionType = new ExpressionType();
            var leftOperand = parameterExpression;
            var rightOperand = Expression.Constant(Convert.ChangeType(value, typeof(T)));
            var expressionTypeValue = (ExpressionType)expressionType.GetType().GetField(Enum.GetName(typeof(Operator), ruleOperator)).GetValue(ruleOperator);
            return CastBuildExpression(expressionTypeValue, value, leftOperand, rightOperand);
        }

        public Expression BuildExpression(Type type, Operator ruleOperator, object value, ParameterExpression parameterExpression)
        {
            ExpressionType expressionType = new ExpressionType();
            var leftOperand = parameterExpression;
            var rightOperand = Expression.Constant(Convert.ChangeType(value, type));
            var expressionTypeValue = (ExpressionType)expressionType.GetType().GetField(Enum.GetName(typeof(Operator), ruleOperator)).GetValue(ruleOperator);
            return CastBuildExpression(expressionTypeValue, value, leftOperand, rightOperand);
        }

        public Expression BuildExpression<T>(string propertyName, Operator ruleOperator, object value, ParameterExpression parameterExpression)
        {
            ExpressionType expressionType = new ExpressionType();
            var leftOperand = MemberExpression.Property(parameterExpression, propertyName);
            var rightOperand = Expression.Constant(Convert.ChangeType(value, value.GetType()));
            FieldInfo fieldInfo = expressionType.GetType().GetField(Enum.GetName(typeof(Operator), ruleOperator));
            var expressionTypeValue = (ExpressionType)fieldInfo.GetValue(ruleOperator);
            return CastBuildExpression(expressionTypeValue, value, leftOperand, rightOperand);
        }

        public Tuple<Expression, ParameterExpression> BuildExpression<T>(string propertyName, Operator ruleOperator, 
            ParameterExpression parameterExpression, List<T> values)
        {                    
            ParameterExpression listExpression = Expression.Parameter(typeof(List<T>));
            ParameterExpression counterExpression = Expression.Parameter(typeof(int));
            ParameterExpression toExpression = Expression.Parameter(typeof(int));
            ParameterExpression arrayExpression = Expression.Parameter(typeof(T[]));
            ParameterExpression valueExpression = Expression.Parameter(typeof(T));
            ParameterExpression checkExpression = Expression.Parameter(typeof(T));
            ParameterExpression returnExpression = Expression.Parameter(typeof(bool));
            MemberExpression memberExpression = MemberExpression.Property(parameterExpression, propertyName);
            Expression expression = memberExpression.Expression;
            var type = memberExpression.Type;
            ParameterExpression propertyExpression = Expression.Parameter(type);
            ParameterExpression localPropertyExpression = Expression.Parameter(type);

            LabelTarget breakLabel = Expression.Label();
            PropertyInfo result = typeof(List<T>).GetProperty("Count");
            MethodInfo toArray = typeof(List<T>).GetMethod("ToArray");
            var toArrayName = toArray.Name;
            MethodInfo getGetMethod = result.GetGetMethod();
            ConstantExpression constantExpression = Expression.Constant(true);
            if (ruleOperator == Operator.NotFoundIn)
            {
                constantExpression = Expression.Constant(false);
            }
            Expression loop = Expression.Block(
                new ParameterExpression[] { toExpression, arrayExpression, valueExpression, counterExpression, 
                returnExpression, propertyExpression, localPropertyExpression, listExpression },
                Expression.Assign(listExpression, Expression.Constant(values)),
                Expression.Assign(toExpression, Expression.Call(listExpression, getGetMethod)),
                Expression.Assign(arrayExpression, Expression.Call(listExpression, toArray)),
                Expression.Assign(propertyExpression, MemberExpression.Property(checkExpression, propertyName)),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.LessThan(counterExpression, toExpression),
                        Expression.Block(
                            Expression.Assign(valueExpression, Expression.ArrayAccess(arrayExpression, counterExpression)),
                            Expression.Assign(localPropertyExpression, Expression.Property(valueExpression, propertyName)),
                            Expression.IfThen(
                                Expression.Equal(propertyExpression, localPropertyExpression),
                                Expression.Block(Expression.Assign(returnExpression, constantExpression),
                                    Expression.Break(breakLabel))),
                            Expression.Assign(Expression.ArrayAccess(arrayExpression, counterExpression), checkExpression),
                            Expression.PostIncrementAssign(counterExpression)),
                        Expression.Break(breakLabel)
                        ), breakLabel
                    ),
                    Expression.And(returnExpression, constantExpression)
                );
            return new Tuple<Expression, ParameterExpression>(Expression.Block(loop), checkExpression);
        }
        
        public BlockExpression ForEachExpression<T>(string propertyName, Operator ruleOperator, List<T> values, 
            ParameterExpression parameterExpression, T value)
        {
           // ParameterExpression varExpr = Expression.Variable(typeof(int), "sampleVar");
           // BlockExpression blockExpr = Expression.Block(
           //    new ParameterExpression[] { varExpr },
           //    Expression.Assign(varExpr, Expression.Constant(1)),
           //    Expression.Add(varExpr, Expression.Constant(5))
           //);

           // // Print out the expressions from the block expression.
           // Debug.WriteLine("The expressions from the block expression:");
           // foreach (var expr in blockExpr.Expressions)
           //     Debug.WriteLine(expr.ToString());

           // Debug.WriteLine("The result of executing the expression tree:");
           // // The following statement first creates an expression tree, 
           // // then compiles it, and then executes it.
           // Debug.WriteLine(
           //     Expression.Lambda<Func<int>>(blockExpr).Compile()());

            //ParameterExpression listExpression = Expression.Parameter(typeof(List<T>));
            //ParameterExpression counterExpression = Expression.Parameter(typeof(int));
            //ParameterExpression toExpression = Expression.Parameter(typeof(int));
            //ParameterExpression arrayExpression = Expression.Parameter(typeof(T[]));
            //ParameterExpression valueExpression = Expression.Parameter(typeof(T));
            //ParameterExpression checkExpression = Expression.Parameter(typeof(T));
            //ParameterExpression returnExpression = Expression.Parameter(typeof(bool));
            //MemberExpression memberExpression = MemberExpression.Property(parameterExpression, propertyName);
            //Expression expression = memberExpression.Expression;
            //var type = memberExpression.Type;
            //ParameterExpression propertyExpression = Expression.Parameter(type);
            //ParameterExpression localPropertyExpression = Expression.Parameter(type);

            //LabelTarget breakLabel = Expression.Label();
            //PropertyInfo result = typeof(List<T>).GetProperty("Count");
            //MethodInfo toArray = typeof(List<T>).GetMethod("ToArray");
            //var toArrayName = toArray.Name;
            //MethodInfo getGetMethod = result.GetGetMethod();

            //Expression loop = Expression.Block(
            //    new ParameterExpression[] { toExpression, arrayExpression, valueExpression, counterExpression, 
            //        returnExpression, propertyExpression, localPropertyExpression },               
            //    Expression.Assign(toExpression, Expression.Call(listExpression, getGetMethod)),
            //    Expression.Assign(arrayExpression, Expression.Call(listExpression, toArray)),
            //    Expression.Assign(propertyExpression, MemberExpression.Property(checkExpression, propertyName)),
            //    Expression.Loop(
            //        Expression.IfThenElse(
            //            Expression.LessThan(counterExpression, toExpression),
            //            Expression.Block(
            //                Expression.Assign(valueExpression, Expression.ArrayAccess(arrayExpression, counterExpression)),
            //                Expression.Assign(localPropertyExpression, Expression.Property(valueExpression, propertyName)),
            //                Expression.IfThen(
            //                    Expression.Equal(propertyExpression, localPropertyExpression),
            //                    Expression.Block(Expression.Assign(returnExpression, Expression.Constant(true)),
            //                        Expression.Break(breakLabel))),
            //                Expression.Assign(Expression.ArrayAccess(arrayExpression, counterExpression), checkExpression),
            //                Expression.PostIncrementAssign(counterExpression)),
            //            Expression.Break(breakLabel)
            //            ), breakLabel
            //        ),
            //        Expression.And(returnExpression, Expression.Constant(true))
            //    );
            //var compiledExpression = Expression.Lambda<Func<List<T>, T, bool>>(
            //    Expression.Block(loop), listExpression, checkExpression).Compile();

            //var resultExpression = compiledExpression(values, value);

            //var testArray = new int[10];

            //for (int i = 0; i < testArray.Count(); i++)
            //{
            //    Debug.WriteLine(testArray[i]);
            //    testArray[i] = i;
            //}

            ////Question: I would like to remove the second parameter 'counter'
            //var resultExpression = compiledExpression(testArray.ToList(), 16);

            //for (int i = 0; i < testArray.Count(); i++)
            //{
            //    Debug.WriteLine(testArray[i]);
            //}

            //ParameterExpression listExpression = Expression.Parameter(typeof(List<int>));
            //ParameterExpression counterExpression = Expression.Parameter(typeof(int));
            //ParameterExpression toExpression = Expression.Parameter(typeof(int));
            //ParameterExpression arrayExpression = Expression.Parameter(typeof(int[]));
            //ParameterExpression valueExpression = Expression.Parameter(typeof(int));
            //ParameterExpression checkExpression = Expression.Parameter(typeof(int));
            //ParameterExpression returnExpression = Expression.Parameter(typeof(bool));

            //LabelTarget breakLabel = Expression.Label();
            //PropertyInfo result = typeof(List<int>).GetProperty("Count");
            //MethodInfo toArray = typeof(List<int>).GetMethod("ToArray");
            ////MethodInfo toString = typeof(int).GetMethods().First(m => m.Name == "ToString");
            //var toArrayName = toArray.Name;
            //MethodInfo getGetMethod = result.GetGetMethod();

            //Expression loop = Expression.Block(
            //    new ParameterExpression[] { toExpression, arrayExpression, valueExpression, counterExpression, returnExpression },
            //    Expression.Assign(toExpression, Expression.Call(listExpression, getGetMethod)),               
            //    Expression.Assign(arrayExpression, Expression.Call(listExpression, toArray)),
            //    Expression.Loop(
            //        Expression.IfThenElse(
            //            Expression.LessThan(counterExpression, toExpression),
            //            Expression.Block(                        
            //                Expression.Assign(valueExpression, Expression.ArrayAccess(arrayExpression, counterExpression)),
                           
            //                Expression.IfThen(
            //                    Expression.Equal(valueExpression, checkExpression),
            //                    Expression.Block(Expression.Assign(returnExpression, Expression.Constant(true)),
            //                        Expression.Break(breakLabel))),
            //                Expression.Assign(Expression.ArrayAccess(arrayExpression, counterExpression), checkExpression),
            //                Expression.PostIncrementAssign(counterExpression)),
            //            Expression.Break(breakLabel)                               
            //            ), breakLabel
            //        ),
            //        Expression.And(returnExpression, Expression.Constant(true))              
            //    );

            //var compiledExpression = Expression.Lambda<Func<List<int>, int, bool>>(
            //    Expression.Block(loop), listExpression, checkExpression).Compile();

            //var testArray = new int[10];

            //for (int i = 0; i < testArray.Count(); i++)
            //{
            //    Debug.WriteLine(testArray[i]);
            //    testArray[i] = i;
            //}

            ////Question: I would like to remove the second parameter 'counter'
            //var resultExpression = compiledExpression(testArray.ToList(), 16);

            //for (int i = 0; i < testArray.Count(); i++)
            //{
            //    Debug.WriteLine(testArray[i]);
            //}
            //var item = Expression.Variable(typeof(T), itemName);
            //var enumerator = Expression.Variable(typeof(IEnumerator<T>), "enumerator");
            //var param = Expression.Parameter(typeof(IEnumerable<T>));
            //var doMoveNext = Expression.Call(enumerator, typeof(IEnumerator).GetMethod("MoveNext"));
            //var assignToEnum = Expression.Assign(enumerator, Expression.Call(param, typeof(IEnumerable<T>).GetMethod("GetEnumerator")));
            //var assignCurrent = Expression.Assign(item, Expression.Property(enumerator, "Current"));
            //var breakLabel = Expression.Label();
            //var foreachBlock = Expression.Block(
            //    new ParameterExpression[] { item, enumerator, param },
            //    assignToEnum,
            //    Expression.Loop(
            //        Expression.IfThenElse(
            //        Expression.NotEqual(doMoveNext, Expression.Constant(false)),
            //            assignCurrent
            //        , Expression.Break(breakLabel))
            //    , breakLabel)
            //);
            //Debug.WriteLine("The expressions from the block expression:");
            //foreach (var expr in foreachBlock.Expressions)
            //    Debug.WriteLine(expr.ToString());
            //var resultExpression = Expression.Lambda<Action<List<T>>>(foreachBlock);
            //var result = resultExpression.Compile();
            //result(list);
            //return Expression.Block(loop);
            return null;
        }

        private Expression CastBuildExpression(ExpressionType expressionTypeValue, object value, Expression leftOperand, ConstantExpression rightOperand)
        {
            if (leftOperand.Type == rightOperand.Type)
            {
                return Expression.MakeBinary(expressionTypeValue, leftOperand, rightOperand);
            }
            else if (CanChangeType(value, leftOperand.Type))
            {
                if (rightOperand.Type != typeof(bool))
                {
                    rightOperand = Expression.Constant(Convert.ChangeType(value, leftOperand.Type));
                }
                else
                {
                    leftOperand = Expression.Constant(Convert.ChangeType(value, rightOperand.Type));
                }
                return Expression.MakeBinary(expressionTypeValue, leftOperand, rightOperand);
            }            
            return null;
        }

        private bool CanChangeType(object sourceType, Type targetType)
        {
            try
            {
                Convert.ChangeType(sourceType, targetType);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
