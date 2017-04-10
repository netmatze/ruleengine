using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuleEngine;
using SimpleExpressionEvaluator;

namespace RuleEngineTests
{
    [TestClass]
    public class DynamicBaseClassTests
    {
        [TestMethod]
        public void DynamicBaseClassSimpleTest()
        {
            DynamicBaseClass person = new DynamicBaseClass()
            { Name = "Person", ReferenceName = "person", Type = ObjectType.Object };
            person.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 25 });

            DynamicBaseClass collection = new DynamicBaseClass()
            { Name = "PersonCollection", ReferenceName = "personcollection", Type = ObjectType.Object };
            collection.Fields.Add(new { Total = 100, AverageAge = 99, Min = 1, Max = 100 });

            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(
                " (person[Name].lower = 'joe' && " +
                "person[Age] <= personcollection[AverageAge]) ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(
                new DynamicBaseClass[] { person, collection },
                new Rule[] { rule1 });
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicBaseClassComplexTest()
        {
            DynamicBaseClass person = new DynamicBaseClass()
                { Name = "Person", ReferenceName = "person", Type = ObjectType.Object };
            person.Fields.Add(new { Name = "Joe", Country="Ireland", Age=25 });

            DynamicBaseClass collection = new DynamicBaseClass()
                { Name = "CollectionPerson", ReferenceName = "collectionperson", Type = ObjectType.Object };
            collection.Fields.Add(new { Total = 100, AverageAge = 99, Min = 1, Max = 100 });

            DynamicBaseClass averagePerson = new DynamicBaseClass()
                { Name = "AveragePerson", ReferenceName = "averageperson", Type = ObjectType.Object };
            averagePerson.Fields.Add(new { AverageAge = 30, StandardDeviation = 7 });

            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(
                " (person[Name] = 'Joei' && " +
                "person[Age] <= averageperson[AverageAge]) " +
                "|| (person[Age] > collectionperson[Total]) " +
                "|| collectionperson[Total] = 100 ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(
                new DynamicBaseClass[] { person, collection, averagePerson },
                new Rule[] { rule1 });
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicBaseClassFilterCountTest()
        {
            DynamicBaseClass collection = new DynamicBaseClass()
            { Name = "CollectionPerson", ReferenceName = "collectionperson", Type = ObjectType.Collection };
            collection.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 25 });
            collection.Fields.Add(new { Name = "Bert", Country = "USA", Age = 60 });
            collection.Fields.Add(new { Name = "Carl", Country = "USA", Age = 30 });
            collection.Fields.Add(new { Name = "Eric", Country = "Sweden", Age = 30 });

            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" collectionperson.Filter[Age > 30].Count >= 1 ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(
                new DynamicBaseClass[] { collection },
                new Rule[] { rule1 });
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicBaseClassFilterSumTest()
        {
            DynamicBaseClass collection = new DynamicBaseClass()
            { Name = "CollectionPerson", ReferenceName = "collectionperson", Type = ObjectType.Collection };
            collection.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 25, Children = 1 });
            collection.Fields.Add(new { Name = "Bert", Country = "USA", Age = 60, Children = 2 });
            collection.Fields.Add(new { Name = "Carl", Country = "USA", Age = 30, Children = 0 });
            collection.Fields.Add(new { Name = "Eric", Country = "Sweden", Age = 30, Children = 0 });

            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" collectionperson[Children].Filter[Age > 30].Sum > 1 ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(
                new DynamicBaseClass[] { collection },
                new Rule[] { rule1 });
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicBaseClassFilterInTest()
        {
            DynamicBaseClass person = new DynamicBaseClass()
            { Name = "Person", ReferenceName = "person", Type = ObjectType.Object };
            person.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 25 });

            DynamicBaseClass collection = new DynamicBaseClass()
            { Name = "CollectionPerson", ReferenceName = "collectionperson", Type = ObjectType.Collection };
            collection.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 25 });
            collection.Fields.Add(new { Name = "Bert", Country = "USA", Age = 60 });
            collection.Fields.Add(new { Name = "Carl", Country = "USA", Age = 30 });
            collection.Fields.Add(new { Name = "Eric", Country = "Sweden", Age = 30 });

            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" person[Name] in collectionperson[Name].Filter[Age < 30] ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(
                new DynamicBaseClass[] { person, collection },
                new Rule[] { rule1 });
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicBaseClassFilterNotInTest()
        {
            DynamicBaseClass person = new DynamicBaseClass()
            { Name = "Person", ReferenceName = "person", Type = ObjectType.Object };
            person.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 25 });

            DynamicBaseClass collection = new DynamicBaseClass()
            { Name = "CollectionPerson", ReferenceName = "collectionperson", Type = ObjectType.Collection };
            collection.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 25 });
            collection.Fields.Add(new { Name = "Bert", Country = "USA", Age = 60 });
            collection.Fields.Add(new { Name = "Carl", Country = "USA", Age = 30 });
            collection.Fields.Add(new { Name = "Eric", Country = "Sweden", Age = 30 });

            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" person[Country] notin collectionperson[Country].Filter[Age > 30] ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(
                new DynamicBaseClass[] { person, collection },
                new Rule[] { rule1 });
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicBaseClassCollectionAverageTest()
        {
            DynamicBaseClass person = new DynamicBaseClass()
            { Name = "Person", ReferenceName = "person", Type = ObjectType.Object };
            person.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 25 });

            DynamicBaseClass collection = new DynamicBaseClass()
            { Name = "CollectionPerson", ReferenceName = "collectionperson", Type = ObjectType.Collection };
            collection.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 25 });
            collection.Fields.Add(new { Name = "Bert", Country = "USA", Age = 60 });
            collection.Fields.Add(new { Name = "Carl", Country = "USA", Age = 30 });

            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" person[Age] >= collectionperson[Age].Average || person[Name] = 'Joe' ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(
                new DynamicBaseClass[] { person, collection },
                new Rule[] { rule1 });
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicBaseClassCollectionSumTest()
        {
            DynamicBaseClass person = new DynamicBaseClass()
            { Name = "Person", ReferenceName = "person", Type = ObjectType.Object };
            person.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 25 });

            DynamicBaseClass collection = new DynamicBaseClass()
            { Name = "CollectionPerson", ReferenceName = "collectionperson", Type = ObjectType.Collection };
            collection.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 5 });
            collection.Fields.Add(new { Name = "Bert", Country = "USA", Age = 5 });
            collection.Fields.Add(new { Name = "Carl", Country = "USA", Age = 5 });
            collection.Fields.Add(new { Name = "Carol", Country = "Germany", Age = 5 });

            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" person[Age] >= collectionperson[Age].Sum ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(
                new DynamicBaseClass[] { person, collection },
                new Rule[] { rule1 });
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicBaseClassCollectionCountTest()
        {
            DynamicBaseClass person = new DynamicBaseClass()
            { Name = "Person", ReferenceName = "person", Type = ObjectType.Object };
            person.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 5 });

            DynamicBaseClass collection = new DynamicBaseClass()
            { Name = "CollectionPerson", ReferenceName = "collectionperson", Type = ObjectType.Collection };
            collection.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 5 });
            collection.Fields.Add(new { Name = "Bert", Country = "USA", Age = 5 });
            collection.Fields.Add(new { Name = "Carl", Country = "USA", Age = 5 });
            collection.Fields.Add(new { Name = "Carol", Country = "Germany", Age = 5 });

            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" person[Age] >= collectionperson[Age].Count ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(
                new DynamicBaseClass[] { person, collection },
                new Rule[] { rule1 });
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicBaseClassCollectionMaxTest()
        {
            DynamicBaseClass person = new DynamicBaseClass()
            { Name = "Person", ReferenceName = "person", Type = ObjectType.Object };
            person.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 25 });

            DynamicBaseClass collection = new DynamicBaseClass()
            { Name = "CollectionPerson", ReferenceName = "collectionperson", Type = ObjectType.Collection };
            collection.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 20 });
            collection.Fields.Add(new { Name = "Bert", Country = "USA", Age = 21 });
            collection.Fields.Add(new { Name = "Carl", Country = "USA", Age = 22 });
            collection.Fields.Add(new { Name = "Carol", Country = "Germany", Age = 23 });

            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" person[Age] >= collectionperson[Age].Max ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(
                new DynamicBaseClass[] { person, collection },
                new Rule[] { rule1 });
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicBaseClassCollectionMinTest()
        {
            DynamicBaseClass person = new DynamicBaseClass()
            { Name = "Person", ReferenceName = "person", Type = ObjectType.Object };
            person.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 25 });

            DynamicBaseClass collection = new DynamicBaseClass()
            { Name = "CollectionPerson", ReferenceName = "collectionperson", Type = ObjectType.Collection };
            collection.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 20 });
            collection.Fields.Add(new { Name = "Bert", Country = "USA", Age = 40 });
            collection.Fields.Add(new { Name = "Carl", Country = "USA", Age = 50 });
            collection.Fields.Add(new { Name = "Carol", Country = "Germany", Age = 60 });

            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" person[Age] >= collectionperson[Age].Min ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(
                new DynamicBaseClass[] { person, collection },
                new Rule[] { rule1 });
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicBaseClassCollectionInTest()
        {
            DynamicBaseClass person = new DynamicBaseClass()
            { Name = "Person", ReferenceName = "person", Type = ObjectType.Object };
            person.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 25 });

            DynamicBaseClass collection = new DynamicBaseClass()
            { Name = "CollectionPerson", ReferenceName = "collectionperson", Type = ObjectType.Collection };
            collection.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 20 });
            collection.Fields.Add(new { Name = "Bert", Country = "USA", Age = 40 });
            collection.Fields.Add(new { Name = "Carl", Country = "USA", Age = 50 });
            collection.Fields.Add(new { Name = "Carol", Country = "Germany", Age = 60 });

            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" person[Name] in collectionperson[Name] ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(
                new DynamicBaseClass[] { person, collection },
                new Rule[] { rule1 });
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicBaseClassCollectionNotInTest()
        {
            DynamicBaseClass person = new DynamicBaseClass()
            { Name = "Person", ReferenceName = "person", Type = ObjectType.Object };
            person.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 25 });

            DynamicBaseClass collection = new DynamicBaseClass()
            { Name = "CollectionPerson", ReferenceName = "collectionperson", Type = ObjectType.Collection };
            collection.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 20 });
            collection.Fields.Add(new { Name = "Bert", Country = "USA", Age = 40 });
            collection.Fields.Add(new { Name = "Carl", Country = "USA", Age = 50 });
            collection.Fields.Add(new { Name = "Carol", Country = "Germany", Age = 60 });

            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" person[Age] notin collectionperson[Age] ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(
                new DynamicBaseClass[] { person, collection },
                new Rule[] { rule1 });
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicBaseClassTrimTest()
        {
            DynamicBaseClass person = new DynamicBaseClass()
            { Name = "Person", ReferenceName = "person", Type = ObjectType.Object };
            person.Fields.Add(new { Name = " Joe ", Country = "Ireland", Age = 25 });
        
            Rule rule1 = new Rule(
                " person[Name].trim = 'Joe' ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(
                new DynamicBaseClass[] { person },
                new Rule[] { rule1 });
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicBaseClassTrimAndLowerTest()
        {
            DynamicBaseClass person = new DynamicBaseClass()
            { Name = "Person", ReferenceName = "person", Type = ObjectType.Object };
            person.Fields.Add(new { Name = " Joe ", Country = "Ireland", Age = 25 });

            Rule rule1 = new Rule(
                " person[Name].trim.lower = 'joe' ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(
                new DynamicBaseClass[] { person },
                new Rule[] { rule1 });
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicBaseClassLeftTrimTest()
        {
            DynamicBaseClass person = new DynamicBaseClass()
            { Name = "Person", ReferenceName = "person", Type = ObjectType.Object };
            person.Fields.Add(new { Name = " Joe", Country = "Ireland", Age = 25 });

            Rule rule1 = new Rule(
                " person[Name].lefttrim = 'Joe' ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(
                new DynamicBaseClass[] { person },
                new Rule[] { rule1 });
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicBaseClassLeftTrimAndUpperTest()
        {
            DynamicBaseClass person = new DynamicBaseClass()
            { Name = "Person", ReferenceName = "person", Type = ObjectType.Object };
            person.Fields.Add(new { Name = " Joe", Country = "Ireland", Age = 25 });

            Rule rule1 = new Rule(
                " person[Name].lefttrim.upper = 'JOE' ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(
                new DynamicBaseClass[] { person },
                new Rule[] { rule1 });
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicBaseClassRightTrimTest()
        {
            DynamicBaseClass person = new DynamicBaseClass()
            { Name = "Person", ReferenceName = "person", Type = ObjectType.Object };
            person.Fields.Add(new { Name = "Joe ", Country = "Ireland", Age = 25 });

            Rule rule1 = new Rule(
                " person[Name].righttrim = 'Joe' ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(
                new DynamicBaseClass[] { person },
                new Rule[] { rule1 });
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicBaseClassLowerTest()
        {
            DynamicBaseClass person = new DynamicBaseClass()
            { Name = "Person", ReferenceName = "person", Type = ObjectType.Object };
            person.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 25 });

            Rule rule1 = new Rule(
                " person[Name].lower = 'joe' ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(
                new DynamicBaseClass[] { person },
                new Rule[] { rule1 });
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicBaseClassUpperTest()
        {
            DynamicBaseClass person = new DynamicBaseClass()
            { Name = "Person", ReferenceName = "person", Type = ObjectType.Object };
            person.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 25 });

            Rule rule1 = new Rule(
                " person[Name].upper = 'JOE' ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(
                new DynamicBaseClass[] { person },
                new Rule[] { rule1 });
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicBaseClassLengthTest()
        {
            DynamicBaseClass person = new DynamicBaseClass()
            { Name = "Person", ReferenceName = "person", Type = ObjectType.Object };
            person.Fields.Add(new { Name = "Joe", Country = "Ireland", Age = 25 });

            Rule rule1 = new Rule(
                " person[Name].length = 3 ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(
                new DynamicBaseClass[] { person },
                new Rule[] { rule1 });
            Assert.AreEqual(result, true);
        }
    }
}
