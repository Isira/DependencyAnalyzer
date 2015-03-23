//////////////////////////////////////////////////////////////////////////////////////
// IService.cs provides contracts for the Dependency Analyzer service               //
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
 *     Message:    Data contract for message
 *     TypeElement: Data contract which represents the class for a TypeDefinition
 *     
 * Interface:
 *      ICommunicator:  DependencyAnalyzer service contract
 */
/* Required Files:
 *   
 * note:
 *  This package doesnt have a main function as it has only interfaces and data contracts
 * * Build command:
 *   csc  IService.cs 
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
using System.Runtime.Serialization;

namespace DependencyAnalyzer
{
    [DataContract]
    public class Message
    {
        public enum Command { Projects,UpdateTypeTable,Depanal};
        [DataMember]
        public Command cmd;
        [DataMember]
        public string src;
        [DataMember]
        public string dst;
        [DataMember]
        public string body;
    }


    [DataContract]
    public class TypeElement 
    {
        public string FileName{ get; set; }
        public string Namespace { get; set; }
        public string TypeName { get; set; }
        public string Type { get; set; }     
    }

    [ServiceContract]
    public interface ICommunicator
    {
        [OperationContract(IsOneWay=true)]
        void PostMessage(Message msg);
        Message GetMessage();
    }
}
