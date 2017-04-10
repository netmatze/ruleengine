using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RuleEngine;
using SimpleExpressionEvaluator;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace RuleEngineTests
{
    [TestClass]
    public class RuleEngineTests
    {
        [TestMethod]
        public void SimpleRuleLoaderPersonTest()
        {
            Person person = new Person()
                { Name = "Mathias", Age = 36, Children = 2, Married = true };
            RuleLoader ruleLoader = new RuleLoader();
            // new Rule("Age", Operator.LessThanOrEqual, 50);
            Rule rule = ruleLoader.Load(2);
            RuleEngine.RuleEngine ruleEngine = new RuleEngine.RuleEngine();
            var ruleFunc = ruleEngine.CompileRule<Person>(rule);
            var result = ruleFunc(person);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void ComplicatedTest()
        {
           Person person2 = new Person()
            {
                Name = "Anna",
                Age = 32,
                Children = 2,
                Married = false
            };
            Dictionary<string, object> result = new Dictionary<string, object>
            {
                {"p1",3},
                {"p2","ee"},
                {"p3",2}
            };
            Dictionary<string, Type> dicPro = new Dictionary<string, Type>();
            Dictionary<string, string> dicValue = new Dictionary<string, string>();
            foreach (var o in result)
            {
                dicPro.Add(o.Key, o.Value.GetType());
                dicValue.Add(o.Key, o.Value.ToString());
            }
            var dashboardObject = RuleDLL.MetricDashboardType.TypeCreator.CreateObject(dicPro, dicValue);
            Evaluator evaluator = new Evaluator();
            var method = evaluator.GetType().GetMethod("Evaluate").MakeGenericMethod(dashboardObject.GetType());
            var evaluatorResult1 = method.Invoke(evaluator, new object[] { "p1 = 3", dashboardObject });
            Assert.AreEqual(evaluatorResult1, true);
            bool evaluatorResultAge = evaluator.Evaluate("Age = 32", person2);
            Assert.AreEqual(evaluatorResultAge, true);
            bool evaluatorResultName = evaluator.Evaluate("Name = 'Anna'", person2);
            Assert.AreEqual(evaluatorResultName, true);
            bool evaluatorResultChildren = evaluator.Evaluate("Children = 3", person2);
            Assert.AreEqual(evaluatorResultChildren, false);
            bool evaluatorResultMarried = evaluator.Evaluate("Married = true", person2);
            Assert.AreEqual(evaluatorResultChildren, false);
        }

        [TestMethod]
        public void SimpleRuleLoaderPersonValidateRulesCount()
        {
            Person person1 = new Person()
            {
                Name = "Mathias",
                Birthdate = new DateTime(1976, 5, 9)
            };
            Person person2 = new Person()
            {
                Name = "Anna",
                Birthdate = new DateTime(2001, 4, 4)
            };            
            Rule rule1 = new Rule("Birthdate", Operator.GreaterThanOrEqual, new DateTime(2000, 1, 1));
            RuleEngine.RuleEngine ruleEngine = new RuleEngine.RuleEngine();            
            var ruleFunc =
                ruleEngine.CompileRule<Person>(rule1);
            RuleValidator ruleValidator = new RuleValidator();
            // calculates the count of persons that match rule1
            var resultSingleRule = ruleValidator.ValidateRuleCount(new[] { person1, person2 }, ruleFunc);
            // one person ("Anna") matches the Birthdate rule
            Assert.AreEqual(resultSingleRule, 1);
            Rule rule2 = new Rule("Name", Operator.Equal, "Anna");
            var combinedRulesFunc =
                ruleEngine.CombineRules<Person>(new Rule[] { rule1, rule2 });
            // calculates the count of persons that match rule1 and rule2
            var resultMultibleRule = ruleValidator.ValidateRulesCount(new[] { person1, person2 }, combinedRulesFunc);
            // one person ("Anna") matches the Birthdate rule and the Name rule
            Assert.AreEqual(resultMultibleRule, 1);
        }

        [TestMethod]
        public void SimpleRuleLoaderPersonValidateRulesAny()
        {
            //Person person1 = new Person() { 
            //    Name = "Mathias", Birthdate = new DateTime(1976,5,9) };
            //Person person2 = new Person() { 
            //    Name = "Anna", Birthdate = new DateTime(2001,4,4) };
            //RuleLoader ruleLoader = new RuleLoader();
            //// new Rule("Age", Operator.LessThanOrEqual, 50);
            //Rule rule1 = ruleLoader.Load(2);
            //Rule rule2 = ruleLoader.Load(3);
            //// new Rule("Children", Operator.GreaterThan, 0);
            ////Rule rule2 = ruleLoader.Load(3);
            //RuleEngine.RuleEngine ruleEngine = new RuleEngine.RuleEngine();
            //var ruleFuncs = ruleEngine.CombineRules<Person>(
            //    new Rule[] { rule1, rule2 });
            //RuleValidator ruleValidator = new RuleValidator();
            //var result = ruleValidator.ValidateRulesAny(
            //    new[] { person1, person2 }, ruleFuncs);
            //Assert.AreEqual(result, true);

            var rule = new Rule("Age", Operator.LessThanOrEqual, 50);
            Person person = new Person() { Name = "Mathias", Age = 36, Children = 2, Married = true };            
            RuleEngine.RuleEngine ruleEngine = new RuleEngine.RuleEngine();
            var ruleFunc = ruleEngine.CompileRule<Person>(rule);
            var result = ruleFunc(person);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void SimpleRuleLoaderPersonValidateRulesAll()
        {
            Person person1 = new Person() { Name = "Mathias", Age = 36, Children = 2, Married = true };
            Person person2 = new Person() { Name = "Anna", Age = 32, Children = 2, Married = false };
            RuleLoader ruleLoader = new RuleLoader();
            // new Rule("Age", Operator.LessThanOrEqual, 50);
            Rule rule1 = ruleLoader.Load(2);
            // new Rule("Married", Operator.Equal, true);
            Rule rule2 = ruleLoader.Load(8);
            RuleEngine.RuleEngine ruleEngine = new RuleEngine.RuleEngine();
            var ruleFuncs = ruleEngine.CombineRules<Person>(
                new Rule[] { rule1, rule2 });
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateRulesAll(new[] { person1, person2 }, ruleFuncs);
            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void SimpleExpressionEvaluatorRules()
        {
            Person person1 = new Person()
            {
                Name = "Mathias",
                Age = 36,
                Children = 2,
                Married = true,
                Birthdate = new DateTime(1976,5,9)
            };                   
            Person person2 = new Person() 
            { 
                Name = "Anna", 
                Age = 32, 
                Children = 2, 
                Married = false,
                Birthdate = new DateTime(2002,2,2)
            };
            RuleEngine.RuleEngine ruleEngine = new RuleEngine.RuleEngine();
            RuleValidator ruleValidator = new RuleValidator();
            var counter = ruleValidator.ValidateExpressionRulesCount(
                new Person[] { person1, person2 }, new Rule[] { new Rule("Birthdate < '2001-09-05'") });
            Assert.AreEqual(counter, 1);
        }

        [TestMethod]
        public void SimpleExpressionEvaluatorWithSet()
        {
            Person person1 = new Person()
            {
                Name = "Mathias",
                Age = 36,
                Children = 2,
                Married = true,
                Birthdate = new DateTime(1976, 5, 9)
            };
            Person person2 = new Person()
            {
                Name = "Anna",
                Age = 32,
                Children = 2,
                Married = false,
                Birthdate = new DateTime(2002, 2, 2)
            };
            Evaluator evaluator = new Evaluator();
            var firstResult = evaluator.Evaluate<Person>(" (Age > 10) ", person2);
            if (firstResult)
                person2.Married = true;
            Assert.AreEqual(firstResult, true);
            var result = evaluator.Evaluate<Person>(" (Age > 10) set Married = false ", person1);
            Assert.AreEqual(result, true);
            Assert.AreEqual(person1.Married, false);
        }

        [TestMethod]
        public void SimpleExpressionEvaluatorWithThenElseMethod()
        {
            Person person1 = new Person()
            {
                Name = "Mathias",
                Age = 36,
                Children = 2,
                Married = true,
                Birthdate = new DateTime(1976, 5, 9)
            };
            Person person2 = new Person()
            {
                Name = "Anna",
                Age = 32,
                Children = 2,
                Married = false,
                Birthdate = new DateTime(2002, 2, 2)
            };
            Evaluator evaluator = new Evaluator();
            var result1 = evaluator.Evaluate<Person>(
                " (Age < 10) then SetCanReceiveBenefits(true) else SetCancelBenefits(true) ", person1);
            Assert.AreEqual(result1, false);
            Assert.AreEqual(person1.CancelBenefits, true);
            Assert.AreEqual(person1.ReceiveBenefits, false);
            var result2 = evaluator.Evaluate<Person>(
                " (Age > 10) then SetCanReceiveBenefits(true) else SetCancelBenefits(true) ", person2);
            Assert.AreEqual(result2, true);
            Assert.AreEqual(person2.CancelBenefits, false);
            Assert.AreEqual(person2.ReceiveBenefits, true);
        }

        [TestMethod]
        public void SimpleTemperatureTestMethod()
        {
            Temperature temperature = new Temperature()
            {
                TemperatureValue1 = 10.2,
                TemperatureValue2 = 30.5
            };
            Evaluator evaluator = new Evaluator();
            var result = TemperatureCalculator(temperature);
            Assert.AreEqual(result, 1);
        }

        private int TemperatureCalculator(Temperature temperature)
        {
            Evaluator evaluator = new Evaluator();
            var result1 = evaluator.Evaluate
                (" TemperatureValue1 < 32", temperature);
            if (result1)
                return 1;
            var result2 = evaluator.Evaluate
                (" TemperatureValue1 > 50.1 and TemperatureValue1 < 80", temperature);
            if (result2)
                return 2;
            var result3 = evaluator.Evaluate
                (" TemperatureValue1 > 104", temperature);
            if (result3)
                return 4;
            return 0;
        }

        [TestMethod]
        public void SimpleExpressionEvaluatorWasEmployedDateMethod()
        {
            Person person1 = new Person()
            {
                Name = "Mathias",
                Age = 36,
                Children = 2,
                Married = true,
                Birthdate = new DateTime(1976, 5, 9),
                EmployDate = new DateTime(2012,12,1)
            };
            Person person2 = new Person()
            {
                Name = "Anna",
                Age = 32,
                Children = 2,
                Married = false,
                Birthdate = new DateTime(2002, 2, 2),
                EmployDate = new DateTime(2013, 12, 1)
            };
            Person person3 = new Person()
            {
                Name = "Karo",
                Age = 38,
                Children = 2,
                Married = true,
                Birthdate = new DateTime(1976, 2, 2),
                EmployDate = new DateTime(2011, 12, 1)
            };
            Evaluator evaluator = new Evaluator();
            var result4 = evaluator.Evaluate(
                " (Age > 30) then (Married = false) then SetCanReceiveBenefits(true) else SetCancelBenefits(true) else SetStopBenefits(true) ", person3);
            Assert.AreEqual(result4, false);
            Assert.AreEqual(person3.CancelBenefits, true);
            Assert.AreEqual(person3.ReceiveBenefits, false);
            Assert.AreEqual(person3.StopBenefits, false);
            person3 = new Person()
            {
                Name = "Karo",
                Age = 38,
                Children = 2,
                Married = true,
                Birthdate = new DateTime(1976, 2, 2),
                EmployDate = new DateTime(2011, 12, 1)
            };
            var result5 = evaluator.Evaluate(
               " (Age > 40) then (Married = false) then SetCanReceiveBenefits(true) else SetCancelBenefits(true) else SetStopBenefits(true) ", person3);
            Assert.AreEqual(result5, false);
            Assert.AreEqual(person3.CancelBenefits, false);
            Assert.AreEqual(person3.ReceiveBenefits, false);
            Assert.AreEqual(person3.StopBenefits, true);

            //var result1 = evaluator.Evaluate
            //    (" (CalculateAge() >= 10 && Married = true) && WasEmployed('2013-12-01') = true ", person1);
            //Assert.AreEqual(result1, true);
            //var result2 = evaluator.Evaluate(" CalculateAge() >= 10 && Married = true || WasEmployed('2014-12-01') = false ", person2);
            //Assert.AreEqual(result2, false);
            //var result3 = evaluator.Evaluate(
            //    " (CalculateAge() >= 2 || Married = true || WasEmployed('2010-12-01') = true) then SetCanReceiveBenefits(true) else SetCancelBenefits(true) ", person3);
            //Assert.AreEqual(result3, true);
        }

        [TestMethod]
        public void SimpleExpressionEvaluatorWithPlusThenMethod()
        {
            Person person1 = new Person()
            {
                Name = "Mathias",
                Age = 36,
                Children = 2,
                Married = true,
                Birthdate = new DateTime(1976, 5, 9)
            };           
            Evaluator evaluator = new Evaluator();
            var result1 = evaluator.Evaluate<Person>(
                " (Age > 5 + 6) set Age = 12 ", person1);
            Assert.AreEqual(result1, true);
            Assert.AreEqual(person1.Age, 12);            
        }

        [TestMethod]
        public void SimpleExpressionPreEvaluatorWithThenMethod()
        {
            Person person1 = new Person()
            {
                Name = "Mathias",
                Age = 36,
                Children = 2,
                Married = true,
                Birthdate = new DateTime(1976, 5, 9),
                CancelBenefits = false,
                ReceiveBenefits = false
            };            
            Evaluator evaluator = new Evaluator();
            bool result = evaluator.Evaluate<Person>(
                " (Age > 10) then SetCanReceiveBenefits(true) ", person1);
            Assert.AreEqual(person1.ReceiveBenefits, true);
        }

        [TestMethod]
        public void SimpleExpressionPreEvaluatorEventPersonWithThenMethod()
        {
            EventPerson person1 = new EventPerson()
            {
                Name = "Mathias",
                ColorComplexion = 5,
                Height = 10,
                Event = "Wedding"                
            };
            Evaluator evaluator = new Evaluator();            
            bool result = evaluator.Evaluate<EventPerson>(
                " (ColorComplexion > 4 && Height > 5 && Event = 'Test') then SetDressNumber(5, 'Beautiful Dress') else SetDressNumber(10, 'Other Dress') ", person1); //,'Beautiful Dress'
            Assert.AreEqual(person1.DressNumber, 10);
            Assert.AreEqual(person1.DressName, "Other Dress");
        }

        [TestMethod]
        public void SimpleExpressionEvaluateNonBoolean()
        {
            EventPerson person1 = new EventPerson()
            {
                Name = "Mathias",
                ColorComplexion = 5,
                Height = 10,
                Event = "Wedding"
            };
            Evaluator evaluator = new Evaluator();
            var result = evaluator.EvaluateNonBoolean<EventPerson, double>(
                " 21 * (64 / 4 + (13 * 2)) ", person1); //,'Beautiful Dress'            
            Assert.AreEqual(result, 882);            
        }

        [TestMethod]
        public void SimpleExpressionPreEvaluatorEmailMultibeParameterMethod()
        {
            Customer customer = new Customer()
            {
                Email = "test@activated.com"               
            };
            Evaluator evaluator = new Evaluator();             
            bool result = evaluator.Evaluate<Customer>(
                "(Email = 'test@activated.com') then SendEmail('" + customer.Email + "', 'SUBJECT', 'This is test email'", customer);
            Assert.AreEqual(customer.MailAdress, "test@activated.com");
            Assert.AreEqual(customer.Subject, "SUBJECT"); 
            Assert.AreEqual(customer.Text, "This is test email");
        }

        [TestMethod]
        public void SimpleExpressionPreEvaluatorWithThenAndElseMethod()
        {
            Person person1 = new Person()
            {
                Name = "Mathias",
                Age = 36,
                Children = 2,
                Married = true,
                Birthdate = new DateTime(1976, 5, 9),
                CancelBenefits = false,
                ReceiveBenefits = false
            };
            Evaluator evaluator = new Evaluator();
            bool result = evaluator.Evaluate<Person>(
                 " (Age < 10) then SetCanReceiveBenefits(true) else SetCancelBenefits(true) ", person1);
            Assert.AreEqual(person1.CancelBenefits, true);
        }

        [TestMethod]
        public void SimpleExpressionPreEvaluatorWithThenElseMethod()
        {
            Person person1 = new Person()
            {
                Name = "Mathias",
                Age = 36,
                Children = 2,
                Married = true,
                Birthdate = new DateTime(1976, 5, 9),
                CancelBenefits = false,
                ReceiveBenefits = false
            };
            Person person2 = new Person()
            {
                Name = "Anna",
                Age = 32,
                Children = 2,
                Married = false,
                Birthdate = new DateTime(2002, 2, 2),
                CancelBenefits = false,
                ReceiveBenefits = false
            };
            Evaluator evaluator = new Evaluator();
            var tuple = evaluator.PreEvaluate<Person>(
                " (Age < 10) then SetCanReceiveBenefits(true) else SetCancelBenefits(true) ");
            evaluator.ExecuteEvaluate(tuple, person1);
            evaluator.ExecuteEvaluate(tuple, person2);
            Assert.AreEqual(person1.CancelBenefits, true);
            Assert.AreEqual(person2.CancelBenefits, true);
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(new Person[] { person1, person2 },
                " (Age > 10) then SetCanReceiveBenefits(true) else SetCancelBenefits(true) ");
            Assert.AreEqual(result, true);
            Assert.AreEqual(person1.ReceiveBenefits, true);
            Assert.AreEqual(person2.ReceiveBenefits, true);
        }

        [TestMethod]
        public void SimpleExpressionStringIsNullMethod()
        {
            Person person1 = new Person()
            {
                Name = "mathias",
                Age = 36,
                Children = 2,
                Married = true,
                Birthdate = new DateTime(1976,05,09),
                CancelBenefits = false,
                ReceiveBenefits = false
            };
            Person person2 = new Person()
            {
                Name = null,
                Age = 32,
                Children = 2,
                Married = false,
                Birthdate = DateTime.Now,
                CancelBenefits = false,
                ReceiveBenefits = false
            };
            Evaluator evaluator = new Evaluator();
            var result1 = evaluator.Evaluate<Person>(
                " Name = 'mathias' ", person1);
            Assert.AreEqual(result1, true);     
            var result2 = evaluator.Evaluate<Person>(
                " Birthdate is null ", person2);
            Assert.AreEqual(result2, false);            
        }

        [TestMethod]
        public void SimpleRuleLoaderPersonAddressValidateRulesAll()
        {
            Person person = new Person() { 
                Name = "Mathias", Age = 36, Children = 2, Married = true };
            Adresse adresseOne = new Adresse() { 
                Street = "Teststreet1", Plz = 3030, City = "New York", ActiveState = true };
            Adresse adresseTwo = new Adresse() { 
                Street = "Teststreet2", Plz = 1010, City = "London", ActiveState = false };
            Adresse adresseThree = new Adresse() { 
                Street = "Teststreet3", Plz = 2020, City = "Paris", ActiveState = false };        
            person.Adresses_.Add(adresseOne);
            person.Adresses_.Add(adresseTwo);
            person.Adresses_.Add(adresseThree);
            RuleLoader ruleLoader = new RuleLoader();
            // new Rule("City", Operator.Equal, "New York");
            Rule firstAdressRule = ruleLoader.Load(4);
            // new Rule("ActiveState", Operator.Equal, true);      
            Rule secondAdressRule = ruleLoader.Load(5);
            RuleEngine.RuleEngine ruleEngine = new RuleEngine.RuleEngine();
            var firstAdressRuleFunc = 
                ruleEngine.CompileRule<Adresse>(firstAdressRule);
            var secondAdressRuleFunc = 
                ruleEngine.CompileRule<Adresse>(secondAdressRule);
            RuleValidator ruleValidator = new RuleValidator();
            bool result = ruleValidator.
                ValidateValuesAny(person.Adresses_, firstAdressRuleFunc);
            Assert.AreEqual(result, true);
            result = ruleValidator.
                ValidateValuesAny(person.Adresses_, secondAdressRuleFunc);            
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void SimpleRuleLoaderPersonValidateRulesSum()
        {
            Person person1 = new Person() { 
                Name = "Mathias", Age = 35, Children = 2 };
            Person person2 = new Person() { 
                Name = "Anna", Age = 32, Children = 2 };
            RuleLoader ruleLoader = new RuleLoader();
            // new Rule("Age", Operator.LessThanOrEqual, 50);
            Rule rule1 = ruleLoader.Load(2);
            // new Rule("Children", Operator.GreaterThan, 0);
            Rule rule2 = ruleLoader.Load(3);
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateRulesSum(
                new Person[] { person1, person2 },
                new Rule[] { rule1, rule2 });            
            Assert.AreEqual(result, false);
        }
        
        [TestMethod]
        public void SimpleRuleLoaderPersonEvaluateExpression()
        {
            Person person1 = new Person() 
                { Name = "Mathias", Age = 35, Children = 2 };
            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            // new Rule(" Name = 'mathias' ");
            Rule rule1 = expressionRuleLoader.Load(1);
            // new Rule(" Age = 35 ");
            Rule rule2 = expressionRuleLoader.Load(2);           
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRules(person1, rule1);
            Assert.AreEqual(result, true);
            result = ruleValidator.ValidateExpressionRules(person1, rule2);
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void SimpleRuleLoaderPersonEvaluateExpressionAny()
        {
            Person person1 = new Person() { Name = "Mathias", Age = 36, Children = 2 };
            Person person2 = new Person() { Name = "Anna", Age = 35, Children = 2 };

            List<Rule> rules = new List<Rule>();
            rules.Add(new Rule(" Name = 'Mathias' "));
            rules.Add(new Rule(" Age = 35 "));

            RuleValidator ruleValidator = new RuleValidator();            
            var result = ruleValidator.ValidateExpressionRulesAny(new Person[] { person1, person2 },
                rules);
            Assert.AreEqual(result, true);            
        }

        [TestMethod]
        public void SimpleRuleLoaderPersonEvaluateFoundInExpression()
        {
            Person person1 = new Person() { Name = "Mathias", Age = 36, Children = 2 };
            Person person2 = new Person() { Name = "Anna", Age = 35, Children = 2 };
            Person person3 = new Person() { Name = "Emil", Age = 4, Children = 0 };            

            List<Person> persons = new List<Person>();
            persons.Add(person1);
            persons.Add(person2);
            persons.Add(person3);

            RuleEngine.RuleEngine ruleEngine = new RuleEngine.RuleEngine();
            var ruleFunc =
                ruleEngine.CompileRule<Person>("Name", Operator.FoundIn, persons);
            var result = ruleFunc(person1);

            Rule<Person> rule = new Rule<Person>("Name", Operator.NotFoundIn, persons);
            var ruleFuncRule =
                ruleEngine.CompileRule<Person>(rule);
            
            var ruleFuncRuleResult = ruleFuncRule(person3);
            Assert.AreEqual(result, true); 
            Assert.AreEqual(ruleFuncRuleResult, false);
        }

        [TestMethod]
        public void SimpleRuleLoaderPersonEvaluateExpressionAll()
        {
            Person person1 = new Person() { Name = "Mathias", Age = 35, Children = 2 };
            Person person2 = new Person() { Name = "Anna", Age = 32, Children = 2 };
            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            // new Rule(" Name = 'mathias' ");
            Rule rule1 = expressionRuleLoader.Load(1);
            // new Rule(" Age = 35 ");
            Rule rule2 = expressionRuleLoader.Load(2);
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRulesAll(new Person[] { person1, person2 },
                new Rule[] { rule1, rule2 });
            Assert.AreEqual(result, false);            
        }

        [TestMethod]
        public void SimpleRuleLoaderPersonValidateExpressionRules()
        {
            Person person1 = new Person() { Name = "Mathias", Age = 35, Children = 2 };
            Person person2 = new Person() { Name = "Anna", Age = 32, Children = 2 };
            ExpressionRuleLoader expressionRuleLoader = new ExpressionRuleLoader();
            // new Rule(" Name = 'mathias' ");
            Rule rule1 = expressionRuleLoader.Load(1);
            // new Rule(" Age = 35 ");
            Rule rule2 = expressionRuleLoader.Load(2);
            RuleValidator ruleValidator = new RuleValidator();
            var result = ruleValidator.ValidateExpressionRules(new Person[] { person1, person2 },
                new Rule[] { rule1, rule2 });
            foreach(var item in result)
            {
                if(item.Item3)
                    Debug.WriteLine("Value: " + item.Item1.Name + " with ProcessingRule: " + item.Item2.ProcessingRule + item.Item2.Value + " passed.");
                else
                    Debug.WriteLine("Value: " + item.Item1.Name + " with ProcessingRule: " + item.Item2.ProcessingRule + item.Item2.Value + " failed.");
            }
        }

        

        [TestMethod]
        public void SimpleExpressionLikeMethod()
        {
            Person person1 = new Person()
            {
                Name = "mathias",
                Age = 36,
                Children = 2,
                Married = true,
                Birthdate = new DateTime(1976, 05, 09),
                CancelBenefits = false,
                ReceiveBenefits = false
            };
            Person person2 = new Person()
            {
                Name = "anna",
                Age = 32,
                Children = 2,
                Married = false,
                Birthdate = DateTime.Now,
                CancelBenefits = false,
                ReceiveBenefits = false
            };
            Evaluator evaluator = new Evaluator();
            var result1 = evaluator.Evaluate<Person>(
                " Name like 'math%' ", person1);
            Assert.AreEqual(result1, true);
            var result2 = evaluator.Evaluate<Person>(
                " Name like '?nn?' ", person2);

            //var result2 = evaluator.PreEvaluate<Person>(
            //   " Name like '?nn?' ", person2);

            Assert.AreEqual(result2, true);
            List<Person> list = new List<Person>() { person1, person2 };
            foreach(var person in list)
            {
                var result = evaluator.Evaluate<Person>(
                    " Name like 'mat%' || Name like 'a??a' ", person); 
                if(result)
                {
                    Debug.WriteLine(person.Name);
                    Assert.AreEqual(result, true);
                }
            }
        }

        [TestMethod]
        public void SimpleRuleLoaderText()
        {
            User user = new User()
            {
                Text = "andrew john jessica"
            };
            List<User> list = new List<User>();
            list.Add(new User() { Text = "andrew jackson clark" });
            list.Add(new User() { Text = "johnsson jackson clark" });
            list.Add(new User() { Text = "jessica jackson john" });                   
            Dictionary<User, int> itemList = new Dictionary<User, int>();
            list.ForEach(innerUser => 
                {
                    var userText = user.Text.Split(' ').ToList();
                    userText.ForEach(userTextPart =>
                    {
                        if (innerUser.Text.Split(' ').Contains(userTextPart))
                        {
                            var index = innerUser.Text.Split(' ').ToList().FindIndex(str => userTextPart == str);
                            if(!itemList.ContainsKey(innerUser))
                                itemList.Add(innerUser, index);
                        }
                    });
                } 
            );
            foreach(var item in itemList)
            {
                Debug.WriteLine(item.Key + " " + item.Value.ToString());
            }
        }

    [TestMethod]
    public void SimpleRuleLoaderProductEvaluateExpression()
    {
        Cart cart = new Cart();
        cart.Add(new Product()
        { ProdID = "D123", productName = "Star Wars", pType = "DVD", price = 14.99, discount = 0 });
        cart.Add(new Product()
        { ProdID = "D567", productName = "Ghostbuster", pType = "DVD", price = 19.99, discount = 0 });
        cart.Add(new Product()
        { ProdID = "BABC", productName = "Ghostbuster", pType = "BOOK", price = 7.99, discount = 0 });
        cart.customer = new Person() { Name = "mathias", Married = true };
        var rule = new Rule(" (Married = true) set Discont = 15 ");
        Debug.WriteLine("Product before Rule Expression execution!");
        foreach (var product in cart.Products)
        {
            Debug.WriteLine("Product: " + product.productName + " Price: " + product.price);
        }
        RuleValidator ruleValidator = new RuleValidator();
        var result = ruleValidator.ValidateExpressionRules(cart,
            rule);
        Debug.WriteLine("Product after Rule Expression execution!");
        foreach (var product in cart.Products)
        {
            Debug.WriteLine("Product: " + product.productName + " Price: " + product.price);
        }
        Assert.AreEqual(result, true);
    }
    }
}
