//////////////////////////////////////////////////////////////////////////
// ProjectFileFinder.cs Given a folder root path finds all project      //
//files which is identified by .sln extension                           //
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
 *     ProjectFileFinder: Given a folder root path finds all project files which is identified by .sln extension  
 */
/* Required Files:
 *   FileManager.cs 
 * Build command:
 *   csc  ProjectFinder.cs FileManager.cs 
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
using CodeAnalysis;
namespace DependencyAnalyzer
{
    public class ProjectFileFinder
    {
        public List<string> projectFiles { get; set; }
        FileManager fileManager;
        string rootPath;

        public ProjectFileFinder(string _rootPath) {
            fileManager = new FileManager();
            rootPath = _rootPath;
        }
        
        /* Find projects(solutions) in the specified path. */
        public void findProjects() 
        {         
            fileManager.addPattern("*.sln");
            fileManager.recurse = true;
            fileManager.findFiles(rootPath);
            projectFiles = fileManager.Files;
        }

#if(PROJECT_FILE_FINDER)
        static void Main(string[] args)
        {
            ProjectFileFinder projectFileFinder = new ProjectFileFinder("E:\\Server");
            projectFileFinder.findProjects();
            List<string> fileNames = projectFileFinder.projectFiles;

            foreach(string file in fileNames)
            {
                Console.WriteLine(file);
            }
        }
#endif
    }
}
