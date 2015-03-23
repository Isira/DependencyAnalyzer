//////////////////////////////////////////////////////////////////////////////
// GUIDisplayExective.cs Mediates and controls user actions through GUI    //
// ver 1.0                                                                  //
// Language:    C#, 2013, .Net Framework 4.5                                //
// Platform:    Macbook Pro, Win 7.0                                        //
// Application: CSE681, Project #4, Fall 2014                               //     
// Author:      Isira Samarasekera (315)5717375, issamara@syr.edu           //
//////////////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ------------------
 * This module defines the following class:
 *     GUIDisplayExecutive: Mediates and controls user actions through GUI
 */
/* Required Files:
 *   ClientUIHandler.cs , 
 *   
 * Build command:
 *   csc  GUIDisplayExecutive.cs ClientUIHandler.cs 
 *                      
 *   
 * Maintenance History:
 * --------------------
 * ver 1.0 : 19 Nov 2014
 * - first release
 * 
 * TODO:
 *  Extend the DisplayExecutive to make a command line display.
 */

using System.Collections.Generic;
using System.Windows;
using MainApplication;

namespace DependencyAnalyzer
{
    public abstract class DisplayExecutive
    {

        protected ClientUIHanlder handler;

        public abstract void StartUp();

        public void GetProjects() 
        {
            handler.GetProjects();
        }

        public abstract void ProjectsReceived(string server, List<string> projects);
     

        public void Analyze(Dictionary<string, List<string>> selectedProjects)
        {
            handler.Analyze(selectedProjects);
        }

        public abstract void ShowRelationshipTable(RelationshipTable table);


        public abstract void ClearAnalyzeResult();

    }
    public  class GUIDisplayExecutive : DisplayExecutive
    {
        MainWindow displayWindow;
        string configPath;

        public GUIDisplayExecutive(ClientUIHanlder _handler,string _configPath)
        {
            handler = _handler; 
            configPath = _configPath;

        }
        public override void StartUp() 
        {
            Application application = new Application();
            displayWindow = new MainWindow(this,configPath);
            application.Run(displayWindow);
        }

       

        public override void ProjectsReceived(string server, List<string> projects)
        {
            displayWindow.UpdateProjects(server,projects);
        }


        public override void ShowRelationshipTable(RelationshipTable table)
        {
            displayWindow.SetRelaletionshipTab(table);
        }

        public override void ClearAnalyzeResult()
        {
            displayWindow.ClearResults();
        }

#if(TEST_GUI_EXECUTIVE)
        public static void main() 
        {
            ConfigurationLoader loader = new ConfigurationLoader("E://Server/client.xml");
            ClientUIHanlder uiHandler = new ClientUIHanlder(loader);
            GUIDisplayExecutive display = new GUIDisplayExecutive(uiHandler);

            display.StartUp();
        }
#endif

   
    }
}
