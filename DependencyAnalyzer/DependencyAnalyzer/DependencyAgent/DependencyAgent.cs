///////////////////////////////////////////////////////////////////////////////////////////
// DependencyAgent.cs Analyze dependencies of a given request                           //
// ver 1.0                                                                              //
// Language:    C#, 2013, .Net Framework 4.5                                            //
// Platform:    Macbook Pro, Win 7.0                                                    //
// Application: CSE681, Project #4, Fall 2014                                           //     
// Author:      Isira Samarasekera (315)5717375, issamara@syr.edu                       //
//////////////////////////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ------------------
 * This module defines the following class:
 *     DependencyAgent: when dependency analyzing or Type table update message is received, do the dependency 
 *                      analyzing accordingly.
 */
/* Required Files:
 *   AnalyzerExecutive.cs , Communication.cs , ConfigurationLoader.cs, IDependencyAnalyzerAgent.cs, Utillity.cs, 
 *   
 * Build command:
 *   csc  DependencyAgent.cs AnalyzerExecutive.cs Communication.cs ConfigurationLoader.cs IDependencyAnalyzerAgent.cs 
 *                      Utillity.cs 
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
    public class DependencyAgent
    {
     
        ConfigurationLoader loader;

        public DependencyAgent(ConfigurationLoader _loader)
        {           
            loader = _loader;
        }

        public void UpdateTypeTableReceivedMethod(Message msg)
        {
            TypeTable table = GetTypeTableFromUpdateTableMessage(msg);
            Analyze(table,msg.src);
        }

#if(TEST_DEP_AGENT)
        static void Main(string[] args)
        {
            // This is called when a Dep Anal Message is received
            ConfigurationLoader _loader = new ConfigurationLoader("E:/DependencyAnalyzer/Server1.xml");
            _loader.Load();

            DependencyAgent depAnal = new DependencyAgent(_loader);
            List<string> selectedProjectPaths = new List<string>();
            selectedProjectPaths.Add("E:/DependencyAnalyzer/Server1.sln");

             Message msg = MessageGenerator.GetDepAnalyzeMessage(selectedProjectPaths,
                _loader.localServiceUrl, _loader.localServiceUrl);

             depAnal.DepAnalize(msg);
        }
#endif

        /* Analyze dependency*/
        public void DepAnalize(Message msg)
        {     
            TypeTable table = GetTypeTableFromDepAnalMessage(msg);
            BroadcastTypeTableUpdate(msg,table);
            Analyze(table,msg.src);
        }

        /* Broadcase the type table update message*/
        private void BroadcastTypeTableUpdate(Message msg, TypeTable table)
        {
            List<Message> updateTypeTableMessages = MessageGenerator.
                GetTypeTableUpdateMessages(table.ToXmlString(), msg.src, loader.servers);
            SendMessages(updateTypeTableMessages);
        }

        /* Analyze the projects with the type table provided */
        private void Analyze(TypeTable typeTable, string src)
        {
            GetTypeTableOfAllProjects getAllTypes = new GetTypeTableOfAllProjects(loader.rootPath);
            getAllTypes.Analyzer();
            TypeTable allTypes = getAllTypes.Results;

            GetRelationshipTable getDependencies = new GetRelationshipTable(loader.rootPath, typeTable, allTypes);
            getDependencies.Analyzer();
            RelationshipTable results = getDependencies.relationshipResults();

            Message result = MessageGenerator.GetDependecyAnanlyzeClientResult(results, src, loader.localServiceUrl);
            SendTheResultToClient(result);
        }

        /* Get TypeTable for  a DepAnal message */
        private TypeTable GetTypeTableFromDepAnalMessage(Message msg)
        {
            TypeTable table = ExtractTypesToDepAnalyze(msg);
            return table;
        }


        /* Get TypeTable for  a Update TypeTable message */
        private TypeTable GetTypeTableFromUpdateTableMessage(Message msg)
        {
            TypeTable table = TypeTable.loadFromXML(msg.body);
            return table;
        }

        private void SendMessages(List<Message> updateTypeTableMessages)
        {
            foreach (Message updateTypeTableMessage in updateTypeTableMessages)
            {
                Sender.Send(updateTypeTableMessage, loader.localServiceUrl);
            }
        }

        /*Get Typetable which needs to be used to process the dep Anal Request*/
        TypeTable ExtractTypesToDepAnalyze(Message msg)
        {
            List<string> solutionFilePaths = Utillity.ConvertToObject<List<string>>(msg.body);
            GetTypeTableOfAProject handler = new GetTypeTableOfAProject(solutionFilePaths);
            handler.Analyzer();
            return handler.Results;
        }

        private void SendTheResultToClient(Message msg)
        {
            Sender.Send(msg, "");
        }

    }

   
}
