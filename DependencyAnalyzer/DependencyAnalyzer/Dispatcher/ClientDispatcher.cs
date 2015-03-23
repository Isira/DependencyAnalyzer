//////////////////////////////////////////////////////////////////////////
// ClientDispatcher.cs Main executive pakage coordinate all the components     //
// ver 1.0                                                              //
// Language:    C#, 2013, .Net Framework 4.5                            //
// Platform:    Macbook Pro, Win 7.0                                    //
// Application: CSE681, Project #4, Fall 2014                           //     
// Author:      Isira Samarasekera (315)5717375, issamara@syr.edu       //
//////////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ------------------
 * This module defines the following class
 *     ClientDispatcher: Extending from the IDispatcher, when a message is received in the client side dispatches 
 *                      it to the appropriate handler
 */
/* Required Files:
 *   ClientUIHandler.cs , Communication.cs , ConfigurationLoader.cs, DependencyAgent.cs, DependencyHandler.cs, 
 *              IDependencyAnalyzerService.cs, MainApplication.cs, ProjectFileFinder.cs RequestGenerator.cs
 *                      Utillity.cs
 * Build command:
 *   csc ClientDispatcher.cs ClientUIHandler.cs Communication.cs ConfigurationLoader.cs DependencyAgent.cs DependencyHandler.cs, 
 *              IDependencyAnalyzerService.cs MainApplication.cs ProjectFileFinder.cs RequestGenerator.cs
 *                      Utillity.cs
 *                      
 *   note:
 *      This package doesn't have a main function the functionality is to merely forward the request to the appropriate Handler,
 *      Thus not providing a testable functionality.
 * Maintenance History
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
public class ClientDispatcher : IDispatcher 
{
        
        private ClientUIHanlder uiHandler;
        private DisplayExecutive display;
        DependencyHandler depHandler;

        public ClientDispatcher(ClientUIHanlder uiHandler, DisplayExecutive display, DependencyHandler depHandler)
        {
            // TODO: Complete member initialization
            this.uiHandler = uiHandler;
            this.display = display;
            this.depHandler = depHandler;
        }
        /* Forwards the message to the appropriate handler in order to process. */
        public override void Dispatch(Message msg) 
        {
            switch(msg.cmd)
            {
                case Message.Command.Projects:
                    OnListOfProjectsReceived(msg);
                    break;
                
                case Message.Command.Depanal:
                    onResultsReceived(msg);
                    break;

                default:
                    break;
            }
             
        }
        
         /* Process a DepAnal result message */
        private void onResultsReceived(Message msg)
        {
            depHandler.OnAnalyzeResultReceived(msg);
        }

        /* Process a List of project result message */
        void OnListOfProjectsReceived(Message msg)
        { 
            List<string> projects = Utillity.ConvertToObject<List<string>>(msg.body);
            display.ProjectsReceived(msg.src,projects);      
        }



    }
}
