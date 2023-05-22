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
            var report_count = 0;
            var report_date = DateTime.Now.ToString("MM/dd/yyyy") + "Report";
            var sid_list = new List<string>();
            var admission_info = new DirectoryInfo(@"Input\Admission");
            var admission_folders = admission_info.GetDirectories();
            var scholarship_info = new DirectoryInfo(@"Input\Scholarship");
            var scholarship_folders = scholarship_info.GetDirectories();
            var output_info = new DirectoryInfo(@"Output");
            var output_folders = output_info.GetDirectories();
            var archive_info = new DirectoryInfo(@"Archive");
            
           //Get the latest report date before current iteration(to cover days during which the program wasn't run)
            var prior_report_date = (DateTime?) null;
            if (output_folders.Any())
            {
                foreach (var folder_info in output_folders)
                {
                    if(prior_report_date == null)
                    {
                        prior_report_date = folder_info.CreationTime;
                    } 
                    else
                    {
                        if(folder_info.CreationTime > prior_report_date)
                        {
                            prior_report_date = folder_info.CreationTime;
                        }
                    }
                }
            }
            //Find the oldest input folder within the Admission subfolder that hasn't been looked at (compare from prior_report_date)
            if (prior_report_date == null || prior_report_date < DateTime.Now)
            {

                DirectoryInfo result_folder = output_info.CreateSubdirectory(DateTime.Now.ToString("yyyyMMdd"));
                //Loop through each input folder between oldest input folder to today's date (DateTime.today)
                foreach (var admiss_folder in admission_folders)
                {
                    if(prior_report_date == null || admiss_folder.CreationTime > prior_report_date) //No reports have been made, must go through each existing folder, else only go through folders newer than prior report
                    {
                        foreach(var file in admiss_folder.GetFiles())
                        {
                            //First, copy all files into archive
                            string targetFilePath = Path.Combine(Path.GetFullPath(archive_info.FullName), file.Name);
                            file.CopyTo(targetFilePath);

                            //Check each file for a matching studentID in scholarship
                            var result = Path.GetFileNameWithoutExtension(file.Name);
                            var stu_id = result.Substring(result.Length - 8);
                            CheckForScholarship(stu_id, file.FullName, report_count, sid_list, scholarship_folders, result_folder);
                        }
                    }
                }
                //Generate report
                //Assumption: the ---- in the demonstration of the report formatted was interpreted as
                //though the line above it was meant to be report folder name and below was contents
                var report_file = result_folder + @"\" + report_date;
                if (!File.Exists(report_file))
                {
                    using (StreamWriter sw = File.CreateText(report_file))
                    {
                        sw.WriteLine("Number of Combined Letters: {0}", report_count);
                        if (report_count > 0)
                        {
                            foreach (var stu_id in sid_list)
                            {
                                sw.WriteLine($"{stu_id}");
                            }
                        }
                    }
                } 
                else
                {
                    using (StreamWriter sw = File.AppendText(report_file))
                    {
                        sw.WriteLine("Number of Combined Letters: {0}", report_count);
                        if (report_count > 0)
                        {
                            foreach (var stu_id in sid_list)
                            {
                                sw.WriteLine($"{stu_id}");
                            }
                        }
                    }
                }
                

                foreach (var scholar_folder in scholarship_folders)
                {
                    if (prior_report_date == null || scholar_folder.CreationTime > prior_report_date) 
                    {
                        foreach (var file in scholar_folder.GetFiles())
                        {
                            //copy all files into archive
                            string targetFilePath = Path.Combine(Path.GetFullPath(scholarship_info.FullName), file.Name);
                            file.CopyTo(targetFilePath);
                        }
                    }
                }
            }
        }

        private void CheckForScholarship(string studentID, string inputFile1, int report_count, List<string> id_list, DirectoryInfo[] scholarship_folders, DirectoryInfo result_folder)
        {

            var output_info = new DirectoryInfo(@"Output");
            var output_folders = output_info.GetDirectories();
            //If there is a matching scholarship letter, call CombineTwoLetters to generate report
            foreach (var scholar_folder in scholarship_folders)
            {
                if(scholar_folder.CreationTime == File.GetCreationTime(inputFile1))
                {
                    foreach(var file in scholar_folder.GetFiles())
                    {
                        if (file.Name.Contains(studentID))
                        {
                            //Increase count for report
                            report_count++;
                            //Add studentID to report list
                            id_list.Add(studentID);
                            string result_name = Path.GetFullPath(result_folder.FullName) + "/" + studentID + ".txt";
                            CombineTwoLetters(inputFile1, file.FullName, result_name);
                        }
                    }
                }
            }
            
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
