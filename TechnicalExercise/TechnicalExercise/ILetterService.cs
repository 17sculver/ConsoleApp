using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalExercise
{
    public interface ILetterService
    {
        ///<summary>
        /// Combine two letter files into one letter.
        ///</summary>
        /// <param name="inputFile1"> File path for first letter</param>
        /// <param name="inputFile2">File path for second letter</param>
        /// <param name="resultFile">File path for the combined letter</param>
        void CombineTwoLetters(string inputFile1, string inputFile2, string resultFile);

        void RunService();
    }


    public class LetterService : ILetterService
    {

        public void RunService()
        {

        }

        public void CombineTwoLetters(string inputFile1, string inputFile2, string resultFile)
        {
            foreach(string line in File.ReadAllLines(inputFile1))
            {
                //If the file doesn't exist yet, create it
                if(!File.Exists(resultFile))
                {
                    using (StreamWriter sw = File.CreateText(resultFile))
                    {
                        sw.WriteLine(line);
                    }
                } 
                else //Otherwise, append to the existing file
                {
                    using (StreamWriter sw = File.AppendText(resultFile))
                    {
                        sw.WriteLine(line);
                    }
                }
            }
            //Puts a blank line to separate the 2 original letters for legibility ease.
            using (StreamWriter sw = File.AppendText(resultFile))
            {
                sw.WriteLine();
            }
            //Here, the resultFile will already exist from the prior loop, so don't need the if-check
            foreach (string line in File.ReadAllLines(inputFile2))
            {
                using (StreamWriter sw = File.AppendText(resultFile))
                {
                    sw.WriteLine(line);
                }
            }
        }
    }
}
