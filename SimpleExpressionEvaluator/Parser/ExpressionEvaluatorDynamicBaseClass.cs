using SimpleExpressionEvaluator.AbstractSyntaxTree;
using SimpleExpressionEvaluator.AbstractSyntaxTree.AggregateFunctions;
using SimpleExpressionEvaluator.AbstractSyntaxTree.StringFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleExpressionEvaluator.Parser
{
    public class ExpressionEvaluatorDynamicBaseClass : EvaluatorBase
    {
        public bool EvaluateDynamicBaseClass(List<AbstractSyntaxTreeNode> postfixList,
         Dictionary<string, AbstractSyntaxTreeNode> symbolTable, DynamicBaseClass[] objectValues)
        {
            int setNameValue = 0;
            bool thenNode = false;
            bool elseNode = false;
            foreach (var item in postfixList)
            {
                if (item is IntegerNode || item is DoubleNode || item is DateTimeNode ||
                    item is BooleanNode || item is StringNode || item is NullNode)
                {
                    valueStack.Push(item);
                }
                else if (item is UpperNode || item is LowerNode || item is LengthNode ||
                    item is TrimNode || item is LeftTrimNode || item is RightTrimNode)
                {
                    if (valueStack.Peek() is StringNode)
                    {
                        if (item is UpperNode)
                        {
                            var stringNode = valueStack.Pop() as StringNode;
                            stringNode.Value = stringNode.Value.ToUpper();
                            valueStack.Push(stringNode);
                        }
                        else if (item is LowerNode)
                        {
                            var stringNode = valueStack.Pop() as StringNode;
                            stringNode.Value = stringNode.Value.ToLower();
                            valueStack.Push(stringNode);
                        }
                        else if (item is LengthNode)
                        {
                            var stringNode = valueStack.Pop() as StringNode;
                            IntegerNode integerNode = new IntegerNode();
                            integerNode.Value = stringNode.Value.Length;
                            valueStack.Push(integerNode);
                        }
                        else if (item is TrimNode)
                        {
                            var stringNode = valueStack.Pop() as StringNode;
                            stringNode.Value = stringNode.Value.Trim();
                            valueStack.Push(stringNode);
                        }
                        else if (item is LeftTrimNode)
                        {
                            var stringNode = valueStack.Pop() as StringNode;
                            stringNode.Value = stringNode.Value.TrimStart();
                            valueStack.Push(stringNode);
                        }
                        else if (item is RightTrimNode)
                        {
                            var stringNode = valueStack.Pop() as StringNode;
                            stringNode.Value = stringNode.Value.TrimEnd();
                            valueStack.Push(stringNode);
                        }
                    }
                }
                else if (item is SetNode)
                {
                    var equalNode = postfixList[setNameValue - 1];
                    var valueNode = postfixList[setNameValue - 2];
                    var nameNode = postfixList[setNameValue - 3];
                    if (equalNode is EqualNode)
                    {
                        var pop = valueStack.Pop();
                        setList.Add(((VariableNode)nameNode).Name, valueNode);
                    }
                    else
                    {
                        var pop = valueStack.Pop();
                        var methodValueNode = postfixList[setNameValue - 1];
                        var methodNode = postfixList[setNameValue - 2];
                        if (methodNode is VariableNode)
                        {
                            setList.Add(((VariableNode)methodNode).Name, methodValueNode);
                        }
                    }
                }
                else if (item is ThenNode)
                {
                    thenNode = true;
                }
                else if (item is ElseNode)
                {
                    elseNode = true;
                }
                else if (item is FieldNode)
                {
                    if (symbolTable.ContainsKey("Field." + ((FieldNode)item).Value.Name))
                    {
                        var property = objectValues.GetType().GetProperty(((FieldNode)item).Value.Name);
                        if (property != null)
                        {
                            object value = property.GetValue(objectValues, null);
                            int intOutValue;
                            double doubleOutValue;
                            bool boolOutValue;
                            DateTime dateTimeOutValue;
                            if (value == null)
                            {
                                NullNode nullNode = new NullNode();
                                valueStack.Push(nullNode);
                            }
                            else if (int.TryParse(value.ToString(), out intOutValue))
                            {
                                IntegerNode integerNode = new IntegerNode();
                                integerNode.Value = intOutValue;
                                valueStack.Push(integerNode);
                            }
                            else if (double.TryParse(value.ToString(), out doubleOutValue))
                            {
                                DoubleNode doubleNode = new DoubleNode();
                                doubleNode.Value = doubleOutValue;
                                valueStack.Push(doubleNode);
                            }
                            else if (bool.TryParse(value.ToString(), out boolOutValue))
                            {
                                BooleanNode booleanNode = new BooleanNode();
                                booleanNode.Value = boolOutValue;
                                valueStack.Push(booleanNode);
                            }
                            else if (DateTime.TryParse(value.ToString(), out dateTimeOutValue))
                            {
                                DateTimeNode dateTimeNode = new DateTimeNode();
                                dateTimeNode.Value = dateTimeOutValue;
                                valueStack.Push(dateTimeNode);
                            }
                            else
                            {
                                StringNode stringNode = new StringNode();
                                stringNode.Value = value.ToString();
                                valueStack.Push(stringNode);
                            }
                        }
                    }
                }
                else if (item is CollectionNode)
                {
                    if (symbolTable.ContainsKey("Collection." + ((CollectionNode)item).Value.Name))
                    {
                        if (objectValues.Count() > 0)
                        {
                            var property = objectValues.First().GetType().GetProperty(((CollectionNode)item).Value.Name);
                            if (property != null)
                            {
                                var collectionNode = item as CollectionNode;
                                if (collectionNode.AggregateFunction != null)
                                {
                                    if (collectionNode.AggregateFunction is AggregateAverageNode)
                                    {
                                        var sum = 0.0;
                                        var counter = 0.0;
                                        foreach (var collectionItem in objectValues)
                                        {
                                            var localProperty = collectionItem.GetType().GetProperty(((CollectionNode)item).Value.Name);
                                            if (localProperty != null && localProperty.PropertyType.IsValueType)
                                            {
                                                var valueItem = localProperty.GetValue(collectionItem, null);
                                                int intOutValue = 0;
                                                double doubleOutValue = 0.0;
                                                if (int.TryParse(valueItem.ToString(), out intOutValue))
                                                {
                                                    sum += Convert.ToInt32(valueItem);
                                                }
                                                else if (double.TryParse(valueItem.ToString(), out doubleOutValue))
                                                {
                                                    sum += Convert.ToDouble(valueItem);
                                                }
                                                counter++;
                                            }
                                        }
                                        DoubleNode doubleNode = new DoubleNode();
                                        doubleNode.Value = sum / counter;
                                        valueStack.Push(doubleNode);
                                    }
                                    else if (collectionNode.AggregateFunction is AggregateSumNode)
                                    {
                                        var sum = 0.0;
                                        foreach (var collectionItem in objectValues)
                                        {
                                            var localProperty = collectionItem.GetType().GetProperty(((CollectionNode)item).Value.Name);
                                            if (localProperty != null && localProperty.PropertyType.IsValueType)
                                            {
                                                var valueItem = localProperty.GetValue(collectionItem, null);
                                                int intOutValue = 0;
                                                double doubleOutValue = 0.0;
                                                if (int.TryParse(valueItem.ToString(), out intOutValue))
                                                {
                                                    sum += Convert.ToInt32(valueItem);
                                                }
                                                else if (double.TryParse(valueItem.ToString(), out doubleOutValue))
                                                {
                                                    sum += Convert.ToDouble(valueItem);
                                                }
                                            }
                                        }
                                        DoubleNode doubleNode = new DoubleNode();
                                        doubleNode.Value = sum;
                                        valueStack.Push(doubleNode);
                                    }
                                    else if (collectionNode.AggregateFunction is AggregateCountNode)
                                    {
                                        var counter = 0;
                                        foreach (var collectionItem in objectValues)
                                        {
                                            var localProperty = collectionItem.GetType().GetProperty(((CollectionNode)item).Value.Name);
                                            if (localProperty != null && localProperty.PropertyType.IsValueType)
                                            {
                                                counter++;
                                            }
                                        }
                                        IntegerNode integerNode = new IntegerNode();
                                        integerNode.Value = counter;
                                        valueStack.Push(integerNode);
                                    }
                                    else if (collectionNode.AggregateFunction is AggregateMaxNode)
                                    {
                                        var maxValue = double.MinValue;
                                        foreach (var collectionItem in objectValues)
                                        {
                                            var localProperty = collectionItem.GetType().GetProperty(((CollectionNode)item).Value.Name);
                                            if (localProperty != null && localProperty.PropertyType.IsValueType)
                                            {
                                                var valueItem = localProperty.GetValue(collectionItem, null);
                                                int intOutValue = 0;
                                                double doubleOutValue = 0.0;
                                                if (int.TryParse(valueItem.ToString(), out intOutValue))
                                                {
                                                    if (intOutValue > maxValue)
                                                    {
                                                        maxValue = intOutValue;
                                                    }
                                                }
                                                else if (double.TryParse(valueItem.ToString(), out doubleOutValue))
                                                {
                                                    if (doubleOutValue > maxValue)
                                                    {
                                                        maxValue = intOutValue;
                                                    }
                                                }
                                            }
                                        }
                                        DoubleNode doubleNode = new DoubleNode();
                                        doubleNode.Value = maxValue;
                                        valueStack.Push(doubleNode);
                                    }
                                    else if (collectionNode.AggregateFunction is AggregateMinNode)
                                    {
                                        var minValue = double.MaxValue;
                                        foreach (var collectionItem in objectValues)
                                        {
                                            var localProperty = collectionItem.GetType().GetProperty(((CollectionNode)item).Value.Name);
                                            if (localProperty != null && localProperty.PropertyType.IsValueType)
                                            {
                                                var valueItem = localProperty.GetValue(collectionItem, null);
                                                int intOutValue = 0;
                                                double doubleOutValue = 0.0;
                                                if (int.TryParse(valueItem.ToString(), out intOutValue))
                                                {
                                                    if (intOutValue < minValue)
                                                    {
                                                        minValue = intOutValue;
                                                    }
                                                }
                                                else if (double.TryParse(valueItem.ToString(), out doubleOutValue))
                                                {
                                                    if (doubleOutValue < minValue)
                                                    {
                                                        minValue = intOutValue;
                                                    }
                                                }
                                            }
                                        }
                                        DoubleNode doubleNode = new DoubleNode();
                                        doubleNode.Value = minValue;
                                        valueStack.Push(doubleNode);
                                    }
                                }
                                else
                                {
                                    valueStack.Push(collectionNode);
                                }
                            }
                        }
                    }
                }
                else if (item is ObjectNode)
                {
                    if (symbolTable.ContainsKey(((ObjectNode)item).Name))
                    {
                        if (((ObjectNode)item).ObjectValue.Type == ObjectType.Collection)
                        {
                            var filteredObjectValues = ((ObjectNode)item).ObjectValue;
                            if (((ObjectNode)item).Filter != null && ((ObjectNode)item).Filter.FilterItems.Count() > 0)
                            {
                                filteredObjectValues = EvaluateDynamicObject(((ObjectNode)item).Filter.FilterItems, symbolTable, ((ObjectNode)item).ObjectValue);
                            }
                            if (((ObjectNode)item).AggregateFunction != null)
                            {
                                if (((ObjectNode)item).AggregateFunction is AggregateAverageNode)
                                {
                                    var sum = 0.0;
                                    var counter = 0.0;
                                    foreach (var field in filteredObjectValues.Fields)
                                    {
                                        var propertyField = field.GetType().GetProperty(((ObjectNode)item).PropertyName);
                                        if (propertyField != null)
                                        {
                                            var objValue = propertyField.GetValue(field, null);
                                            int intOutValue = 0;
                                            double doubleOutValue = 0.0;
                                            if (int.TryParse(objValue.ToString(), out intOutValue))
                                            {
                                                sum += Convert.ToInt32(objValue);
                                            }
                                            else if (double.TryParse(objValue.ToString(), out doubleOutValue))
                                            {
                                                sum += Convert.ToDouble(objValue);
                                            }
                                            counter++;
                                        }
                                    }
                                    DoubleNode doubleNode = new DoubleNode();
                                    doubleNode.Value = sum / counter;
                                    valueStack.Push(doubleNode);
                                }
                                else if (((ObjectNode)item).AggregateFunction is AggregateSumNode)
                                {
                                    var sum = 0.0;
                                    foreach (var field in filteredObjectValues.Fields)
                                    {
                                        var propertyField = field.GetType().GetProperty(((ObjectNode)item).PropertyName);
                                        if (propertyField != null)
                                        {
                                            var objValue = propertyField.GetValue(field, null);
                                            int intOutValue = 0;
                                            double doubleOutValue = 0.0;
                                            if (int.TryParse(objValue.ToString(), out intOutValue))
                                            {
                                                sum += Convert.ToInt32(objValue);
                                            }
                                            else if (double.TryParse(objValue.ToString(), out doubleOutValue))
                                            {
                                                sum += Convert.ToDouble(objValue);
                                            }
                                        }
                                    }
                                    DoubleNode doubleNode = new DoubleNode();
                                    doubleNode.Value = sum;
                                    valueStack.Push(doubleNode);
                                }
                                else if (((ObjectNode)item).AggregateFunction is AggregateCountNode)
                                {
                                    var counter = 0;
                                    if (!string.IsNullOrEmpty(((ObjectNode)item).PropertyName))
                                    {
                                        foreach (var field in filteredObjectValues.Fields)
                                        {
                                            var propertyField = field.GetType().GetProperty(((ObjectNode)item).PropertyName);
                                            if (propertyField != null)
                                            {
                                                counter++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        counter = filteredObjectValues.Fields.Count;
                                    }
                                    IntegerNode integerNode = new IntegerNode();
                                    integerNode.Value = counter;
                                    valueStack.Push(integerNode);
                                }
                                else if (((ObjectNode)item).AggregateFunction is AggregateMaxNode)
                                {
                                    var maxValue = double.MinValue;
                                    foreach (var field in filteredObjectValues.Fields)
                                    {
                                        var propertyField = field.GetType().GetProperty(((ObjectNode)item).PropertyName);
                                        if (propertyField != null)
                                        {
                                            var objValue = propertyField.GetValue(field, null);
                                            int intOutValue = 0;
                                            double doubleOutValue = 0.0;
                                            if (int.TryParse(objValue.ToString(), out intOutValue))
                                            {
                                                if (intOutValue > maxValue)
                                                {
                                                    maxValue = intOutValue;
                                                }
                                            }
                                            else if (double.TryParse(objValue.ToString(), out doubleOutValue))
                                            {
                                                if (doubleOutValue > maxValue)
                                                {
                                                    maxValue = intOutValue;
                                                }
                                            }
                                        }
                                    }
                                    DoubleNode doubleNode = new DoubleNode();
                                    doubleNode.Value = maxValue;
                                    valueStack.Push(doubleNode);
                                }
                                else if (((ObjectNode)item).AggregateFunction is AggregateMinNode)
                                {
                                    var minValue = double.MaxValue;
                                    foreach (var field in filteredObjectValues.Fields)
                                    {
                                        var propertyField = field.GetType().GetProperty(((ObjectNode)item).PropertyName);
                                        if (propertyField != null)
                                        {
                                            var objValue = propertyField.GetValue(field, null);
                                            int intOutValue = 0;
                                            double doubleOutValue = 0.0;
                                            if (int.TryParse(objValue.ToString(), out intOutValue))
                                            {
                                                if (intOutValue < minValue)
                                                {
                                                    minValue = intOutValue;
                                                }
                                            }
                                            else if (double.TryParse(objValue.ToString(), out doubleOutValue))
                                            {
                                                if (doubleOutValue < minValue)
                                                {
                                                    minValue = intOutValue;
                                                }
                                            }
                                        }
                                    }
                                    DoubleNode doubleNode = new DoubleNode();
                                    doubleNode.Value = minValue;
                                    valueStack.Push(doubleNode);
                                }
                            }
                            else
                            {
                                valueStack.Push(item);
                            }
                        }
                        else
                        {
                            foreach (var objectValue in objectValues)
                            {
                                if (objectValue.ReferenceName == ((ObjectNode)item).Name)
                                {
                                    var property = objectValue.GetType().GetProperty(((ObjectNode)item).PropertyName);
                                    object obj = objectValue;
                                    foreach (var field in objectValue.Fields)
                                    {
                                        var propertyField = field.GetType().GetProperty(((ObjectNode)item).PropertyName);
                                        if (propertyField != null)
                                        {
                                            property = propertyField;
                                            obj = field;
                                            break;
                                        }
                                    }
                                    if (property != null)
                                    {
                                        object value = property.GetValue(obj, null);
                                        int intOutValue;
                                        double doubleOutValue;
                                        bool boolOutValue;
                                        DateTime dateTimeOutValue;
                                        if (value == null)
                                        {
                                            NullNode nullNode = new NullNode();
                                            valueStack.Push(nullNode);
                                        }
                                        else if (int.TryParse(value.ToString(), out intOutValue))
                                        {
                                            IntegerNode integerNode = new IntegerNode();
                                            integerNode.Value = intOutValue;
                                            valueStack.Push(integerNode);
                                        }
                                        else if (double.TryParse(value.ToString(), out doubleOutValue))
                                        {
                                            DoubleNode doubleNode = new DoubleNode();
                                            doubleNode.Value = doubleOutValue;
                                            valueStack.Push(doubleNode);
                                        }
                                        else if (bool.TryParse(value.ToString(), out boolOutValue))
                                        {
                                            BooleanNode booleanNode = new BooleanNode();
                                            booleanNode.Value = boolOutValue;
                                            valueStack.Push(booleanNode);
                                        }
                                        else if (DateTime.TryParse(value.ToString(), out dateTimeOutValue))
                                        {
                                            DateTimeNode dateTimeNode = new DateTimeNode();
                                            dateTimeNode.Value = dateTimeOutValue;
                                            valueStack.Push(dateTimeNode);
                                        }
                                        else
                                        {
                                            StringNode stringNode = new StringNode();
                                            stringNode.Value = value.ToString();
                                            valueStack.Push(stringNode);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (item is VariableNode)
                {
                    if (symbolTable.ContainsKey(((VariableNode)item).Name))
                    {
                        var property = objectValues.GetType().GetProperty(((VariableNode)item).Name);
                        if (property != null)
                        {
                            object value = property.GetValue(objectValues, null);
                            int intOutValue;
                            double doubleOutValue;
                            bool boolOutValue;
                            DateTime dateTimeOutValue;
                            if (value == null)
                            {
                                NullNode nullNode = new NullNode();
                                valueStack.Push(nullNode);
                            }
                            else if (int.TryParse(value.ToString(), out intOutValue))
                            {
                                IntegerNode integerNode = new IntegerNode();
                                integerNode.Value = intOutValue;
                                valueStack.Push(integerNode);
                            }
                            else if (double.TryParse(value.ToString(), out doubleOutValue))
                            {
                                DoubleNode doubleNode = new DoubleNode();
                                doubleNode.Value = doubleOutValue;
                                valueStack.Push(doubleNode);
                            }
                            else if (bool.TryParse(value.ToString(), out boolOutValue))
                            {
                                BooleanNode booleanNode = new BooleanNode();
                                booleanNode.Value = boolOutValue;
                                valueStack.Push(booleanNode);
                            }
                            else if (DateTime.TryParse(value.ToString(), out dateTimeOutValue))
                            {
                                DateTimeNode dateTimeNode = new DateTimeNode();
                                dateTimeNode.Value = dateTimeOutValue;
                                valueStack.Push(dateTimeNode);
                            }
                            else
                            {
                                StringNode stringNode = new StringNode();
                                stringNode.Value = value.ToString();
                                valueStack.Push(stringNode);
                            }
                        }
                    }
                }
                else if (item is MethodCallNode)
                {
                    if (!thenNode && !elseNode)
                    {
                        MethodCallNodeExecuteCall(objectValues, item);
                    }
                    else if (thenNode)
                    {
                        var methodNodeName = ((MethodCallNode)item).Name;
                        var list = new List<Tuple<AbstractSyntaxTreeNode, AbstractSyntaxTreeNode>>();
                        list.Add(new Tuple<AbstractSyntaxTreeNode, AbstractSyntaxTreeNode>(item, item));
                        thenList.Add(new Tuple<string, string>(methodNodeName, null), list);
                        thenNode = false;
                    }
                    else if (elseNode)
                    {
                        var methodNodeName = ((MethodCallNode)item).Name;
                        var list = new List<Tuple<AbstractSyntaxTreeNode, AbstractSyntaxTreeNode>>();
                        list.Add(new Tuple<AbstractSyntaxTreeNode, AbstractSyntaxTreeNode>(item, item));
                        elseList.Add(new Tuple<string, string>(methodNodeName, null), list);
                        elseNode = false;
                    }
                }
                else if (item is AddNode)
                {
                    var secondoperand = valueStack.Pop();
                    var firstoperand = valueStack.Pop();
                    if (secondoperand is IntegerNode && firstoperand is IntegerNode)
                    {
                        IntegerNode integerNode = new IntegerNode();
                        integerNode.Value = ((IntegerNode)firstoperand).Value +
                            ((IntegerNode)secondoperand).Value;
                        valueStack.Push(integerNode);
                    }
                    else if (secondoperand is DoubleNode && firstoperand is DoubleNode)
                    {
                        DoubleNode doubleNode = new DoubleNode();
                        doubleNode.Value = ((DoubleNode)firstoperand).Value + ((DoubleNode)secondoperand).Value;
                        valueStack.Push(doubleNode);
                    }
                    else if (secondoperand is IntegerNode && firstoperand is DoubleNode)
                    {
                        DoubleNode integerNode = new DoubleNode();
                        integerNode.Value = ((DoubleNode)firstoperand).Value + ((IntegerNode)secondoperand).Value;
                        valueStack.Push(integerNode);
                    }
                    else if (secondoperand is DoubleNode && firstoperand is IntegerNode)
                    {
                        DoubleNode doubleNode = new DoubleNode();
                        doubleNode.Value = ((IntegerNode)firstoperand).Value + ((DoubleNode)secondoperand).Value;
                        valueStack.Push(doubleNode);
                    }
                }
                else if (item is SubNode)
                {
                    var secondoperand = valueStack.Pop();
                    var firstoperand = valueStack.Pop();
                    if (secondoperand is IntegerNode && firstoperand is IntegerNode)
                    {
                        IntegerNode integerNode = new IntegerNode();
                        integerNode.Value = ((IntegerNode)firstoperand).Value - ((IntegerNode)secondoperand).Value;
                        valueStack.Push(integerNode);
                    }
                    else if (secondoperand is DoubleNode && firstoperand is DoubleNode)
                    {
                        DoubleNode doubleNode = new DoubleNode();
                        doubleNode.Value = ((DoubleNode)firstoperand).Value - ((DoubleNode)secondoperand).Value;
                        valueStack.Push(doubleNode);
                    }
                    else if (secondoperand is IntegerNode && firstoperand is DoubleNode)
                    {
                        DoubleNode integerNode = new DoubleNode();
                        integerNode.Value = ((DoubleNode)firstoperand).Value - ((IntegerNode)secondoperand).Value;
                        valueStack.Push(integerNode);
                    }
                    else if (secondoperand is DoubleNode && firstoperand is IntegerNode)
                    {
                        DoubleNode doubleNode = new DoubleNode();
                        doubleNode.Value = ((IntegerNode)firstoperand).Value - ((DoubleNode)secondoperand).Value;
                        valueStack.Push(doubleNode);
                    }
                }
                else if (item is MulNode)
                {
                    var secondoperand = valueStack.Pop();
                    var firstoperand = valueStack.Pop();
                    if (secondoperand is IntegerNode && firstoperand is IntegerNode)
                    {
                        IntegerNode integerNode = new IntegerNode();
                        integerNode.Value = ((IntegerNode)firstoperand).Value * ((IntegerNode)secondoperand).Value;
                        valueStack.Push(integerNode);
                    }
                    else if (secondoperand is DoubleNode && firstoperand is DoubleNode)
                    {
                        DoubleNode doubleNode = new DoubleNode();
                        doubleNode.Value = ((DoubleNode)firstoperand).Value * ((DoubleNode)secondoperand).Value;
                        valueStack.Push(doubleNode);
                    }
                    else if (secondoperand is IntegerNode && firstoperand is DoubleNode)
                    {
                        DoubleNode integerNode = new DoubleNode();
                        integerNode.Value = ((DoubleNode)firstoperand).Value * ((IntegerNode)secondoperand).Value;
                        valueStack.Push(integerNode);
                    }
                    else if (secondoperand is DoubleNode && firstoperand is IntegerNode)
                    {
                        DoubleNode doubleNode = new DoubleNode();
                        doubleNode.Value = ((IntegerNode)firstoperand).Value * ((DoubleNode)secondoperand).Value;
                        valueStack.Push(doubleNode);
                    }
                }
                else if (item is DivNode)
                {
                    var secondoperand = valueStack.Pop();
                    var firstoperand = valueStack.Pop();
                    if (secondoperand is IntegerNode && firstoperand is IntegerNode)
                    {
                        IntegerNode integerNode = new IntegerNode();
                        integerNode.Value = ((IntegerNode)firstoperand).Value / ((IntegerNode)secondoperand).Value;
                        valueStack.Push(integerNode);
                    }
                    else if (secondoperand is DoubleNode && firstoperand is DoubleNode)
                    {
                        DoubleNode doubleNode = new DoubleNode();
                        doubleNode.Value = ((DoubleNode)firstoperand).Value / ((DoubleNode)secondoperand).Value;
                        valueStack.Push(doubleNode);
                    }
                    else if (secondoperand is IntegerNode && firstoperand is DoubleNode)
                    {
                        DoubleNode integerNode = new DoubleNode();
                        integerNode.Value = ((DoubleNode)firstoperand).Value / ((IntegerNode)secondoperand).Value;
                        valueStack.Push(integerNode);
                    }
                    else if (secondoperand is DoubleNode && firstoperand is IntegerNode)
                    {
                        DoubleNode doubleNode = new DoubleNode();
                        doubleNode.Value = ((IntegerNode)firstoperand).Value / ((DoubleNode)secondoperand).Value;
                        valueStack.Push(doubleNode);
                    }
                }
                else if (item is ModuloNode)
                {
                    var secondoperand = valueStack.Pop();
                    var firstoperand = valueStack.Pop();
                    if (secondoperand is IntegerNode && firstoperand is IntegerNode)
                    {
                        IntegerNode integerNode = new IntegerNode();
                        integerNode.Value = ((IntegerNode)firstoperand).Value % ((IntegerNode)secondoperand).Value;
                        valueStack.Push(integerNode);
                    }
                    else if (secondoperand is DoubleNode && firstoperand is DoubleNode)
                    {
                        DoubleNode doubleNode = new DoubleNode();
                        doubleNode.Value = ((DoubleNode)firstoperand).Value % ((DoubleNode)secondoperand).Value;
                        valueStack.Push(doubleNode);
                    }
                    else if (secondoperand is IntegerNode && firstoperand is DoubleNode)
                    {
                        DoubleNode integerNode = new DoubleNode();
                        integerNode.Value = ((DoubleNode)firstoperand).Value % ((IntegerNode)secondoperand).Value;
                        valueStack.Push(integerNode);
                    }
                    else if (secondoperand is DoubleNode && firstoperand is IntegerNode)
                    {
                        DoubleNode doubleNode = new DoubleNode();
                        doubleNode.Value = ((IntegerNode)firstoperand).Value % ((DoubleNode)secondoperand).Value;
                        valueStack.Push(doubleNode);
                    }
                }
                else if (item is LikeNode)
                {
                    var secondoperand = valueStack.Pop();
                    var firstoperand = valueStack.Pop();
                    if (secondoperand is StringNode && firstoperand is StringNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        var firstoperatorString = ((StringNode)firstoperand).Value;
                        var secondperatorString = ((StringNode)secondoperand).Value;
                        var result = Like(firstoperatorString, secondperatorString);
                        booleanNode.Value = result;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is StringNode && firstoperand is IntegerNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        var firstoperatorString = ((IntegerNode)firstoperand).Value.ToString();
                        var secondperatorString = ((StringNode)secondoperand).Value;
                        var result = Like(firstoperatorString, secondperatorString);
                        booleanNode.Value = result;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is StringNode && firstoperand is DoubleNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        var firstoperatorString = ((DoubleNode)firstoperand).Value.ToString();
                        var secondperatorString = ((StringNode)secondoperand).Value;
                        var result = Like(firstoperatorString, secondperatorString);
                        booleanNode.Value = result;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is StringNode && firstoperand is DateTimeNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        var firstoperatorString = ((DateTimeNode)firstoperand).Value.ToString();
                        var secondperatorString = ((StringNode)secondoperand).Value;
                        var result = Like(firstoperatorString, secondperatorString);
                        booleanNode.Value = result;
                        valueStack.Push(booleanNode);
                    }
                }
                else if (item is AggregateInNode)
                {
                    var secondoperand = valueStack.Pop();
                    var firstoperand = valueStack.Pop();
                    if (secondoperand is ObjectNode && firstoperand is StringNode)
                    {
                        var filteredObjectValues = ((ObjectNode)secondoperand).ObjectValue;
                        if (((ObjectNode)secondoperand).Filter != null && ((ObjectNode)secondoperand).Filter.FilterItems.Count() > 0)
                        {
                            filteredObjectValues = EvaluateDynamicObject(((ObjectNode)secondoperand).Filter.FilterItems, symbolTable, ((ObjectNode)secondoperand).ObjectValue);
                        }
                        var contains = false;
                        foreach (var field in filteredObjectValues.Fields)
                        {
                            var propertyField = field.GetType().GetProperty(((ObjectNode)secondoperand).PropertyName);
                            if (propertyField != null)
                            {
                                var objValue = propertyField.GetValue(field, null);
                                if (objValue == ((StringNode)firstoperand).Value)
                                {
                                    contains = true;
                                    break;
                                }
                            }
                        }
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = contains;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is ObjectNode && firstoperand is IntegerNode)
                    {
                        var filteredObjectValues = ((ObjectNode)secondoperand).ObjectValue;
                        if (((ObjectNode)secondoperand).Filter != null && ((ObjectNode)secondoperand).Filter.FilterItems.Count() > 0)
                        {
                            filteredObjectValues = EvaluateDynamicObject(((ObjectNode)secondoperand).Filter.FilterItems, symbolTable, ((ObjectNode)secondoperand).ObjectValue);
                        }
                        var contains = false;
                        foreach (var field in filteredObjectValues.Fields)
                        {
                            var propertyField = field.GetType().GetProperty(((ObjectNode)secondoperand).PropertyName);
                            if (propertyField != null)
                            {
                                var objValue = propertyField.GetValue(field, null);
                                if (objValue == ((IntegerNode)firstoperand).Value)
                                {
                                    contains = true;
                                    break;
                                }
                            }
                        }
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = contains;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is ObjectNode && firstoperand is DoubleNode)
                    {
                        var filteredObjectValues = ((ObjectNode)secondoperand).ObjectValue;
                        if (((ObjectNode)secondoperand).Filter != null && ((ObjectNode)secondoperand).Filter.FilterItems.Count() > 0)
                        {
                            filteredObjectValues = EvaluateDynamicObject(((ObjectNode)secondoperand).Filter.FilterItems, symbolTable, ((ObjectNode)secondoperand).ObjectValue);
                        }
                        var contains = false;
                        foreach (var field in filteredObjectValues.Fields)
                        {
                            var propertyField = field.GetType().GetProperty(((ObjectNode)secondoperand).PropertyName);
                            if (propertyField != null)
                            {
                                var objValue = propertyField.GetValue(field, null);
                                if (objValue == ((DoubleNode)firstoperand).Value)
                                {
                                    contains = true;
                                    break;
                                }
                            }
                        }
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = contains;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is ObjectNode && firstoperand is DateTimeNode)
                    {
                        var filteredObjectValues = ((ObjectNode)secondoperand).ObjectValue;
                        if (((ObjectNode)secondoperand).Filter != null && ((ObjectNode)secondoperand).Filter.FilterItems.Count() > 0)
                        {
                            filteredObjectValues = EvaluateDynamicObject(((ObjectNode)secondoperand).Filter.FilterItems, symbolTable, ((ObjectNode)secondoperand).ObjectValue);
                        }
                        var contains = false;
                        foreach (var field in filteredObjectValues.Fields)
                        {
                            var propertyField = field.GetType().GetProperty(((ObjectNode)secondoperand).PropertyName);
                            if (propertyField != null)
                            {
                                var objValue = propertyField.GetValue(field, null);
                                if (objValue == ((DateTimeNode)firstoperand).Value)
                                {
                                    contains = true;
                                    break;
                                }
                            }
                        }
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = contains;
                        valueStack.Push(booleanNode);
                    }
                }
                else if (item is AggregateNotInNode)
                {
                    var secondoperand = valueStack.Pop();
                    var firstoperand = valueStack.Pop();
                    if (secondoperand is ObjectNode && firstoperand is StringNode)
                    {
                        var filteredObjectValues = ((ObjectNode)secondoperand).ObjectValue;
                        if (((ObjectNode)secondoperand).Filter != null && ((ObjectNode)secondoperand).Filter.FilterItems.Count() > 0)
                        {
                            filteredObjectValues = EvaluateDynamicObject(((ObjectNode)secondoperand).Filter.FilterItems, symbolTable, ((ObjectNode)secondoperand).ObjectValue);
                        }
                        var contains = true;
                        foreach (var field in filteredObjectValues.Fields)
                        {
                            var propertyField = field.GetType().GetProperty(((ObjectNode)secondoperand).PropertyName);
                            if (propertyField != null)
                            {
                                var objValue = propertyField.GetValue(field, null);
                                if (objValue == ((StringNode)firstoperand).Value)
                                {
                                    contains = false;
                                    break;
                                }
                            }
                        }
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = contains;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is ObjectNode && firstoperand is IntegerNode)
                    {
                        var filteredObjectValues = ((ObjectNode)secondoperand).ObjectValue;
                        if (((ObjectNode)secondoperand).Filter != null && ((ObjectNode)secondoperand).Filter.FilterItems.Count() > 0)
                        {
                            filteredObjectValues = EvaluateDynamicObject(((ObjectNode)secondoperand).Filter.FilterItems, symbolTable, ((ObjectNode)secondoperand).ObjectValue);
                        }
                        var contains = true;
                        foreach (var field in filteredObjectValues.Fields)
                        {
                            var propertyField = field.GetType().GetProperty(((ObjectNode)secondoperand).PropertyName);
                            if (propertyField != null)
                            {
                                var objValue = propertyField.GetValue(field, null);
                                if (objValue == ((IntegerNode)firstoperand).Value)
                                {
                                    contains = false;
                                    break;
                                }
                            }
                        }
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = contains;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is ObjectNode && firstoperand is DoubleNode)
                    {
                        var filteredObjectValues = ((ObjectNode)secondoperand).ObjectValue;
                        if (((ObjectNode)secondoperand).Filter != null && ((ObjectNode)secondoperand).Filter.FilterItems.Count() > 0)
                        {
                            filteredObjectValues = EvaluateDynamicObject(((ObjectNode)secondoperand).Filter.FilterItems, symbolTable, ((ObjectNode)secondoperand).ObjectValue);
                        }
                        var contains = true;
                        foreach (var field in filteredObjectValues.Fields)
                        {
                            var propertyField = field.GetType().GetProperty(((ObjectNode)secondoperand).PropertyName);
                            if (propertyField != null)
                            {
                                var objValue = propertyField.GetValue(field, null);
                                if (objValue == ((DoubleNode)firstoperand).Value)
                                {
                                    contains = false;
                                    break;
                                }
                            }
                        }
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = contains;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is ObjectNode && firstoperand is DateTimeNode)
                    {
                        var filteredObjectValues = ((ObjectNode)secondoperand).ObjectValue;
                        if (((ObjectNode)secondoperand).Filter != null && ((ObjectNode)secondoperand).Filter.FilterItems.Count() > 0)
                        {
                            filteredObjectValues = EvaluateDynamicObject(((ObjectNode)secondoperand).Filter.FilterItems, symbolTable, ((ObjectNode)secondoperand).ObjectValue);
                        }
                        var contains = true;
                        foreach (var field in filteredObjectValues.Fields)
                        {
                            var propertyField = field.GetType().GetProperty(((ObjectNode)secondoperand).PropertyName);
                            if (propertyField != null)
                            {
                                var objValue = propertyField.GetValue(field, null);
                                if (objValue == ((DateTimeNode)firstoperand).Value)
                                {
                                    contains = false;
                                    break;
                                }
                            }
                        }
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = contains;
                        valueStack.Push(booleanNode);
                    }
                }
                else if (item is EqualNode)
                {
                    var secondoperand = valueStack.Pop();
                    var firstoperand = valueStack.Pop();
                    if (secondoperand is IntegerNode && firstoperand is IntegerNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((IntegerNode)firstoperand).Value == ((IntegerNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DoubleNode && firstoperand is DoubleNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((DoubleNode)firstoperand).Value == ((DoubleNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is BooleanNode && firstoperand is BooleanNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((BooleanNode)firstoperand).Value == ((BooleanNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is StringNode && firstoperand is StringNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((StringNode)firstoperand).Value == ((StringNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DateTimeNode && firstoperand is DateTimeNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((DateTimeNode)firstoperand).Value == ((DateTimeNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is IntegerNode && firstoperand is DoubleNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((DoubleNode)firstoperand).Value == ((IntegerNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DoubleNode && firstoperand is IntegerNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((IntegerNode)firstoperand).Value == ((DoubleNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DateTimeNode && firstoperand is DateTimeNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((DateTimeNode)firstoperand).Value == ((DateTimeNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DateTimeNode && firstoperand is StringNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((StringNode)firstoperand).Value == ((DateTimeNode)secondoperand).Value.ToString();
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is StringNode && firstoperand is DateTimeNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((DateTimeNode)firstoperand).Value.ToString() == ((StringNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                }
                else if (item is UnEqualNode)
                {
                    var secondoperand = valueStack.Pop();
                    var firstoperand = valueStack.Pop();
                    if (secondoperand is IntegerNode && firstoperand is IntegerNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((IntegerNode)firstoperand).Value != ((IntegerNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DoubleNode && firstoperand is DoubleNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((DoubleNode)firstoperand).Value != ((DoubleNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is BooleanNode && firstoperand is BooleanNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((BooleanNode)firstoperand).Value != ((BooleanNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is StringNode && firstoperand is StringNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((StringNode)firstoperand).Value != ((StringNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DateTimeNode && firstoperand is DateTimeNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((DateTimeNode)firstoperand).Value != ((DateTimeNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is IntegerNode && firstoperand is DoubleNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((DoubleNode)firstoperand).Value != ((IntegerNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DoubleNode && firstoperand is IntegerNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((IntegerNode)firstoperand).Value != ((DoubleNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DateTimeNode && firstoperand is DateTimeNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((DateTimeNode)firstoperand).Value != ((DateTimeNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DateTimeNode && firstoperand is StringNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((StringNode)firstoperand).Value != ((DateTimeNode)secondoperand).Value.ToString();
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is StringNode && firstoperand is DateTimeNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((DateTimeNode)firstoperand).Value.ToString() != ((StringNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                }
                else if (item is SmallerThenNode)
                {
                    var secondoperand = valueStack.Pop();
                    var firstoperand = valueStack.Pop();
                    if (secondoperand is IntegerNode && firstoperand is IntegerNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((IntegerNode)firstoperand).Value < ((IntegerNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DoubleNode && firstoperand is DoubleNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((DoubleNode)firstoperand).Value < ((DoubleNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is IntegerNode && firstoperand is DoubleNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((DoubleNode)firstoperand).Value < ((IntegerNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DoubleNode && firstoperand is IntegerNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((IntegerNode)firstoperand).Value < ((DoubleNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DateTimeNode && firstoperand is DateTimeNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((DateTimeNode)firstoperand).Value < ((DateTimeNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DateTimeNode && firstoperand is StringNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        var str = ((StringNode)firstoperand).Value;
                        booleanNode.Value = DateTime.Parse(str) < ((DateTimeNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is StringNode && firstoperand is DateTimeNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        var str = ((StringNode)secondoperand).Value;
                        booleanNode.Value = ((DateTimeNode)firstoperand).Value < DateTime.Parse(str);
                        valueStack.Push(booleanNode);
                    }
                }
                else if (item is SmallerThenOrEqualNode)
                {
                    var secondoperand = valueStack.Pop();
                    var firstoperand = valueStack.Pop();
                    if (secondoperand is IntegerNode && firstoperand is IntegerNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((IntegerNode)firstoperand).Value <= ((IntegerNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DoubleNode && firstoperand is DoubleNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((DoubleNode)firstoperand).Value <= ((DoubleNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is IntegerNode && firstoperand is DoubleNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((DoubleNode)firstoperand).Value <= ((IntegerNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DoubleNode && firstoperand is IntegerNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((IntegerNode)firstoperand).Value <= ((DoubleNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DateTimeNode && firstoperand is DateTimeNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((DateTimeNode)firstoperand).Value <= ((DateTimeNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DateTimeNode && firstoperand is StringNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        var str = ((StringNode)firstoperand).Value;
                        booleanNode.Value = DateTime.Parse(str) <= ((DateTimeNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is StringNode && firstoperand is DateTimeNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        var str = ((StringNode)secondoperand).Value;
                        booleanNode.Value = ((DateTimeNode)firstoperand).Value <= DateTime.Parse(str);
                        valueStack.Push(booleanNode);
                    }
                }
                else if (item is GreaterThenNode)
                {
                    var secondoperand = valueStack.Pop();
                    var firstoperand = valueStack.Pop();
                    if (secondoperand is IntegerNode && firstoperand is IntegerNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((IntegerNode)firstoperand).Value > ((IntegerNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DoubleNode && firstoperand is DoubleNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((DoubleNode)firstoperand).Value > ((DoubleNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is IntegerNode && firstoperand is DoubleNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((DoubleNode)firstoperand).Value > ((IntegerNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DoubleNode && firstoperand is IntegerNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((IntegerNode)firstoperand).Value > ((DoubleNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DateTimeNode && firstoperand is DateTimeNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((DateTimeNode)firstoperand).Value > ((DateTimeNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DateTimeNode && firstoperand is StringNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        var str = ((StringNode)firstoperand).Value;
                        booleanNode.Value = DateTime.Parse(str) > ((DateTimeNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is StringNode && firstoperand is DateTimeNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        var str = ((StringNode)secondoperand).Value;
                        booleanNode.Value = ((DateTimeNode)firstoperand).Value > DateTime.Parse(str);
                        valueStack.Push(booleanNode);
                    }
                }
                else if (item is GreaterThenOrEqualNode)
                {
                    var secondoperand = valueStack.Pop();
                    var firstoperand = valueStack.Pop();
                    if (secondoperand is IntegerNode && firstoperand is IntegerNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((IntegerNode)firstoperand).Value >= ((IntegerNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DoubleNode && firstoperand is DoubleNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((DoubleNode)firstoperand).Value >= ((DoubleNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is IntegerNode && firstoperand is DoubleNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((DoubleNode)firstoperand).Value >= ((IntegerNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DoubleNode && firstoperand is IntegerNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((IntegerNode)firstoperand).Value >= ((DoubleNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DateTimeNode && firstoperand is DateTimeNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((DateTimeNode)firstoperand).Value >= ((DateTimeNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is DateTimeNode && firstoperand is StringNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        var str = ((StringNode)firstoperand).Value;
                        booleanNode.Value = DateTime.Parse(str) >= ((DateTimeNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                    else if (secondoperand is StringNode && firstoperand is DateTimeNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        var str = ((StringNode)secondoperand).Value;
                        booleanNode.Value = ((DateTimeNode)firstoperand).Value >= DateTime.Parse(str);
                        valueStack.Push(booleanNode);
                    }
                }
                else if (item is IsNode)
                {
                    var secondoperand = valueStack.Pop();
                    var firstoperand = valueStack.Pop();
                    if (secondoperand is NullNode && firstoperand is NullNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = true;
                        valueStack.Push(booleanNode);
                    }
                    else
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = false;
                        valueStack.Push(booleanNode);
                    }
                }
                else if (item is OrNode)
                {
                    var secondoperand = valueStack.Pop();
                    var firstoperand = valueStack.Pop();
                    if (secondoperand is BooleanNode && firstoperand is BooleanNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((BooleanNode)firstoperand).Value || ((BooleanNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                }
                else if (item is AndNode)
                {
                    var secondoperand = valueStack.Pop();
                    var firstoperand = valueStack.Pop();
                    if (secondoperand is BooleanNode && firstoperand is BooleanNode)
                    {
                        BooleanNode booleanNode = new BooleanNode();
                        booleanNode.Value = ((BooleanNode)firstoperand).Value && ((BooleanNode)secondoperand).Value;
                        valueStack.Push(booleanNode);
                    }
                }
                setNameValue++;
            }
            var lastValue = false;
            var reveredStack = new Stack<AbstractSyntaxTreeNode>();
            while (valueStack.Count > 0)
            {
                reveredStack.Push(valueStack.Pop());
            }
            var ebene = reveredStack.Count - 1;
            while (reveredStack.Count > 0)
            {
                var endValue = reveredStack.Pop();
                if (endValue is BooleanNode)
                {
                    if (((BooleanNode)endValue).Value)
                    {
                        if (setList.Count > 0)
                        {
                            foreach (var item in setList)
                            {
                                var propertyName = item.Key;
                                var objectToSet = item.Value;
                                var objectCastValue = new object();
                                if (objectToSet is IntegerNode)
                                {
                                    objectCastValue = ((IntegerNode)objectToSet).Value;
                                }
                                else if (objectToSet is DoubleNode)
                                {
                                    objectCastValue = ((DoubleNode)objectToSet).Value;
                                }
                                else if (objectToSet is BooleanNode)
                                {
                                    objectCastValue = ((BooleanNode)objectToSet).Value;
                                }
                                else if (objectToSet is StringNode)
                                {
                                    objectCastValue = ((StringNode)objectToSet).Value;
                                }
                                else if (objectToSet is DateTimeNode)
                                {
                                    objectCastValue = ((DateTimeNode)objectToSet).Value;
                                }
                                var property = objectValues.GetType().GetProperty(propertyName);
                                if (property != null)
                                    property.SetValue(objectValues, objectCastValue, null);
                                else
                                {
                                    var method = objectValues.GetType().GetMethod(propertyName);
                                    if (method != null)
                                    {
                                        method.Invoke(objectValues, new object[] { objectCastValue });
                                    }
                                }
                            }
                        }
                    }
                    if (((BooleanNode)endValue).Value)
                    {
                        if (ebene < thenList.Count)
                        {
                            var item = thenList.ToList()[ebene];
                            MethodCallNodeExecuteCall(objectValues, item.Value.First().Item2);
                            lastValue = ((BooleanNode)endValue).Value;
                            break;
                        }
                    }
                    else
                    {
                        if (ebene < elseList.Count)
                        {
                            var item = elseList.ToList()[ebene];
                            MethodCallNodeExecuteCall(objectValues, item.Value.First().Item2);
                            lastValue = ((BooleanNode)endValue).Value;
                            break;
                        }
                    }
                    lastValue = ((BooleanNode)endValue).Value;
                }
                ebene--;
            }
            return lastValue;
        }

        public DynamicBaseClass EvaluateDynamicObject(List<AbstractSyntaxTreeNode> postfixList,
           Dictionary<string, AbstractSyntaxTreeNode> symbolTable, DynamicBaseClass objectValue)
        {
            int setNameValue = 0;
            bool thenNode = false;
            bool elseNode = false;
            List<dynamic> fieldList = new List<dynamic>();
            Stack<AbstractSyntaxTreeNode> localValueStack = new Stack<AbstractSyntaxTreeNode>();
            foreach (var field in objectValue.Fields)
            {
                foreach (var item in postfixList)
                {
                    if (item is IntegerNode || item is DoubleNode || item is DateTimeNode ||
                        item is FieldNode || item is CollectionNode ||
                        item is BooleanNode || item is StringNode || item is NullNode)
                    {
                        localValueStack.Push(item);
                    }
                    else if (item is SetNode)
                    {
                        var equalNode = postfixList[setNameValue - 1];
                        var valueNode = postfixList[setNameValue - 2];
                        var nameNode = postfixList[setNameValue - 3];
                        if (equalNode is EqualNode)
                        {
                            var pop = localValueStack.Pop();
                            setList.Add(((VariableNode)nameNode).Name, valueNode);
                        }
                        else
                        {
                            var pop = localValueStack.Pop();
                            var methodValueNode = postfixList[setNameValue - 1];
                            var methodNode = postfixList[setNameValue - 2];
                            if (methodNode is VariableNode)
                            {
                                setList.Add(((VariableNode)methodNode).Name, methodValueNode);
                            }
                        }
                    }
                    else if (item is ThenNode)
                    {
                        thenNode = true;
                    }
                    else if (item is ElseNode)
                    {
                        elseNode = true;
                    }
                    else if (item is FieldNode)
                    {
                        if (symbolTable.ContainsKey("Field." + ((FieldNode)item).Value.Name))
                        {
                            var property = objectValue.GetType().GetProperty(((FieldNode)item).Value.Name);
                            if (property != null)
                            {
                                object value = property.GetValue(objectValue, null);
                                int intOutValue;
                                double doubleOutValue;
                                bool boolOutValue;
                                DateTime dateTimeOutValue;
                                if (value == null)
                                {
                                    NullNode nullNode = new NullNode();
                                    localValueStack.Push(nullNode);
                                }
                                else if (int.TryParse(value.ToString(), out intOutValue))
                                {
                                    IntegerNode integerNode = new IntegerNode();
                                    integerNode.Value = intOutValue;
                                    localValueStack.Push(integerNode);
                                }
                                else if (double.TryParse(value.ToString(), out doubleOutValue))
                                {
                                    DoubleNode doubleNode = new DoubleNode();
                                    doubleNode.Value = doubleOutValue;
                                    localValueStack.Push(doubleNode);
                                }
                                else if (bool.TryParse(value.ToString(), out boolOutValue))
                                {
                                    BooleanNode booleanNode = new BooleanNode();
                                    booleanNode.Value = boolOutValue;
                                    localValueStack.Push(booleanNode);
                                }
                                else if (DateTime.TryParse(value.ToString(), out dateTimeOutValue))
                                {
                                    DateTimeNode dateTimeNode = new DateTimeNode();
                                    dateTimeNode.Value = dateTimeOutValue;
                                    localValueStack.Push(dateTimeNode);
                                }
                                else
                                {
                                    StringNode stringNode = new StringNode();
                                    stringNode.Value = value.ToString();
                                    localValueStack.Push(stringNode);
                                }
                            }
                        }
                    }
                    else if (item is VariableNode)
                    {
                        if (symbolTable.ContainsKey(((VariableNode)item).Name))
                        {
                            var propertyField = field.GetType().GetProperty(((VariableNode)item).Name);
                            if (propertyField != null)
                            {
                                var value = propertyField.GetValue(field, null);
                                int intOutValue;
                                double doubleOutValue;
                                bool boolOutValue;
                                DateTime dateTimeOutValue;
                                if (value == null)
                                {
                                    NullNode nullNode = new NullNode();
                                    localValueStack.Push(nullNode);
                                }
                                else if (int.TryParse(value.ToString(), out intOutValue))
                                {
                                    IntegerNode integerNode = new IntegerNode();
                                    integerNode.Value = intOutValue;
                                    localValueStack.Push(integerNode);
                                }
                                else if (double.TryParse(value.ToString(), out doubleOutValue))
                                {
                                    DoubleNode doubleNode = new DoubleNode();
                                    doubleNode.Value = doubleOutValue;
                                    localValueStack.Push(doubleNode);
                                }
                                else if (bool.TryParse(value.ToString(), out boolOutValue))
                                {
                                    BooleanNode booleanNode = new BooleanNode();
                                    booleanNode.Value = boolOutValue;
                                    localValueStack.Push(booleanNode);
                                }
                                else if (DateTime.TryParse(value.ToString(), out dateTimeOutValue))
                                {
                                    DateTimeNode dateTimeNode = new DateTimeNode();
                                    dateTimeNode.Value = dateTimeOutValue;
                                    localValueStack.Push(dateTimeNode);
                                }
                                else
                                {
                                    StringNode stringNode = new StringNode();
                                    stringNode.Value = value.ToString();
                                    localValueStack.Push(stringNode);
                                }
                            }
                        }
                    }
                    else if (item is MethodCallNode)
                    {
                        if (!thenNode && !elseNode)
                        {
                            MethodCallNodeExecuteCall(objectValue, item);
                        }
                        else if (thenNode)
                        {
                            var methodNodeName = ((MethodCallNode)item).Name;
                            var list = new List<Tuple<AbstractSyntaxTreeNode, AbstractSyntaxTreeNode>>();
                            list.Add(new Tuple<AbstractSyntaxTreeNode, AbstractSyntaxTreeNode>(item, item));
                            thenList.Add(new Tuple<string, string>(methodNodeName, null), list);
                            thenNode = false;
                        }
                        else if (elseNode)
                        {
                            var methodNodeName = ((MethodCallNode)item).Name;
                            var list = new List<Tuple<AbstractSyntaxTreeNode, AbstractSyntaxTreeNode>>();
                            list.Add(new Tuple<AbstractSyntaxTreeNode, AbstractSyntaxTreeNode>(item, item));
                            elseList.Add(new Tuple<string, string>(methodNodeName, null), list);
                            elseNode = false;
                        }
                    }
                    else if (item is AddNode)
                    {
                        var secondoperand = localValueStack.Pop();
                        var firstoperand = localValueStack.Pop();
                        if (secondoperand is IntegerNode && firstoperand is IntegerNode)
                        {
                            IntegerNode integerNode = new IntegerNode();
                            integerNode.Value = ((IntegerNode)firstoperand).Value +
                                ((IntegerNode)secondoperand).Value;
                            localValueStack.Push(integerNode);
                        }
                        else if (secondoperand is DoubleNode && firstoperand is DoubleNode)
                        {
                            DoubleNode doubleNode = new DoubleNode();
                            doubleNode.Value = ((DoubleNode)firstoperand).Value + ((DoubleNode)secondoperand).Value;
                            localValueStack.Push(doubleNode);
                        }
                        else if (secondoperand is IntegerNode && firstoperand is DoubleNode)
                        {
                            DoubleNode integerNode = new DoubleNode();
                            integerNode.Value = ((DoubleNode)firstoperand).Value + ((IntegerNode)secondoperand).Value;
                            localValueStack.Push(integerNode);
                        }
                        else if (secondoperand is DoubleNode && firstoperand is IntegerNode)
                        {
                            DoubleNode doubleNode = new DoubleNode();
                            doubleNode.Value = ((IntegerNode)firstoperand).Value + ((DoubleNode)secondoperand).Value;
                            localValueStack.Push(doubleNode);
                        }
                    }
                    else if (item is SubNode)
                    {
                        var secondoperand = localValueStack.Pop();
                        var firstoperand = localValueStack.Pop();
                        if (secondoperand is IntegerNode && firstoperand is IntegerNode)
                        {
                            IntegerNode integerNode = new IntegerNode();
                            integerNode.Value = ((IntegerNode)firstoperand).Value - ((IntegerNode)secondoperand).Value;
                            localValueStack.Push(integerNode);
                        }
                        else if (secondoperand is DoubleNode && firstoperand is DoubleNode)
                        {
                            DoubleNode doubleNode = new DoubleNode();
                            doubleNode.Value = ((DoubleNode)firstoperand).Value - ((DoubleNode)secondoperand).Value;
                            localValueStack.Push(doubleNode);
                        }
                        else if (secondoperand is IntegerNode && firstoperand is DoubleNode)
                        {
                            DoubleNode integerNode = new DoubleNode();
                            integerNode.Value = ((DoubleNode)firstoperand).Value - ((IntegerNode)secondoperand).Value;
                            localValueStack.Push(integerNode);
                        }
                        else if (secondoperand is DoubleNode && firstoperand is IntegerNode)
                        {
                            DoubleNode doubleNode = new DoubleNode();
                            doubleNode.Value = ((IntegerNode)firstoperand).Value - ((DoubleNode)secondoperand).Value;
                            localValueStack.Push(doubleNode);
                        }
                    }
                    else if (item is MulNode)
                    {
                        var secondoperand = localValueStack.Pop();
                        var firstoperand = localValueStack.Pop();
                        if (secondoperand is IntegerNode && firstoperand is IntegerNode)
                        {
                            IntegerNode integerNode = new IntegerNode();
                            integerNode.Value = ((IntegerNode)firstoperand).Value * ((IntegerNode)secondoperand).Value;
                            localValueStack.Push(integerNode);
                        }
                        else if (secondoperand is DoubleNode && firstoperand is DoubleNode)
                        {
                            DoubleNode doubleNode = new DoubleNode();
                            doubleNode.Value = ((DoubleNode)firstoperand).Value * ((DoubleNode)secondoperand).Value;
                            localValueStack.Push(doubleNode);
                        }
                        else if (secondoperand is IntegerNode && firstoperand is DoubleNode)
                        {
                            DoubleNode integerNode = new DoubleNode();
                            integerNode.Value = ((DoubleNode)firstoperand).Value * ((IntegerNode)secondoperand).Value;
                            localValueStack.Push(integerNode);
                        }
                        else if (secondoperand is DoubleNode && firstoperand is IntegerNode)
                        {
                            DoubleNode doubleNode = new DoubleNode();
                            doubleNode.Value = ((IntegerNode)firstoperand).Value * ((DoubleNode)secondoperand).Value;
                            localValueStack.Push(doubleNode);
                        }
                    }
                    else if (item is DivNode)
                    {
                        var secondoperand = localValueStack.Pop();
                        var firstoperand = localValueStack.Pop();
                        if (secondoperand is IntegerNode && firstoperand is IntegerNode)
                        {
                            IntegerNode integerNode = new IntegerNode();
                            integerNode.Value = ((IntegerNode)firstoperand).Value / ((IntegerNode)secondoperand).Value;
                            localValueStack.Push(integerNode);
                        }
                        else if (secondoperand is DoubleNode && firstoperand is DoubleNode)
                        {
                            DoubleNode doubleNode = new DoubleNode();
                            doubleNode.Value = ((DoubleNode)firstoperand).Value / ((DoubleNode)secondoperand).Value;
                            localValueStack.Push(doubleNode);
                        }
                        else if (secondoperand is IntegerNode && firstoperand is DoubleNode)
                        {
                            DoubleNode integerNode = new DoubleNode();
                            integerNode.Value = ((DoubleNode)firstoperand).Value / ((IntegerNode)secondoperand).Value;
                            localValueStack.Push(integerNode);
                        }
                        else if (secondoperand is DoubleNode && firstoperand is IntegerNode)
                        {
                            DoubleNode doubleNode = new DoubleNode();
                            doubleNode.Value = ((IntegerNode)firstoperand).Value / ((DoubleNode)secondoperand).Value;
                            localValueStack.Push(doubleNode);
                        }
                    }
                    else if (item is ModuloNode)
                    {
                        var secondoperand = localValueStack.Pop();
                        var firstoperand = localValueStack.Pop();
                        if (secondoperand is IntegerNode && firstoperand is IntegerNode)
                        {
                            IntegerNode integerNode = new IntegerNode();
                            integerNode.Value = ((IntegerNode)firstoperand).Value % ((IntegerNode)secondoperand).Value;
                            localValueStack.Push(integerNode);
                        }
                        else if (secondoperand is DoubleNode && firstoperand is DoubleNode)
                        {
                            DoubleNode doubleNode = new DoubleNode();
                            doubleNode.Value = ((DoubleNode)firstoperand).Value % ((DoubleNode)secondoperand).Value;
                            localValueStack.Push(doubleNode);
                        }
                        else if (secondoperand is IntegerNode && firstoperand is DoubleNode)
                        {
                            DoubleNode integerNode = new DoubleNode();
                            integerNode.Value = ((DoubleNode)firstoperand).Value % ((IntegerNode)secondoperand).Value;
                            localValueStack.Push(integerNode);
                        }
                        else if (secondoperand is DoubleNode && firstoperand is IntegerNode)
                        {
                            DoubleNode doubleNode = new DoubleNode();
                            doubleNode.Value = ((IntegerNode)firstoperand).Value % ((DoubleNode)secondoperand).Value;
                            localValueStack.Push(doubleNode);
                        }
                    }
                    else if (item is LikeNode)
                    {
                        var secondoperand = localValueStack.Pop();
                        var firstoperand = localValueStack.Pop();
                        if (secondoperand is StringNode && firstoperand is StringNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            var firstoperatorString = ((StringNode)firstoperand).Value;
                            var secondperatorString = ((StringNode)secondoperand).Value;
                            var result = Like(firstoperatorString, secondperatorString);
                            booleanNode.Value = result;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is StringNode && firstoperand is IntegerNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            var firstoperatorString = ((IntegerNode)firstoperand).Value.ToString();
                            var secondperatorString = ((StringNode)secondoperand).Value;
                            var result = Like(firstoperatorString, secondperatorString);
                            booleanNode.Value = result;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is StringNode && firstoperand is DoubleNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            var firstoperatorString = ((DoubleNode)firstoperand).Value.ToString();
                            var secondperatorString = ((StringNode)secondoperand).Value;
                            var result = Like(firstoperatorString, secondperatorString);
                            booleanNode.Value = result;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is StringNode && firstoperand is DateTimeNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            var firstoperatorString = ((DateTimeNode)firstoperand).Value.ToString();
                            var secondperatorString = ((StringNode)secondoperand).Value;
                            var result = Like(firstoperatorString, secondperatorString);
                            booleanNode.Value = result;
                            localValueStack.Push(booleanNode);
                        }
                    }
                    else if (item is EqualNode)
                    {
                        var secondoperand = localValueStack.Pop();
                        var firstoperand = localValueStack.Pop();
                        if (secondoperand is IntegerNode && firstoperand is IntegerNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((IntegerNode)firstoperand).Value == ((IntegerNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DoubleNode && firstoperand is DoubleNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((DoubleNode)firstoperand).Value == ((DoubleNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is BooleanNode && firstoperand is BooleanNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((BooleanNode)firstoperand).Value == ((BooleanNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is StringNode && firstoperand is StringNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((StringNode)firstoperand).Value == ((StringNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DateTimeNode && firstoperand is DateTimeNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((DateTimeNode)firstoperand).Value == ((DateTimeNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is IntegerNode && firstoperand is DoubleNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((DoubleNode)firstoperand).Value == ((IntegerNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DoubleNode && firstoperand is IntegerNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((IntegerNode)firstoperand).Value == ((DoubleNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DateTimeNode && firstoperand is DateTimeNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((DateTimeNode)firstoperand).Value == ((DateTimeNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DateTimeNode && firstoperand is StringNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((StringNode)firstoperand).Value == ((DateTimeNode)secondoperand).Value.ToString();
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is StringNode && firstoperand is DateTimeNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((DateTimeNode)firstoperand).Value.ToString() == ((StringNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                    }
                    else if (item is UnEqualNode)
                    {
                        var secondoperand = localValueStack.Pop();
                        var firstoperand = localValueStack.Pop();
                        if (secondoperand is IntegerNode && firstoperand is IntegerNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((IntegerNode)firstoperand).Value != ((IntegerNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DoubleNode && firstoperand is DoubleNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((DoubleNode)firstoperand).Value != ((DoubleNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is BooleanNode && firstoperand is BooleanNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((BooleanNode)firstoperand).Value != ((BooleanNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is StringNode && firstoperand is StringNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((StringNode)firstoperand).Value != ((StringNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DateTimeNode && firstoperand is DateTimeNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((DateTimeNode)firstoperand).Value != ((DateTimeNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is IntegerNode && firstoperand is DoubleNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((DoubleNode)firstoperand).Value != ((IntegerNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DoubleNode && firstoperand is IntegerNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((IntegerNode)firstoperand).Value != ((DoubleNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DateTimeNode && firstoperand is DateTimeNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((DateTimeNode)firstoperand).Value != ((DateTimeNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DateTimeNode && firstoperand is StringNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((StringNode)firstoperand).Value != ((DateTimeNode)secondoperand).Value.ToString();
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is StringNode && firstoperand is DateTimeNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((DateTimeNode)firstoperand).Value.ToString() != ((StringNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                    }
                    else if (item is SmallerThenNode)
                    {
                        var secondoperand = localValueStack.Pop();
                        var firstoperand = localValueStack.Pop();
                        if (secondoperand is IntegerNode && firstoperand is IntegerNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((IntegerNode)firstoperand).Value < ((IntegerNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DoubleNode && firstoperand is DoubleNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((DoubleNode)firstoperand).Value < ((DoubleNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is IntegerNode && firstoperand is DoubleNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((DoubleNode)firstoperand).Value < ((IntegerNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DoubleNode && firstoperand is IntegerNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((IntegerNode)firstoperand).Value < ((DoubleNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DateTimeNode && firstoperand is DateTimeNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((DateTimeNode)firstoperand).Value < ((DateTimeNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DateTimeNode && firstoperand is StringNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            var str = ((StringNode)firstoperand).Value;
                            booleanNode.Value = DateTime.Parse(str) < ((DateTimeNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is StringNode && firstoperand is DateTimeNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            var str = ((StringNode)secondoperand).Value;
                            booleanNode.Value = ((DateTimeNode)firstoperand).Value < DateTime.Parse(str);
                            localValueStack.Push(booleanNode);
                        }
                    }
                    else if (item is SmallerThenOrEqualNode)
                    {
                        var secondoperand = localValueStack.Pop();
                        var firstoperand = localValueStack.Pop();
                        if (secondoperand is IntegerNode && firstoperand is IntegerNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((IntegerNode)firstoperand).Value <= ((IntegerNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DoubleNode && firstoperand is DoubleNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((DoubleNode)firstoperand).Value <= ((DoubleNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is IntegerNode && firstoperand is DoubleNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((DoubleNode)firstoperand).Value <= ((IntegerNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DoubleNode && firstoperand is IntegerNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((IntegerNode)firstoperand).Value <= ((DoubleNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DateTimeNode && firstoperand is DateTimeNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((DateTimeNode)firstoperand).Value <= ((DateTimeNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DateTimeNode && firstoperand is StringNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            var str = ((StringNode)firstoperand).Value;
                            booleanNode.Value = DateTime.Parse(str) <= ((DateTimeNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is StringNode && firstoperand is DateTimeNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            var str = ((StringNode)secondoperand).Value;
                            booleanNode.Value = ((DateTimeNode)firstoperand).Value <= DateTime.Parse(str);
                            localValueStack.Push(booleanNode);
                        }
                    }
                    else if (item is GreaterThenNode)
                    {
                        var secondoperand = localValueStack.Pop();
                        var firstoperand = localValueStack.Pop();
                        if (secondoperand is IntegerNode && firstoperand is IntegerNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((IntegerNode)firstoperand).Value > ((IntegerNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DoubleNode && firstoperand is DoubleNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((DoubleNode)firstoperand).Value > ((DoubleNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is IntegerNode && firstoperand is DoubleNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((DoubleNode)firstoperand).Value > ((IntegerNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DoubleNode && firstoperand is IntegerNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((IntegerNode)firstoperand).Value > ((DoubleNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DateTimeNode && firstoperand is DateTimeNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((DateTimeNode)firstoperand).Value > ((DateTimeNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DateTimeNode && firstoperand is StringNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            var str = ((StringNode)firstoperand).Value;
                            booleanNode.Value = DateTime.Parse(str) > ((DateTimeNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is StringNode && firstoperand is DateTimeNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            var str = ((StringNode)secondoperand).Value;
                            booleanNode.Value = ((DateTimeNode)firstoperand).Value > DateTime.Parse(str);
                            localValueStack.Push(booleanNode);
                        }
                    }
                    else if (item is GreaterThenOrEqualNode)
                    {
                        var secondoperand = localValueStack.Pop();
                        var firstoperand = localValueStack.Pop();
                        if (secondoperand is IntegerNode && firstoperand is IntegerNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((IntegerNode)firstoperand).Value >= ((IntegerNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DoubleNode && firstoperand is DoubleNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((DoubleNode)firstoperand).Value >= ((DoubleNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is IntegerNode && firstoperand is DoubleNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((DoubleNode)firstoperand).Value >= ((IntegerNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DoubleNode && firstoperand is IntegerNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((IntegerNode)firstoperand).Value >= ((DoubleNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DateTimeNode && firstoperand is DateTimeNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((DateTimeNode)firstoperand).Value >= ((DateTimeNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is DateTimeNode && firstoperand is StringNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            var str = ((StringNode)firstoperand).Value;
                            booleanNode.Value = DateTime.Parse(str) >= ((DateTimeNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                        else if (secondoperand is StringNode && firstoperand is DateTimeNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            var str = ((StringNode)secondoperand).Value;
                            booleanNode.Value = ((DateTimeNode)firstoperand).Value >= DateTime.Parse(str);
                            localValueStack.Push(booleanNode);
                        }
                    }
                    else if (item is IsNode)
                    {
                        var secondoperand = localValueStack.Pop();
                        var firstoperand = localValueStack.Pop();
                        if (secondoperand is NullNode && firstoperand is NullNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = true;
                            localValueStack.Push(booleanNode);
                        }
                        else
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = false;
                            localValueStack.Push(booleanNode);
                        }
                    }
                    else if (item is OrNode)
                    {
                        var secondoperand = localValueStack.Pop();
                        var firstoperand = localValueStack.Pop();
                        if (secondoperand is BooleanNode && firstoperand is BooleanNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((BooleanNode)firstoperand).Value || ((BooleanNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                    }
                    else if (item is AndNode)
                    {
                        var secondoperand = localValueStack.Pop();
                        var firstoperand = localValueStack.Pop();
                        if (secondoperand is BooleanNode && firstoperand is BooleanNode)
                        {
                            BooleanNode booleanNode = new BooleanNode();
                            booleanNode.Value = ((BooleanNode)firstoperand).Value && ((BooleanNode)secondoperand).Value;
                            localValueStack.Push(booleanNode);
                        }
                    }
                    setNameValue++;
                }
                var lastValue = false;
                var reveredStack = new Stack<AbstractSyntaxTreeNode>();
                while (localValueStack.Count > 0)
                {
                    reveredStack.Push(localValueStack.Pop());
                }
                var ebene = reveredStack.Count - 1;
                while (reveredStack.Count > 0)
                {
                    var endValue = reveredStack.Pop();
                    if (endValue is BooleanNode)
                    {
                        if (((BooleanNode)endValue).Value)
                        {
                            if (setList.Count > 0)
                            {
                                foreach (var item in setList)
                                {
                                    var propertyName = item.Key;
                                    var objectToSet = item.Value;
                                    var objectCastValue = new object();
                                    if (objectToSet is IntegerNode)
                                    {
                                        objectCastValue = ((IntegerNode)objectToSet).Value;
                                    }
                                    else if (objectToSet is DoubleNode)
                                    {
                                        objectCastValue = ((DoubleNode)objectToSet).Value;
                                    }
                                    else if (objectToSet is BooleanNode)
                                    {
                                        objectCastValue = ((BooleanNode)objectToSet).Value;
                                    }
                                    else if (objectToSet is StringNode)
                                    {
                                        objectCastValue = ((StringNode)objectToSet).Value;
                                    }
                                    else if (objectToSet is DateTimeNode)
                                    {
                                        objectCastValue = ((DateTimeNode)objectToSet).Value;
                                    }
                                    var property = objectValue.GetType().GetProperty(propertyName);
                                    if (property != null)
                                        property.SetValue(objectValue, objectCastValue, null);
                                    else
                                    {
                                        var method = objectValue.GetType().GetMethod(propertyName);
                                        if (method != null)
                                        {
                                            method.Invoke(objectValue, new object[] { objectCastValue });
                                        }
                                    }
                                }
                            }
                        }
                        if (((BooleanNode)endValue).Value)
                        {
                            if (ebene < thenList.Count)
                            {
                                var item = thenList.ToList()[ebene];
                                MethodCallNodeExecuteCall(objectValue, item.Value.First().Item2);
                                lastValue = ((BooleanNode)endValue).Value;
                                break;
                            }
                        }
                        else
                        {
                            if (ebene < elseList.Count)
                            {
                                var item = elseList.ToList()[ebene];
                                MethodCallNodeExecuteCall(objectValue, item.Value.First().Item2);
                                lastValue = ((BooleanNode)endValue).Value;
                                break;
                            }
                        }
                        lastValue = ((BooleanNode)endValue).Value;
                    }
                    ebene--;
                }
                if (lastValue)
                    fieldList.Add(field);
            }
            objectValue.Fields.Clear();
            foreach (var field in fieldList)
            {
                objectValue.Fields.Add(field);
            }
            return objectValue;
        }
    }
}
