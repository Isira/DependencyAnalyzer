//////////////////////////////////////////////////////////////////////////
// ClientExecutive.cs Main executive pakage coordinate all the components     //
//                                                in the client side    //
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
 *     ClientExecutive: Mediate and coordinate  Receiver, ClientDispatcher, ConfigurationLoader, ClientUIHanlder,
 *                      GUIDisplayExecutive, DependencyHandler
 */
/* Required Files:
 *   ClientHanlder.cs , Communication.cs , ConfigurationLoader.cs, DependencyHandler.cs, Dispatcher.cs, I
 *                      IDependencyAnalyzerService.cs, MainApplication.cs,
 * Build command:
 *   csc  ClientExecutive.cs ClientHanlder.cs Communication.cs ConfigurationLoader.cs DependencyHandler.cs Dispatcher.cs
 *                      IDependencyAnalyzerService.cs MainApplication.cs,
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
using MainApplication;



namespace DependencyAnalyzer
{
    /* Main Client Executive class */
    public class ClientExecutive
    {
        Receiver receiver;
        ClientDispatcher dispatcher;      
        ConfigurationLoader loader;       
        ClientUIHanlder uiHandler;
        DisplayExecutive display;
        DependencyHandler depHandler;
      
        public ClientExecutive(string configurationFilePath) 
        {
            loader = new ConfigurationLoader(configurationFilePath);
            uiHandler = new ClientUIHanlder(loader);
            display = new GUIDisplayExecutive(uiHandler, configurationFilePath);
            depHandler = new DependencyHandler(loader, display, uiHandler);
            dispatcher = new ClientDispatcher(uiHandler, display, depHandler);        
        }

        public void initialize() 
        {
            loader.Load();
            StartClientsideService();
            display.StartUp();
        }



        [STAThread]
        static void Main( string[] args)
        {
            // Load Configurations from the ConfigurationLoader
            
            ClientExecutive executive = new ClientExecutive(args[0]);
            executive.execute();
            
        }

        public void execute()
        {      
            Console.Write("\n  Client Started \n");
            initialize();
        }
         
        /* Start Client Side Service Listner */
        void StartClientsideService() 
        {
            string endpoint = loader.localServiceUrl;
            try
            {
                receiver = new Receiver();
                receiver.CreateRecvChannel(endpoint);
                Task.Run(() => ThreadProc());
            }
            catch (Exception)
            {
                Console.WriteLine("Exception occured");
            }
        }

        /* This thread will wait on a message and forward it to the dispather */
        void ThreadProc()
        {
            while (true)
            {
                Message rcvdMsg = receiver.GetMessage();
                dispatcher.Dispatch(rcvdMsg);
            }
        }
    }
    }


