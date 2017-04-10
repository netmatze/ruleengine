using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuleEngineTests
{
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public int Children { get; set; }
        public bool Married { get; set; }
        public DateTime Birthdate { get; set; }
        public Adresse Adresse_ { get; set; }
        public bool ReceiveBenefits { get; set; }
        public bool CancelBenefits { get; set; }

        public bool StopBenefits { get; set; }

        public DateTime EmployDate { get; set; }
        public void SetCanReceiveBenefits(bool receiveBenefits)
        {
            ReceiveBenefits = receiveBenefits;
        }
        public void SetCancelBenefits(bool cancelBenefits)
        {
            CancelBenefits = cancelBenefits;
        }

        public void SetStopBenefits(bool stopBenefits)
        {
            StopBenefits = stopBenefits;
        }

        public bool WasEmployed(DateTime dateTime)
        {
            if(EmployDate < dateTime)
            {
                return true;
            }
            return false;
        }

        public bool IsEmployed()
        {            
            return true;         
        }

        public int CalculateAge()
        {
            return DateTime.Now.Year - Birthdate.Year;
        }

        private List<Adresse> adresses = new List<Adresse>();
        public List<Adresse> Adresses_ 
        { 
            get { return adresses; } 
            set { adresses = value; } 
        }
    }

    public class EventPerson
    {
        public string Name { get; set; }
        public int ColorComplexion { get; set; }
        public int Height { get; set; }
        public string Event { get; set; }
        public int DressNumber { get; private set; }
        public string DressName { get; private set; }

        public void SetDressNumber(int dressNumber, string dressName)
        {
            DressNumber = dressNumber;
            DressName = dressName;
        }        
    }

    public class Customer
    {
        public string Email { get; set; }
        public string MailAdress { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }        

        public void SendEmail(string mailAdress, string subject, string text)
        {
            Text = text;
            Subject = subject;
            MailAdress = mailAdress;
        }
    }

    public class User
    {
        public string Text { get; set; }
    }

    public class Dictionary
    {
        public List<string> TextCollection { get; set; }
    }
}
