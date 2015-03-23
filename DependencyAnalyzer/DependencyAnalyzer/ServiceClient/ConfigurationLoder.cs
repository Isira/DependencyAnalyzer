//////////////////////////////////////////////////////////////////////////////
// ConfugurationLoader.cs Loads configuration settings given a file path    //
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
 *     ConfigurationLoader: Loads configuration settings from xml
 */
/* 
 * Build command:
 *   csc  ConfigurationLoader.cs
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
using System.Threading;
using System.Xml.Linq;

namespace DependencyAnalyzer
{
    // Loads Configurations
    public class ConfigurationLoader
    {
        public List<string> servers    {get;set;}
        public string localServiceUrl   {get;set;}    
        string configurationFilePath;
        public string rootPath { get; set; }  

        public ConfigurationLoader(string _configurationFilePath)
        {
            configurationFilePath = _configurationFilePath;
            servers = new List<string>();
        }

        /* Read configuration xml file and load the settings */
        public void Load()
        {
            try
            {
                XDocument doc = XDocument.Load(configurationFilePath);
                localServiceUrl = doc.Root.Element("LocalAddress").Value;
                rootPath = doc.Root.Element("RootPath").Value;
                var _servers = doc.Root.Elements("Servers").Elements("Server");

                foreach (XElement _server in _servers)
                {
                    servers.Add(_server.Value);
                }
            }
            catch(Exception)
            {
                Console.WriteLine("Loading XML");
            }
        }
#if(CONFIG_LOADER)
    static void Main(string[] args)
    {
        ConfigurationLoader client = new ConfigurationLoader("E:/DependencyAnalyzer/Server2.xml");
        client.Load();

        Console.WriteLine("Local URL:");
        Console.WriteLine(client.localServiceUrl);
        Console.WriteLine("\n");

        Console.WriteLine("Service URLs:");
        Console.WriteLine(client.servers);      
        Console.WriteLine("\n");    
    }
#endif
  }
 }

