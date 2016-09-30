using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SurpassApiSdk.DataContracts.Candidate;

namespace SurpassAPI
{
    class Program
    {
        public static string SurpassUrl { get; set; }
        public static string SurpassUsername { get; set; }
        public static string SurpassPassword { get; set; }
        static void Main(string[] args)
        {
            var mySurpassClient = new SurpassApiHelper(SurpassUrl, SurpassUsername, SurpassPassword);

        }
    }
}
