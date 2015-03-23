//////////////////////////////////////////////////////////////////////////
// Executive.cs Main executive pakage coordinate all the components     //
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
 *     SeverDispatcher: Mediate and coordinate CommandLineParser, FileManager, Analyzer and Display
 */
/* Required Files:
 *   ClientUIHandler.cs , Communication.cs , ConfigurationLoader.cs, DependencyAgent.cs, DependencyHandler.cs, 
 *              IDependencyAnalyzerService.cs, MainApplication.cs, ProjectFileFinder.cs RequestGenerator.cs
 *                      Utillity.cs
 * Build command:
 *   csc ServerDispatcher.cs ClientUIHandler.cs Communication.cs ConfigurationLoader.cs DependencyAgent.cs DependencyHandler.cs, 
 *              IDependencyAnalyzerService.cs MainApplication.cs ProjectFileFinder.cs RequestGenerator.cs
 *                      Utillity.cs
 *   
 *   note:
 *      This package doesn't have a main function the functionality is to merely forward the request to the appropriate Handler,
 *      Thus not providing a testable functionality.
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
using System.Xml.Serialization;
using System.IO;

namespace DependencyAnalyzer
{
    // Make this a separate thread that Dequeu the messages and let them process
    // If the message queue is empty, sleep 
    
    // This Should Have a sender
    public class ServerDispatcher : IDispatcher
    {
        //AnalyzerAgent agent;
        ConfigurationLoader loader;
        DependencyAgent depAgent;
        public ServerDispatcher(ConfigurationLoader _loader) 
        {
            loader = _loader;         
            depAgent = new DependencyAgent(_loader);
        }

        // Forward the message to appropriate handler
        public override void Dispatch(Message msg) 
        { 
            switch (msg.cmd)
            {
                case Message.Command.Projects:
                    getProjectsList(msg);
                    break;

                case Message.Command.Depanal:
                    DepAnalyze(msg);
                    break;

                // Receiving this from other servers
                case Message.Command.UpdateTypeTable:
                    updateTypeTable(msg);
                    break;

                default:
                    // Unsupported Commands at the server end.
                    break;
            }        
        }

        private void DepAnalyze(Message msg)
        {
            depAgent.DepAnalize(msg);
        }


        void  getProjectsList(Message msg)
        {                  
            ProjectFileFinder projectFileFinder = new ProjectFileFinder(loader.rootPath);
            projectFileFinder.findProjects();
            List<string> fileNames = projectFileFinder.projectFiles;
            
            Message response =  MessageGenerator.GetProjectsReplyMessage(fileNames,msg.src,loader.localServiceUrl);
            Sender sender = new Sender(response.dst,response.src);
            sender.PostMessage(response);
        }

        void updateTypeTable(Message msg) 
        {
            depAgent.UpdateTypeTableReceivedMethod(msg);
        }

    }
}

