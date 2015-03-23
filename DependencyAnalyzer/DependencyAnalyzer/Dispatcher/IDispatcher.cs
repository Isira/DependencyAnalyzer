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
 *     Executive: Mediate and coordinate CommandLineParser, FileManager, Analyzer and Display
 */
/* Required Files:
 *   Analyzer.cs , CommandLineParser.cs , Display.cs, FileManager.cs, Parser.cs, IRulesAndActions.cs, RulesAndActions.cs,
 *                      Semi.cs,  Toker.cs
 * Build command:
 *   csc  Executive.cs Analyzer.cs CommandLineParser.cs Display.cs FileManager.cs Parser.cs IRulesAndActions.cs RulesAndActions.cs \
 *                      Semi.cs Toker.cs
 *                      
 * note:
 *  This package doesn't contain a main function as its an interface.
 * Maintenance History:
 * --------------------
 * ver 1.0 : 19 Nov 2014
 * - first release
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DependencyAnalyzer
{
    public abstract class  IDispatcher 
    { 
       public abstract void Dispatch(Message msg);
    }
}
