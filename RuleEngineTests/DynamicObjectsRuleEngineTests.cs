using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuleEngine;
using SimpleExpressionEvaluator;

namespace RuleEngineTests
{
    [TestClass]
    public class DynamicObjectsRuleEngineTest
    {
        [TestMethod]
        public void DynamicObjectTest()
        {
            dynamic person1 = new { Name = "Mathias", Age = 35, Children = 2 };
            dynamic person2 = new { Name = "Anna", Age = 32, Children = 2 };
            dynamic pet1 = new { Name = "lucky", Age = 5, Type = "cat" };
            dynamic pet2 = new { Name = "wuffy", Age = 5, Type = "dog" };
            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = expressionRuleLoader.Load(1);
            Rule rule2 = expressionRuleLoader.Load(2);
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(new dynamic[] { person1, person2, pet1, pet2 },
                new Rule[] { rule1, rule2 });
            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void DynamicObjectTestTest()
        {
            dynamic person1 = new { Name = "Mathias", Age = 35, Children = 2 };
            dynamic person2 = new { Name = "Anna", Age = 32, Children = 2 };
            dynamic pet1 = new { Name = "lucky", Age = 5, Type = "cat" };
            dynamic pet2 = new { Name = "wuffy", Age = 5, Type = "dog" };
            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" Name = 'wuffy' ");
            Rule rule2 = new Rule(" Age = 5 ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAny(
                new dynamic[] { person1, person2, pet1, pet2 },
                new Rule[] { rule1, rule2 });
            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void DynamicObjectFieldTest()
        {
            dynamic person1 = new { Name = "Mathias", Age = 35, Children = 2 };           
            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" Field[Name] = 'Mathias' ");
            RuleValidator ruleValidator = new RuleValidator();
            bool result = ruleValidator.ValidateExpressionRulesDynamic(person1,
                 rule1);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicObjectCollectionInTest()
        {
            dynamic person1 = new { Name = "Mathias", Age = 35, Children = 2 };
            dynamic person2 = new { Name = "Mathias", Age = 32, Children = 2 };
            dynamic pet1 = new { Name = "lucky", Age = 5, Type = "cat" };
            dynamic pet2 = new { Name = "wuffy", Age = 5, Type = "dog" };
            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" Field[Name] in Collection[Name] ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRules(person1,
                new dynamic[] { person2, pet1, pet2 }, rule1 );
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicObjectCollectionNotInTest()
        {
            dynamic person1 = new { Name = "Mathias", Age = 35, Children = 2 };
            dynamic person2 = new { Name = "Anna", Age = 32, Children = 2 };
            dynamic pet1 = new { Name = "lucky", Age = 5, Type = "cat" };
            dynamic pet2 = new { Name = "wuffy", Age = 5, Type = "dog" };
            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" Field[Name] notin Collection[Name] ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRules(person1,
                new dynamic[] { person2, pet1, pet2 }, rule1);
            Assert.AreEqual(result, true);
        }       

