using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Models
{
    public class TechnicalEmployee : Employee
    {
        public string[] Languages { get; private set; }

        public void Change(string firstname, string[] languages)
        {
            Languages = languages;
            // log change

            base.Change(firstname);
        }
    }
}