/////////////////////////////////////////////////////////////////////////////////
// Analyzer.cs Analyze Function Complexity or user derfined type Relationships //                        
// ver 1.0                                                                     //
// Language:    C#, 2013, .Net Framework 4.5                                   //
// Platform:    Macbook Pro, Win 7.0                                           //
// Application: CSE681, Project #4 Fall 2014                                  //     
// Author:      Isira Samarasekera (315)5717375, issamara@syr.edu              //
/////////////////////////////////////////////////////////////////////////////////

/*
 * Module Operations:
 * ------------------
 * This module defines the following classes:
 *   Analyzer : Abstract class to Anlyze a set of a files
 *   FunctionComplexityAnalyzer : Concrete Analyzer which Anlayses a set of files 
 *   for User Defined Types, Functions , Size and complexity of the functions
 *   TypeRelationshipAnalyzer : Concrete Analyzer which analyses a set of files for 
 *   Agrregation, Inheritance, Composition and using relationships between user defined types
 */
/* Required Files:
 *    IRulesAndActions.cs, RulesAndActions.cs, Parser.cs, Semi.cs, Toker.cs
 *   
 * Build command:
 *   csc /D:TEST_ANALYZER Analyzer.cs Parser.cs IRulesAndActions.cs RulesAndActions.cs \
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
using System.IO;
using DependencyAnalyzer;

namespace CodeAnalysis
{
    // Abstract class to analyze a set of files 
    public abstract class Analyzer
    {
        // extracts the Results for the given file
        protected abstract void UpdateResults(string file);

        protected abstract bool ParseFile(string file);
        public virtual void Analyze(List<String> files)
        {
            foreach (String file in files)
                AnalyzeFile(file);
        }
        protected void AnalyzeFile(String file)
        {
            Console.WriteLine("Analyzing File: "+ file);
            if (ParseFile(file))
                UpdateResults(file);
        }
        
        protected void ParseSemi(CSsemi.CSemiExp semi, Parser parser)
        {
            try
            {
                while (semi.getSemi())
                    parser.parse(semi);
            }
            catch (Exception ex)
            {
                Console.Write("\n\n  {0}\n", ex.Message);
            }
        }

        protected CSsemi.CSemiExp GetSemiExpressionFromFile(object file)
        {
            CSsemi.CSemiExp semi = new CSsemi.CSemiExp();
            semi.displayNewLines = false;
            if (!semi.open(file as string))
            {
                Console.Write("\n  Can't open {0}\n\n", file);
                return null;
            }

            return semi;
        }   
       

#if(TEST_ANALYZER)

        static void Main(string[] args)
        {
            string path = "../../";
            List<string> patterns = new List<string>();
            patterns.Add("*.cs");
            TypeAnalyzer funcAnalyzer = new TypeAnalyzer();
            List<string> files = funcAnalyzer.getFiles(path, patterns);
            funcAnalyzer.Analyze(files);

            Console.WriteLine("Displaying Function Complexities");
            Console.WriteLine("=================================");

            TypeTable interestedTypes = new TypeTable();
            TypeTable allTypes = new TypeTable();
            Analyzer relationshipAnalyzer = new TypeRelationshipAnalyzer(interestedTypes, allTypes);

            relationshipAnalyzer.Analyze(files);

            //results = relationshipAnalyzer.getResults();
            Console.WriteLine("\n\nDisplaying Relationship Results");
            Console.WriteLine("=================================");
        }

        // Given a folder as a path and the a list of patterns populate the matching files
        private List<string> getFiles(string path, List<string> patterns)
        {
            FileManager fm = new FileManager();
            foreach (string pattern in patterns)
            {
                fm.addPattern(pattern);
            }
            fm.findFiles(path);
            return fm.Files;
        } 
#endif
       
    }

 
    public class TypeAnalyzer: Analyzer   
    {

        List<TypeElement> userDefineTypes_ = new List<TypeElement>();
        protected override bool ParseFile(String file)
        {
            CSsemi.CSemiExp semi = GetSemiExpressionFromFile(file);
            if (semi == null)
                return false;
            BuildCodeAnalyzer builder = new BuildCodeAnalyzer(semi);
            Parser parser = builder.build();
            ParseSemi(semi, parser);
            semi.close();
            return true;
        }

        // extracts the FunctionComplexityResults for the given file
        protected override void UpdateResults(string fileName)
        {
            List<TypeElement> _results = Repository.getInstance().userDefinedTypes;
            for (int i = 0; i < _results.Count; i++)
            {
                TypeElement result = _results.ElementAt(i);
                result.FileName = fileName;
                userDefineTypes_.Add(result);
            }    
        }

        public List<TypeElement> userDefinedTypes
        {
            get{return userDefineTypes_;}
        }
               
    }

    // Analyze a set of files to get Relationships among the user defined types
    public class TypeRelationshipAnalyzer : Analyzer
    {
        TypeTable interestedTypes;
        TypeTable allTypes;
        RelationshipTable relationshipTable;

        // Parses the files to extract the user defined types

        public TypeRelationshipAnalyzer( TypeTable _interestedTypes, TypeTable _allTypes) 
        {
            interestedTypes = _interestedTypes;
            allTypes = _allTypes;
            relationshipTable = new RelationshipTable();
        }

        // Parses the files to extract relationships among userdefined types
        protected override bool ParseFile(String file)
        {
            CSsemi.CSemiExp semi = GetSemiExpressionFromFile(file);
            if (semi == null)
                return false;
            BuildCodeAnalyzer builder = new BuildCodeAnalyzer(semi);
            Parser parser = builder.buildParserForRelationships(interestedTypes,allTypes);
            ParseSemi(semi, parser);
            semi.close();
            return true;
        }
        protected override void UpdateResults(string fileName)
        {
            RelationshipTable _results = Repository.getInstance().relationshipTable;
            relationshipTable.Merge(_results);
        }

        public RelationshipTable relationshipResults()
        {
            return relationshipTable;
        }
    }

}
