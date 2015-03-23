//////////////////////////////////////////////////////////////////////////
// TypeTable.cs This package represents and manipulates tables of       //
//                                              Types and relationships //
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
 *      TypeTable: Keeps and manipulates TypeElement which represent class, interface struct or enum
 *      RelationshipTable : Keeps and mamipulates Relationships which are kept as dictionary of Type
 */
/* Required Files:
 *   IDependencyAnalyzerService.cs
 *   
 * Build command:
 *   csc  TypeTable.cs IDependencyAnalyzerService.cs
 *                      
 *   
 * Maintenance History:
 * --------------------

 * ver 1.0 : 06 Oct 2014
 * - first release
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
namespace DependencyAnalyzer
{
    public class TypeTable
    {
        public Dictionary<string, List<TypeElement>> types { get; set; }
        public TypeTable()
        {
            types = new Dictionary<string, List<TypeElement>>();
        }

        public TypeTable(List<TypeElement> _types)
        {
            types = new Dictionary<string, List<TypeElement>>();
            Merge(_types);
        }

        /* Adds type into the type table if it doesnt already exist. */
        public bool add(string TypeName, string Type, string Namespace, string Filename)
        {
            if (types.Keys.Contains(TypeName))
            {
                foreach (TypeElement te in types[TypeName])
                {
                    if (te.Namespace == Namespace)
                        return false;
                }
                TypeElement insert = new TypeElement();
                insert.TypeName = TypeName;
                insert.Namespace = Namespace;
                insert.FileName = Filename;
                insert.Type = Type;
                types[TypeName].Add(insert);
                return true;
            }
            TypeElement elem = new TypeElement();
            elem.TypeName = TypeName;
            elem.Namespace = Namespace;
            elem.FileName = Filename;
            elem.Type = Type;
            types[TypeName] = new List<TypeElement>();
            types[TypeName].Add(elem);
            return true;
        }
        public bool remove(string Type)
        {
            return types.Remove(Type);
        }
        public bool contains(string Type)
        {
            return types.Keys.Contains(Type);
        }
        public bool add(TypeElement element)
        {
            return add(element.TypeName, element.Type, element.Namespace, element.FileName);
        }

        /* Get XML representation of a type table. */
        public string ToXmlString()
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("typeInfo");
            doc.Add(root);
            foreach (string type in types.Keys)
            {
                XElement typ = new XElement("type");
                root.Add(typ);
                XElement name = new XElement("name");
                name.Value = type;
                typ.Add(name);
                foreach (TypeElement te in types[type])
                {
                    XElement namespce = new XElement("namespace");
                    // namespce.Value = te.Namespace;
                    typ.Add(namespce);
                    XElement filename = new XElement("filename");
                    filename.Value = te.FileName;
                    typ.Add(filename);
                    XElement definedType = new XElement("definedType");
                    definedType.Value = te.Type;
                    typ.Add(definedType);
                }
            }
            return doc.ToString();
        }

        public void show()
        {

            foreach (string Type in types.Keys)
            {
                Console.Write("\n  {0}", Type);
                foreach (TypeElement te in types[Type])
                {
                    Console.Write("\n    Namespace = {0,-20} File = {1,-20}", te.Namespace, te.FileName);
                }
            }
        }

        /* Add a list of Type Elements to the table. */
        public bool add(List<TypeElement> elements)
        {
            bool successful = true;
            foreach (TypeElement element in elements)
            {
                successful = add(element) && successful;
            }
            return successful;
        }

        public void Merge(TypeTable table)
        {
            foreach (string key in table.types.Keys)
            {
                add(table.types[key]);
            }
        }


        public void Merge(List<TypeElement> _types)
        {
            foreach (TypeElement type in _types)
            {
                add(type);
            }
        }

        public bool isEmpty()
        {
            return types.Keys.Count == 0;
        }

        /* Load TypeTable from a xml string */
        public static TypeTable loadFromXML(string xml)
        {

            TypeTable table = new TypeTable();
            try 
            {
                XDocument doc = XDocument.Parse(xml);
                XElement root = doc.Element("typeInfo");

                foreach (XElement type in root.Elements("type"))
                {
                    TypeElement element = new TypeElement();
                    element.TypeName = type.Element("name").Value;
                    element.Namespace = type.Element("namespace").Value;
                    element.FileName = type.Element("filename").Value;
                    element.Type = type.Element("definedType").Value;
                    table.add(element);
                }
            }

            catch(Exception e)
            {
                Console.WriteLine(e);
                table.Clear();
            }
           
            return table;
        }

        public void Clear()
        {
            types.Clear();
        }

