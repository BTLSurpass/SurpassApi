using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurpassAPI.Models
{
    public class PsychometricImportItem
    {
        public String QuestionName { get; set; }
        public String SetCase { get; set; }
        public String SetCaseNum { get; set; }
        public String Letter { get; set; }
        public Double IRT_A { get; set; }
        public Double IRT_B { get; set; }
        public Double IRT_C { get; set; }
        public List<String> Enemies { get; set; }
        public List<String> Friends { get; set; }

        public PsychometricImportItem(String[] questionData)
        {
            QuestionName = questionData[0];
            SetCase = questionData[1];
            SetCaseNum = questionData[2];
            Letter = questionData[3];
            IRT_A = Convert.ToDouble(questionData[4]);
            IRT_B = Convert.ToDouble(questionData[5]);
            IRT_C = Convert.ToDouble(questionData[6]);
            Enemies = new List<string>();
            if (questionData[7].ToLower() != "n/a")
            {
                var myList = questionData[7].Split('|');
                findAssociations(Enemies, myList);
            }
            Friends = new List<string>();
            if (questionData[8].ToLower() != "n/a")
            {
                var myList = questionData[8].Split('|');
                findAssociations(Friends, myList);
            }
            
        }

        private void findAssociations(List<String> relationList, String[] sourceList)
        {
            foreach (String data in sourceList)
            {
                //data is of form IT121193 (ID: 123P3) - we don't need the ID data
                var myEnemyData = data.Split(' ');
                var myEnemy = myEnemyData[0];
                relationList.Add(myEnemy);
            }
        }

    }
}
