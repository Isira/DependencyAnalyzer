/////////////////////////////////////////////////////////////////////////////
// ClientUIHandler.cs : Mediates in handling requests provided              //                        
//                              by the user through command line and  GUI   //
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
 *     ClientUIHanlder: mediates the requests made by the user through CLI and GUI
 */
/* Required Files:
 *   Communication.cs , ConfigurationLoader.cs , IDependencyAnalyzerService.cs, RequetGenerator.cs, 
 *   
 * Build command:
 *   csc  ClientHandler.cs Communication.cs ConfigurationLoader.cs IDependencyService.cs RequetGenerator.cs 
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


namespace DependencyAnalyzer
{
    public class ClientUIHanlder
    {

        ConfigurationLoader loader;
        public delegate void NewRequestMade();
        public NewRequestMade subscribe;

        public ClientUIHanlder(ConfigurationLoader _loader)
        {
            loader = _loader;
        }

        /* Send request to get the list of projects in a server */
        public void GetProjects()
        {
            List<Message> requests = MessageGenerator.
                getProjectListRequestMessages(loader.localServiceUrl,loader.servers);
            
            foreach (Message request in requests)
            {        
                Sender sender = new Sender(request.dst,request.src);
                sender.PostMessage(request);
            }
        }

#if(TEST_CL_HANDLER)
        static void Main(string[] args)
        {

        }
#endif
        /* Send request to analyze a list of projects to a server */
        public void Analyze(Dictionary<string, List<string>> selectedProjects)
        {
            subscribe();
            foreach(string server in selectedProjects.Keys)
            {
                Message msg = MessageGenerator.
                    GetDepAnalyzeMessage(selectedProjects[server], server, loader.localServiceUrl);
                Sender sender = new Sender(msg.dst, msg.src);
                sender.PostMessage(msg);            
            }
        }

    }
}


  