#if(TEST_TYPETABLE)
        static void Main(string[] args)
        {
            TypeTable table = new TypeTable();
            TypeElement element = new TypeElement();
            element.FileName = "Test.cs";
            element.TypeName = "Test";
            element.Type = "class";

            table.add(element);
            Console.WriteLine("________ Table 1 ________________");
            table.show();

            TypeTable table2 = new TypeTable();
            TypeElement element2 = new TypeElement();
            element2.FileName = "Test.cs";
            element2.TypeName = "Test";
            element2.Type = "class";
            
            table2.add(element2);
            Console.WriteLine("________ Table 2 ________________");
            table2.show();

            table.Merge(table2);
            Console.WriteLine("________ After Mergin Table 1 with Table 2 ________________");
            table.show();

        }
#endif

        public TypeElement GetTypeElement(string name)
        {
            // Problem Happens here
            return types[name].First();
        }
    }


    public class RelationshipTable
    {
        public Dictionary<TypeElement, List<TypeElement>> relationshipTable { get; set; }

        public RelationshipTable()
        {
            relationshipTable = new Dictionary<TypeElement, List<TypeElement>>();
        }

        /* Add Relationship into the relationship table. */
        public bool add(TypeElement child, TypeElement parent)
        {
            if (relationshipTable.Keys.Contains(child))
            {
                foreach (TypeElement te in relationshipTable[child])
                {
                    if (te == parent)                   // Don't Add if its already existing
                        return false;
                }
                relationshipTable[child].Add(parent);
                return true;
            }

            List<TypeElement> parents = new List<TypeElement>();
            parents.Add(parent);

            relationshipTable[child] = parents;
            return true;

        }

        public void Merge(RelationshipTable _results)
        {
            foreach (TypeElement child in _results.relationshipTable.Keys)
            {
                List<TypeElement> parents = _results.relationshipTable[child];
                foreach (TypeElement parent in parents)
                {
                    add(child, parent);
                }
            }
        }
        /*
         * XML Representation of RelationshipTable
         * 
         * <RelationshipInfo>
         *      <Dependency>
         *          <child>
         *              <name></name>
         *              <namespace></namespace>
         *              <filename></filename>
         *              <definedtype></definedtype>
         *          </child>
         *          <Relationship>
         *          <parent>
         *              <name></name>
         *              <namespace></namespace>
         *              <filename></filename>
         *              <definedtype></definedtype>
         *          </parent>
         *          <RelationshipName></RelationshipName>
         *          </Relationship>
         *          <Relationship>
         *          ..............
         *          </Relationship>
         *          .........
         *          ........
         *       </Dependency> 
         *      <Dependency>
         *      ............
         *      </Dependency>
         *      ...........
         *      ............
         * </RelationshipInfo>
         */

        /* Get XML representation of a relationship table. */
        public string ToXMLString()
        {
            string xmlString = "";

            XDocument doc = new XDocument();
            XElement root = new XElement("RelationshipInfo");
            doc.Add(root);
            foreach (TypeElement child in relationshipTable.Keys)
            {
                XElement dependencyElement = new XElement("Dependency");
                root.Add(dependencyElement);
                XElement childElement = new XElement("child");
                dependencyElement.Add(childElement);
                LoadXElemenFromTypeElement(child, childElement);
                XElement relationshipElement = new XElement("Relationship");
                childElement.Add(relationshipElement);
                
                foreach (TypeElement parent in relationshipTable[child])
                {
                    XElement parentElement = new XElement("parent");
                    LoadXElemenFromTypeElement(parent, parentElement);
                    relationshipElement.Add(parentElement);
                }
               
            }
            xmlString = doc.ToString();
            return xmlString;
        }

        /* Get Relationship table from the xml string */
        public static RelationshipTable loadFromXML(string xml)
        {
            RelationshipTable table = new RelationshipTable();
            XDocument doc = XDocument.Parse(xml);
            XElement root = doc.Element("RelationshipInfo");

            foreach (XElement dependency in root.Elements("Dependency"))
            {
                XElement childElement = dependency.Element("child");
                TypeElement child = GetTypeElementFromXElement(childElement);

                foreach (XElement relationshipElement in childElement.Elements("Relationship"))
                {
                    XElement parentElement = relationshipElement.Element("parent");
                    TypeElement parent = GetTypeElementFromXElement(parentElement);
                    table.add(child, parent);
                }
                
            }
            return table;
        }
        
        /* Load Relationship Table from a file */
        public static RelationshipTable loadFromXMLFile(string xml)
        {
            XDocument doc = XDocument.Load(xml);
            return loadFromXML(doc.ToString());
        }

        /* Get TypeElement from a XElement*/
        private static TypeElement GetTypeElementFromXElement(XElement parentElement)
        {
            TypeElement parent = new TypeElement();
            parent.TypeName = parentElement.Element("name").Value;
            parent.Namespace = parentElement.Element("namespace").Value;
            parent.FileName = parentElement.Element("filename").Value;
            parent.Type = parentElement.Element("definedType").Value;
            return parent;
        }

        /* load XElement from TypeElement */
        private void LoadXElemenFromTypeElement(TypeElement child, XElement childElement)
        {
            XElement name = new XElement("name");
            name.Value = child.TypeName;
            childElement.Add(name);
            XElement namespce = new XElement("namespace");
            // namespce.Value = te.Namespace;
            childElement.Add(namespce);
            XElement filename = new XElement("filename");
            filename.Value = child.FileName;
            childElement.Add(filename);
            XElement definedType = new XElement("definedType");
            definedType.Value = child.Type;
            childElement.Add(definedType);
        }

        public Dictionary<string,List<string>> GetTypeDependency() 
        {
            Dictionary<string, List<string>> typeDependency = new Dictionary<string, List<string>>();
            foreach (TypeElement child in relationshipTable.Keys)
            {
                if (!typeDependency.Keys.Contains(child.TypeName))
                    typeDependency.Add(child.TypeName,new List<string>());

                foreach (TypeElement parent in relationshipTable[child])
                {
                     typeDependency[child.TypeName].Add(parent.TypeName);
                }

            }
            return typeDependency;
        }

        public Dictionary<string, List<string>> GetPackageDependecy() 
        {
            Dictionary<string, List<string>> packageDependency = new Dictionary<string, List<string>>();
            foreach (TypeElement child in relationshipTable.Keys)
            {
                if (!packageDependency.Keys.Contains(child.FileName))
                    packageDependency.Add(child.FileName, new List<string>());

                foreach (TypeElement parent in relationshipTable[child])
                {
                    packageDependency[child.FileName].Add(parent.FileName);
                }

            }
            return packageDependency;
        }

        /* Get Package Dependency XML Representation */
        public string PackageDependencyXMLRepresentation() 
        {
            string xmlString = "";

            Dictionary<string, List<string>> pckgDeps = GetPackageDependecy();
            XDocument doc = new XDocument();
            XElement root = new XElement("PackageDependencyInfo");
            doc.Add(root);
            foreach (string parent in pckgDeps.Keys)
            {
                XElement dependencyElement = new XElement("Dependency");
                root.Add(dependencyElement);
                XElement parentElement = new XElement("parent");
                dependencyElement.Add(parentElement);
                parentElement.Value = parent;
                XElement childrenElement = new XElement("Children");
                parentElement.Add(childrenElement);

                foreach (string child in pckgDeps[parent])
                {
                    XElement childElement = new XElement("child");
                    childElement.Value = child;
                    childrenElement.Add(parentElement);
                }

            }
            xmlString = doc.ToString();
            Console.WriteLine(xmlString);
            return xmlString;
        }

        /* Save relationship Table to a file */
        public void saveToFile(string filePath) 
        {
            XDocument typeDoc = XDocument.Parse(ToXMLString());
            typeDoc.Save(filePath);

        }
#if(TEST_TYPETABLE)
        static void Main(string[] args)
        {
            RelationshipTable table = new RelationshipTable();
            TypeElement parent = new TypeElement();
            parent.FileName = "Test.cs";
            parent.TypeName = "Test";
            parent.Type = "class";

            TypeElement child = new TypeElement();
            child.FileName = "Test.cs";
            child.TypeName = "Test";
            child.Type = "class";

            table.add(parent,child);

            Console.WriteLine(table.PackageDependencyXMLRepresentation());
            Console.WriteLine(table.ToXMLString());


        }
#endif
    }
}
