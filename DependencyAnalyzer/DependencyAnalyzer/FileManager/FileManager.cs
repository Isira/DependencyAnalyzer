//////////////////////////////////////////////////////////////////////////
// FileManager.cs Detects files specified by a path and patterns        //
// ver 1.0                                                              //
// Language:    C#, 2013, .Net Framework 4.5                            //
// Platform:    Macbook Pro, Win 7.0                                    //
// Application: CSE681, Project #4, Fall 2014                           //     
// Author:      Isira Samarasekera (315)5717375, issamara@syr.edu       //
//////////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ------------------
 * This module defines the following class:
 *   
 */
/* Build command:
 *   csc /D:TEST_FILEMANAGER FileManager.cs
 *                      
 *   
 * Maintenance History:
 * --------------------
 * ver 1.0 : 19 Nov 2014
 * - first release
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CodeAnalysis
{
    public class FileManager
    {
        private List<string> files = new List<string>();
        private List<string> patterns = new List<string>();
        
        // Property specifying whether to recurse in to subdirectories. 
        //If set needs to traverse all the subdirectories to find the files
        public bool recurse
        {
            get;
            set;
        }

        // populates the files list for a given path and list of patterns
        public bool findFiles(string path)
        {
            if (!isValidPath(path)) {
                Console.WriteLine("Invalid Directory Path is Specified");
                return false;
            }
            
            if (patterns.Count == 0)
            {
                addPattern("*.*");
            }

            foreach (string pattern in patterns)
            {
                string[] newFiles = Directory.GetFiles(path, pattern);

                for (int i = 0; i < newFiles.Length; i++)
                {
                    newFiles[i] = Path.GetFullPath(newFiles[i]);
                }

                files.AddRange(newFiles);
            }

            if (recurse)
            {
                string[] dirs = Directory.GetDirectories(path);

                foreach (string dir in dirs)
                {
                    findFiles(dir);
                }
            }
            return true;
        }

        // Checks whether a directory exists for the given path
        private bool isValidPath(string path)
        {
            return  Directory.Exists(path);
        }

        
        public void addPattern(string pattern)
        {   
            if(!patterns.Contains(pattern))
                patterns.Add(pattern);
        }

        public void addPatterns(List<String> _patterns) {
            patterns.AddRange(_patterns);
        }

        public void reset() {
            files.Clear();
            patterns.Clear();
        }

        public List<string> Files
        {
            get { return files; }
        }


        public List<string>  FileNames 
        {
            get {
                var fileNames = from  file in files select Path.GetFileName(file);
                return fileNames.ToList<string>();
            }
        }

        public string getProjectFolderForSolution(string solutionPath) 
        {
            return Directory.GetParent(solutionPath).FullName;
        }

        void findFilesInSolution(string solutionPath) 
        {
            string solutionFolder = getProjectFolderForSolution(solutionPath);
            findFiles(solutionFolder);
        }

        public void FindFilesInSolutions(List<string> solutionPaths)
        {
            foreach (string solutionPath in solutionPaths)
                findFilesInSolution(solutionPath);
        }

#if(TEST_FILEMANAGER) 
        static void Main(string[] args)
        {
            Console.Write("\n Testing Filemanager Class");
            Console.Write("\n =========================\n");
            FileManager fm = new FileManager();
            fm.addPattern("*.cs");
            fm.findFiles("../../");

            List<string> files = fm.Files;

            foreach(string file in files)
            {
                Console.Write("\n {0}", file);
            }

            Console.Write("\n\n");   

        }
#endif
    }
}
