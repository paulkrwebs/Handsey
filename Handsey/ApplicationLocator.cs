using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public static class ApplicationLocator
    {
        private static volatile IApplicaton _instance;
        private static object syncRoot = new Object();

        public static IApplicationConfiguration ApplicationConfiguration { get; set; }

        public static IApplicaton Instance
        {
            get
            {
                throw new NotImplementedException("Need to make sure thead safe");

                // taken from https://msdn.microsoft.com/en-gb/library/ff650316.aspx
                //if (_instance == null)
                //{
                //    lock (syncRoot)
                //    {
                //        if (_instance == null)
                //            _instance = new Application();
                //    }
                //}

                //return _instance;
            }
        }
    }
}