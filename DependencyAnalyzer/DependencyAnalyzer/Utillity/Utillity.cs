//////////////////////////////////////////////////////////////////////////
// Utillity.cs Utilly pakage for xml serializing and deserializing      //
// ver 1.0                                                              //
// Language:    C#, 2013, .Net Framework 4.5                            //
// Platform:    Macbook Pro, Win 7.0                                    //
// Application: CSE681, Project #2, Fall 2014                           //     
// Author:      Isira Samarasekera (315)5717375, issamara@syr.edu       //
//////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * ------------------
 * This module defines the following class:
 *     Utillity : Provide utillity functionality to serialize object to xml and deserialize xml to object
 *     public interface:
 *     string obj = new string("Hello");
 *     string xmlRepresentation = Utillity.ConvertToXml(obj);
 *     Object stringRepresentation = ConvertToObject<string>(xmlRepresentation);
 */
/* Required Files:
 *   
 * Build command:
 *   csc  Utillity.cs
 *                      
 *   
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
    public class Utillity
    {
        /* Generic function to convert object to xml. */
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

        /* Generic function to desirialise string to a object. */
        public static  T ConvertToObject<T>(string toDeserialize)
        {

            try
            {
                XmlSerializer _xmlSerializer = new XmlSerializer(typeof(T));

                using (StringReader writer = new StringReader(toDeserialize))
                {
                    var result = (T)_xmlSerializer.Deserialize(writer);
                    return result;
                }

            }
            catch (Exception)
            {
                return default(T);
            }
        }


        #if(TEST_UTILLITY)
         
        static void Main(string[] args)
        {
            string obj = new string("Hello".ToCharArray());
            string xmlRepresentation = Utillity.ConvertToXml(obj);
            Console.WriteLine(xmlRepresentation);

            Object stringRepresentation = ConvertToObject<string>(xmlRepresentation);
            Console.WriteLine(stringRepresentation);
        }

#endif



    }
}
