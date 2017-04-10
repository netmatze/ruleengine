using System.Collections.Generic;
namespace RuleEngine
{
    public class Rule
    {
        private bool propertySet = false;
        public string PropertyName { get; set; }
        public Operator Operator_ { get; set; }
        public object Value { get; set; }        
        public string ProcessingRule { get; set; }

        public Rule(Operator operator_, object value)
        {
            this.Operator_ = operator_;
            this.Value = value;
        }

        public Rule(string propertyName, Operator operator_)
        {
            this.Operator_ = operator_;
            this.PropertyName = propertyName;
            if (!string.IsNullOrEmpty(propertyName))
                this.propertySet = true;
        }

        public Rule(string propertyName, Operator operator_, object value)
        {
            this.Operator_ = operator_;
            this.Value = value;
            this.PropertyName = propertyName;
            if (!string.IsNullOrEmpty(propertyName))
                this.propertySet = true;
        }

        public Rule(string processingRule)
        {
            this.ProcessingRule = processingRule;
        }
    }

    public class Rule<T> : Rule
    {
        public List<T> Values { get; set; }

        public Rule(string propertyName, Operator operator_, List<T> values) : 
            base(propertyName, operator_)
        {
            this.Values = values;
        }
    }
}