        [TestMethod]
        public void DynamicObjectCollectionAverageTest()
        {
            dynamic person1 = new { Name = "Mathias", Age = 35, Children = 2 };
            dynamic person2 = new { Name = "Anna", Age = 32, Children = 2 };
            dynamic pet1 = new { Name = "lucky", Age = 5, Type = "cat" };
            dynamic pet2 = new { Name = "wuffy", Age = 5, Type = "dog" };
            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();           
            Rule rule1 = new Rule(" Field[Age] > Collection[Age].Average ");           
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRules(person1,
                new dynamic[] { person2, pet1, pet2 }, rule1);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicObjectCollectionSumTest()
        {
            dynamic person1 = new { Name = "Mathias", Age = 35, Children = 2 };
            dynamic person2 = new { Name = "Anna", Age = 32, Children = 2 };
            dynamic pet1 = new { Name = "lucky", Age = 5, Type = "cat" };
            dynamic pet2 = new { Name = "wuffy", Age = 5, Type = "dog" };
            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" Field[Age] > Collection[Age].Sum ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRules(person1,
                new dynamic[] { person2, pet1, pet2 }, rule1);
            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void DynamicObjectCollectionCountTest()
        {
            dynamic person1 = new { Name = "Mathias", Age = 35, Children = 2 };
            dynamic person2 = new { Name = "Anna", Age = 32, Children = 2 };
            dynamic pet1 = new { Name = "lucky", Age = 5, Type = "cat" };
            dynamic pet2 = new { Name = "wuffy", Age = 5, Type = "dog" };
            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" Field[Age] > Collection[Age].Count ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRules(person1,
                new dynamic[] { person2, pet1, pet2 }, rule1);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicObjectCollectionMinTest()
        {
            dynamic person1 = new { Name = "Mathias", Age = 35, Children = 2 };
            dynamic person2 = new { Name = "Anna", Age = 32, Children = 2 };
            dynamic pet1 = new { Name = "lucky", Age = 5, Type = "cat" };
            dynamic pet2 = new { Name = "wuffy", Age = 5, Type = "dog" };
            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" Field[Age] > Collection[Age].Min ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRules(person1,
                new dynamic[] { person2, pet1, pet2 }, rule1);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicObjectCollectionMaxTest()
        {
            dynamic person1 = new { Name = "Mathias", Age = 35, Children = 2 };
            dynamic person2 = new { Name = "Anna", Age = 32, Children = 2 };
            dynamic pet1 = new { Name = "lucky", Age = 5, Type = "cat" };
            dynamic pet2 = new { Name = "wuffy", Age = 5, Type = "dog" };
            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" Field[Age] > Collection[Age].Max ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRules(person1,
                new dynamic[] { person2, pet1, pet2 }, rule1);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicObjectCollectionInAndConstantTest()
        {
            dynamic person1 = new { Name = "Mathias", Age = 35, Children = 2 };
            dynamic person2 = new { Name = "Mathias", Age = 32, Children = 2 };
            dynamic pet1 = new { Name = "lucky", Age = 5, Type = "cat" };
            dynamic pet2 = new { Name = "wuffy", Age = 5, Type = "dog" };
            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" Field[Age] > 40 && Field[Name] in Collection[Name] ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRules(person1,
                new dynamic[] { person2, pet1, pet2 }, rule1);
            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void DynamicObjectCollectionInAndAverageTest()
        {
            dynamic person1 = new { Name = "Mathias", Age = 35, Children = 2 };
            dynamic person2 = new { Name = "Mathias", Age = 32, Children = 2 };
            dynamic pet1 = new { Name = "lucky", Age = 5, Type = "cat" };
            dynamic pet2 = new { Name = "wuffy", Age = 5, Type = "dog" };
            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" Field[Age] < Collection[Age].Average && Field[Name] in Collection[Name] ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRules(person1,
                new dynamic[] { person2, pet1, pet2 }, rule1);
            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void DynamicObjectCollectionMaxAndAverageTest()
        {
            dynamic person1 = new { Name = "Mathias", Age = 35, Children = 2 };
            dynamic person2 = new { Name = "Mathias", Age = 32, Children = 2 };
            dynamic pet1 = new { Name = "lucky", Age = 5, Type = "cat" };
            dynamic pet2 = new { Name = "wuffy", Age = 5, Type = "dog" };
            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" Field[Age] > Collection[Age].Average && Field[Children] > Collection[Children].Max ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRules(person1,
                new dynamic[] { person2, pet1, pet2 }, rule1);
            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void DynamicObjectCollectionMaxAndAverageOrInTest()
        {
            dynamic person1 = new { Name = "Mathias", Age = 35, Children = 2 };
            dynamic person2 = new { Name = "Mathias", Age = 32, Children = 2 };
            dynamic pet1 = new { Name = "lucky", Age = 5, Type = "cat" };
            dynamic pet2 = new { Name = "wuffy", Age = 5, Type = "dog" };
            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            Rule rule1 = new Rule(" Field[Age] > Collection[Age].Average && Field[Children] > Collection[Children].Max || Field[Name] in Collection[Name] ");
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRules(person1,
                new dynamic[] { person2, pet1, pet2 }, rule1);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void DynamicSimpleExpressionEvaluateNonBoolean()
        {
            dynamic person1 = new
            {
                Name = "mathias",
                ColorComplexion = 5,
                Height = 10,
                Event = "Wedding"
            };
            Evaluator evaluator = new Evaluator();
            var resultNonBoolean = evaluator.EvaluateDynamic(
                " (Field[Name] = 'mathias' && Height >= 100) || ColorComplexion = 4", person1); //,'Beautiful Dress'            
            Assert.AreEqual(resultNonBoolean, false);
        }

        [TestMethod]
        public void DynamicSimpleExpressionEvaluateExpressionNonBoolean()
        {
            dynamic person1 = new
            {
                Name = "mathias",
                ColorComplexion = 5,
                Height = 10,
                Event = "Wedding"
            };
            Evaluator evaluator = new Evaluator();
            var resultNonBoolean = evaluator.EvaluateNonBooleanDynamic<double>(
                " Field[Height] * 2 ", person1); //,'Beautiful Dress'            
            Assert.AreEqual(resultNonBoolean, 20);
        }

        [TestMethod]
        public void DynamicSimpleExpressionEvaluateFilterCollectionCount()
        {
            dynamic person1 = new { Name = "Mathias", Age = 35, Children = 2 };
            dynamic person2 = new { Name = "Mathias", Age = 32, Children = 2 };
            dynamic pet1 = new { Name = "lucky", Age = 5, Type = "cat" };
            dynamic pet2 = new { Name = "wuffy", Age = 5, Type = "dog" };
            Evaluator evaluator = new Evaluator();
            var resultNonBoolean = evaluator.EvaluateNonBooleanDynamic<double>(
                " Collection.Filter[Age > 10].Count ", new dynamic[] { person1, person2, pet1, pet2 }); //,'Beautiful Dress'            
            Assert.AreEqual(resultNonBoolean, 2);
        }

        [TestMethod]
        public void DynamicSimpleExpressionEvaluateFilterCollectionAverage()
        {
            dynamic person1 = new { Name = "Mathias", Age = 35, Children = 2 };
            dynamic person2 = new { Name = "Mathias", Age = 32, Children = 2 };
            dynamic pet1 = new { Name = "lucky", Age = 5, Type = "cat" };
            dynamic pet2 = new { Name = "wuffy", Age = 5, Type = "dog" };
            Evaluator evaluator = new Evaluator();
            var resultNonBoolean = evaluator.EvaluateNonBooleanDynamic<double>(
                " Collection[Age].Filter[Age > 10].Average ", new dynamic[] { person1, person2, pet1, pet2 }); //,'Beautiful Dress'            
            Assert.AreEqual(resultNonBoolean, 33.5);
        }

        [TestMethod]
        public void DynamicSimpleExpressionEvaluateFilterCollectionSum()
        {
            dynamic person1 = new { Name = "Mathias", Age = 35, Children = 2 };
            dynamic person2 = new { Name = "Mathias", Age = 32, Children = 2 };
            dynamic pet1 = new { Name = "lucky", Age = 5, Type = "cat" };
            dynamic pet2 = new { Name = "wuffy", Age = 5, Type = "dog" };
            Evaluator evaluator = new Evaluator();
            var resultNonBoolean = evaluator.EvaluateNonBooleanDynamic<double>(
                " Collection[Children].Filter[Name = 'Mathias'].Sum ", new dynamic[] { person1, person2, pet1, pet2 }); //,'Beautiful Dress'            
            Assert.AreEqual(resultNonBoolean, 4);
        }

        [TestMethod]
        public void DynamicSimpleExpressionEvaluateFilterCollectionMin()
        {
            dynamic person1 = new { Name = "Mathias", Age = 35, Children = 2 };
            dynamic person2 = new { Name = "Mathias", Age = 32, Children = 2 };
            dynamic pet1 = new { Name = "lucky", Age = 5, Type = "cat" };
            dynamic pet2 = new { Name = "wuffy", Age = 5, Type = "dog" };
            Evaluator evaluator = new Evaluator();
            var resultNonBoolean = evaluator.EvaluateNonBooleanDynamic<double>(
                " Collection[Age].Filter[Name = 'Mathias'].Min ", new dynamic[] { person1, person2, pet1, pet2 }); //,'Beautiful Dress'            
            Assert.AreEqual(resultNonBoolean, 32);
        }

        [TestMethod]
        public void DynamicSimpleExpressionEvaluateFilterCollectionMax()
        {
            dynamic person1 = new { Name = "Mathias", Age = 35, Children = 2 };
            dynamic person2 = new { Name = "Mathias", Age = 32, Children = 2 };
            dynamic pet1 = new { Name = "lucky", Age = 5, Type = "cat" };
            dynamic pet2 = new { Name = "wuffy", Age = 8, Type = "dog" };
            Evaluator evaluator = new Evaluator();
            var resultNonBoolean = evaluator.EvaluateNonBooleanDynamic<double>(
                " Collection[Age].Filter[Name = 'lucky' || Name = 'wuffy'].Max ", new dynamic[] { person1, person2, pet1, pet2 }); //,'Beautiful Dress'            
            Assert.AreEqual(resultNonBoolean, 8);
        }
    }
}
