//////////////////////////////////////////////////////////////////////////
// AnalyzerExecutive.cs Main executive pakage coordinate all the components     //
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
 *     GetTypeTableOfAProject:  Get TypeTable of a Project
 *     GetTypeTableOfAllProjects :  Get TypeTable of all Projects
 *     GetRelationshipTable : Get Relationship table provided the TypeTables.
 */
/* Required Files:
 *   Analyzer.cs ,  FileManager.cs, IDependencyAnalyzerService.cs Parser.cs, IRulesAndActions.cs, RulesAndActions.cs,
 *                      Semi.cs,  Toker.cs
 * Build command:
 *   csc  AnalyzerExecutive.cs Analyzer.cs FileManager.cs IDependencyAnalyzerService.cs Parser.cs IRulesAndActions.cs RulesAndActions.cs
 *                      Semi.cs Toker.cs
 *                      
 *   
 * Maintenance History:
 * --------------------

 * ver 1.0 : 06 Oct 2014
 * - first release
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DependencyAnalyzer;

namespace CodeAnalysis
{
    /*  Get TypeTable for a Project */
    public class GetTypeTableOfAProject
    {
        List<string> solutionPaths;
        private TypeAnalyzer analyzer;
        FileManager fm;
        List<TypeElement> results;

        public GetTypeTableOfAProject(List<string> _solutionPaths) 
        {
            solutionPaths = _solutionPaths;
            analyzer = new TypeAnalyzer();
            fm = new FileManager();
        }

        public void Analyzer()
        {
            initFileManager();
            Analyze();
            results = analyzer.userDefinedTypes;
            
        }

        private void Analyze()
        {
            List<String> files = fm.Files;
            analyzer.Analyze(files);
        }

        private void initFileManager()
        {
            fm.recurse = true;
            fm.addPattern("*.cs");
            fm.FindFilesInSolutions(solutionPaths);
        }

        public TypeTable Results
        {
            get { return new TypeTable(results); }
        }
    }


    /*  Get TypeTable of all the projects  */
    public class GetTypeTableOfAllProjects
    {
        string rootFolder;
        private TypeAnalyzer analyzer;
        FileManager fm;
        TypeTable results;
        public GetTypeTableOfAllProjects(string _rootFolder) 
        {
            rootFolder = _rootFolder;
            analyzer = new TypeAnalyzer();
            fm = new FileManager();
        }

        public void  Analyzer()
        {
            initFileManager();
            Analyze();
            results = new TypeTable(analyzer.userDefinedTypes);      
        }

        private void Analyze()
        {
            List<String> files = fm.Files;
            analyzer.Analyze(files);
        }

        private void initFileManager()
        {
            fm.recurse = true;
            fm.addPattern("*.cs");
            fm.findFiles(rootFolder);
        }


        public TypeTable Results {
            get { return results; }
        }


    }

   /*  Get Relationship table provided the TypeTables. */
    public class GetRelationshipTable
    {
        string rootFolder;
        private TypeRelationshipAnalyzer analyzer;
        FileManager fm;

        public GetRelationshipTable(string _rootFolder,TypeTable _interestedTypes, TypeTable _allTypes)
        {
            rootFolder = _rootFolder;
            analyzer = new TypeRelationshipAnalyzer(_interestedTypes, _allTypes);
            fm = new FileManager();
        }

        public void Analyzer()
        {
            initFileManager();
            Analyze();
            
        }

        private void Analyze()
        {
            List<String> files = fm.Files;
            analyzer.Analyze(files);
        }

        private void initFileManager()
        {
            fm.recurse = true;
            fm.addPattern("*.cs");
            fm.findFiles(rootFolder);
        }

        public RelationshipTable relationshipResults() {
            return analyzer.relationshipResults();
        }

#if(TEST_ANALYZER_EXEC)
        static void Main(string[] args)
        {
            List<string> solutionFilePaths = new List<string>();
            solutionFilePaths.Add("E:DependencyAnalyzer");
            GetTypeTableOfAProject handler = new GetTypeTableOfAProject(solutionFilePaths);
            handler.Analyzer();
            TypeTable  typeTable = handler.Results;

            string rootPath = "E://DependencyAnalyzer";
            GetTypeTableOfAllProjects getAllTypes = new GetTypeTableOfAllProjects(rootPath);
            getAllTypes.Analyzer();
            TypeTable allTypes = getAllTypes.Results;

            GetRelationshipTable getDependencies = new GetRelationshipTable(rootPath, typeTable, allTypes);
            getDependencies.Analyzer();
            RelationshipTable results = getDependencies.relationshipResults();

            Console.WriteLine(results.ToXMLString());
        }
#endif
    }


}
