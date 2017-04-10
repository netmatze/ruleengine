using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuleEngineTests
{
    public class Product
    {
        public string ProdID { get; set; }
        public string pType { get; set; }
        public string productName { get; set; }
        public double price { get; set; }
        public double discount;
    }

    public class Cart
    {
        private System.Collections.Generic.List<Product> products =
            new System.Collections.Generic.List<Product>();
        public Person customer { get; set; }

        public void Add(Product prod)
        {
            products.Add(prod);
        }

        private int discont;

        public int Discont
        {
            get { return discont; }
            set
            {
                discont = value;
                SetDiscont(discont);
            }
        }

        private void SetDiscont(int discont)
        {
            foreach(var product in products)
            {
                product.price -= product.price / 100 * 15;
            }
        }

        public bool Married
        {
            get { return customer.Married; }
        }

        public System.Collections.Generic.List<Product> Products
        {
            get { return products; }
        }
    }
}
