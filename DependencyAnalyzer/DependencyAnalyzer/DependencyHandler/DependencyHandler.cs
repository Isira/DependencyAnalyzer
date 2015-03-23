//////////////////////////////////////////////////////////////////////////////////
// DependencyHandler.cs : Act as a mediator for the user interface actions      //
// ver 1.0                                                                      //
// Language:    C#, 2013, .Net Framework 4.5                                    //
// Platform:    Macbook Pro, Win 7.0                                            //
// Application: CSE681, Project #4, Fall 2014                                   //     
// Author:      Isira Samarasekera (315)5717375, issamara@syr.edu               //
//////////////////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ------------------
 * This module defines the following class:
 *     DependencyHandler: Mediate and coordinate CommandLineParser, FileManager, Analyzer and Display
 */
/* Required Files:
 *   ClientUIHanlder.cs , Communication.cs , ConfigurationLoader.cs, IDependencyAnalyzerService.cs, MainApplication.cs, 
 *      TypeTable.cs
 * 
 * Build command:
 *   csc  DependencyHanlder.cs ClientUIHanlder.cs Communication.cs ConfigurationLoader.cs IDependencyAnalyzerService.cs 
 *      MainApplication.cs TypeTable.cs 
 *                      
 *   
 * Maintenance History:
 * --------------------
 * ver 1.0 : 19 Nov 2014
 * - first release
 */

using System.Xml.Linq;

namespace DependencyAnalyzer
{
    public class DependencyHandler
    {
        ConfigurationLoader loader;
        RelationshipTable table;
        DisplayExecutive executive;
        ClientUIHanlder uiHandler;

        public DependencyHandler(ConfigurationLoader _loader, DisplayExecutive _executive, ClientUIHanlder _uiHandler)
        {
            loader = _loader;
            table = new RelationshipTable();
            executive = _executive;
            uiHandler = _uiHandler;
            uiHandler.subscribe += MessageSent;
        }

        /* Invoked when a relationship table is received */
        public void OnRelationshipsReceived(RelationshipTable _table)
        {
           
            table.Merge(_table);
            executive.ShowRelationshipTable(table);
        }
#if(TEST_DEP_HANLDER)
        static void Main(string[] args)
        {
            string configFilePath = args[0];
            ConfigurationLoader loader = new ConfigurationLoader(configFilePath);
            ClientUIHanlder uiHandler = new ClientUIHanlder(loader);
            GUIDisplayExecutive display = new GUIDisplayExecutive(uiHandler, configFilePath);
            DependencyHandler depHandler = new DependencyHandler(loader, display, uiHandler);

            Message msg = new Message();

            depHandler.OnAnalyzeResultReceived(msg);
        }
#endif 
        /* Invoked when Analyze result is received */
        public void OnAnalyzeResultReceived(Message msg)
        {
            RelationshipTable _table = RelationshipTable.loadFromXML(msg.body);
            OnRelationshipsReceived(_table);
        }

        void MessageSent() 
        {
            table.relationshipTable.Clear();
            executive.ClearAnalyzeResult();
        }
    }
}

