//////////////////////////////////////////////////////////////////////////////////////
// RequestGenerator.cs Generate Request Messages which needs to be communicated     //
// ver 1.0                                                                          //
// Language:    C#, 2013, .Net Framework 4.5                                        //
// Platform:    Macbook Pro, Win 7.0                                                //
// Application: CSE681, Project #4, Fall 2014                                       //     
// Author:      Isira Samarasekera (315)5717375, issamara@syr.edu                   //
//////////////////////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ------------------
 * This module defines the following class:
 *     MessageGenerator: Generate Request Messages which needs to be communicated
 */
/* Required Files:
 *   ConfigurationLoader.cs , IDependencyAnalyzerService.cs 
 *   
 * Build command:
 *   csc  RequestGenerator.cs ConfigurationLoader.cs , IDependencyAnalyzerService.cs                    
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
using DependencyAnalyzer;
using System.Xml.Serialization;
using System.IO;

namespace DependencyAnalyzer
{
    public static class MessageGenerator
    {

    #if(TEST_REQ_GEN)  
        static void Main(string[] args)
        {
            List<string> servers = new List<string>();
            servers.Add("http://localhost:8081/ICommunicator");

            List<Message> messages = getProjectListRequestMessages("http://localhost:8081/ICommunicator", servers);

            foreach(Message msg in messages)
            {
                Console.WriteLine(msg.src);
                Console.WriteLine(msg.dst);
                Console.WriteLine(msg.cmd);
                Console.WriteLine(msg.body);
            }
        }
    #endif
        /* Serialize object to an xml string */
        public static string ConvertToXml(object toSerialize)
        {
            string temp;
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);
            var serializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, toSerialize, ns);
                temp = writer.ToString();
            }
            return temp;
        }

        /* Generate list of messages for Project list requests for all the servers. */
        public static List<Message> getProjectListRequestMessages( string localServiceUrl, List<string> servers) 
        {
            List<Message> requests = new List<Message>();
            // Create Message Required to create Messages
            foreach(string server in servers)
            {
                Message msg = new Message();
                msg.cmd = Message.Command.Projects;
                msg.src = localServiceUrl;
                msg.dst = server;
                requests.Add(msg);

            }
            return requests;
        }

        /* Generate Type Table update message list, which needs to be sent to all the servers */
        public static List<Message> GetTypeTableUpdateMessages(string body,string clientUri, List<string> servers) 
        {
            List<Message> requests = new List<Message>();
            // Create Message Required to create Messages
            foreach (string server in servers)
            {
                Message msg = new Message();
                msg.cmd = Message.Command.UpdateTypeTable;
                msg.src = clientUri;
                msg.dst = server;
                msg.body = body;
                requests.Add(msg);
            }
            return requests;
        }

        /* Generate a message which is sent to the client with the dep anal results */
        public static Message GetDependecyAnanlyzeClientResult(RelationshipTable table, string currentRequestUrl, string localServiceUrl)
        {
            Message msg = new Message();
            msg.cmd = Message.Command.Depanal;
            msg.src = localServiceUrl;
            msg.dst = currentRequestUrl;
            msg.body = table.ToXMLString();
            return msg;
        }

        /* Generate message to be sent with the results for the project list request. */
        public static Message GetProjectsReplyMessage(List<string> fileNames, string dstUrl, string localServiceUrl) 
        {
            Message msg = new Message();
            msg.src = localServiceUrl;
            msg.dst = dstUrl;
            msg.cmd = Message.Command.Projects;
            msg.body = ConvertToXml(fileNames);
            return msg;
        }

        public static Message GetDepAnalyzeMessage(List<string> selectedProjects, string dstUrl, string localServiceUrl)
        {
            Message msg = new Message();
            msg.src = localServiceUrl;
            msg.dst = dstUrl;
            msg.cmd = Message.Command.Depanal;
            msg.body = ConvertToXml(selectedProjects);
            return msg;
        }

    }
}
