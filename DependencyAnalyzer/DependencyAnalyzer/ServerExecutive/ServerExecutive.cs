//////////////////////////////////////////////////////////////////////////
// ServerExecutive.cs Main executive pakage coordinate all components  //
//                                              in the Server side     //
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
 *     Server Executive: Mediate and coordinate ServerDispatcher, Receiver, ConfigurationLoader
 */
/* Required Files:
 *   Communication.cs , ConfigurationLoader.cs , Dispatcher.cs, IDependencyAnalyzerService.cs
 *   
 * Build command:
 *   csc  ServerExecutive.cs  Communication.cs , ConfigurationLoader.cs , Dispatcher.cs, IDependencyAnalyzerService.cs Communication.cs , ConfigurationLoader.cs , Dispatcher.cs, IDependencyAnalyzerService.cs
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
using System.ServiceModel;


namespace DependencyAnalyzer
{

    public class ServerExecutive
    {
      
        ServerDispatcher dispatcher;
        Receiver recvr;
        ConfigurationLoader loader;
        
        public ServerExecutive(string configurationFilePath) 
        {
            loader = new ConfigurationLoader(configurationFilePath);
            dispatcher = new ServerDispatcher(loader);                 
        }

        /* Initialize the serverside objects */
        void initialize() 
        {
            loader.Load();
        }

        public void execute() 
        {
            initialize();
            KeepRunning();
        }
        static void Main(string[] args)
        {
            ServerExecutive executive = new ServerExecutive(args[0]);
            executive.initialize();
            executive.StartUp();
            executive.KeepRunning();
        }

        private void KeepRunning()
        {
            Console.Write("\n  press key to terminate service\n");
            Console.ReadKey();
            Console.Write("\n");
        }

        /* Create the service channel to listen. */
        private void StartUp()
        {
            string endpoint = loader.localServiceUrl;
            
            try
            {
                recvr = new Receiver();
                recvr.CreateRecvChannel(endpoint);
                Console.WriteLine("Server Started on: " + endpoint);
                Task.Run(() => ThreadProc());
               
            }
            catch (Exception)
            {
                Console.WriteLine("Couldn't start up service:");
            }
        }

        /* This thread will wait for a message in the blocking queue and forward it to the dispatcher*/
        void ThreadProc()
        {
            while (true)
            {
                Message rcvdMsg = recvr.GetMessage();
                dispatcher.Dispatch(rcvdMsg);
            }
        }
    }
}
