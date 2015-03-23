//////////////////////////////////////////////////////////////////////////
// MainWindow.xaml.cs Main executive pakage coordinate all the components     //
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
 *     MainWindow: Code behaviour for MainWindow
 */
/* Required Files:
 *   ClientUIHanlder.cs GUIDisplayExecutive.cs
 *   
 * Build command:
 *   csc  MainWindow.xaml.cs ClientUIHanlder.cs GUIDisplayExecutive.cs
 *      
 * note:
 *      This package doesnt have a main function as it depends on the other Classes to work
 *  
 * Maintenance History:
 * --------------------
 * ver 1.0 : 19 Nov 2014
 * - first release
 */

using System.Windows;
using System.Windows.Controls;
using DependencyAnalyzer;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace MainApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GUIDisplayExecutive guiExecutive;
        Dictionary<string,List<string>> projectsList;
        string configPath;

        public ObservableCollection<Project> _projects { get; private set; }
        public ObservableCollection<Dependency> _typeDependency { get; private set; }
        public ObservableCollection<Dependency> _packageDependency { get; private set; }

        RelationshipTable table;


        public MainWindow(GUIDisplayExecutive _guiExecutive,string _configPath)
        {
            InitializeComponent();
            guiExecutive = _guiExecutive;
            configPath = _configPath;
            InitializeClientSideObjects();
        }

        private void InitializeClientSideObjects()
        {
            projectsList = new Dictionary<string, List<string>>();
            _projects = new ObservableCollection<Project>();
            _typeDependency = new ObservableCollection<Dependency>();
            _packageDependency = new ObservableCollection<Dependency>();
            projectList.ItemsSource = _projects;
            TypeDependecyTable.ItemsSource = _typeDependency;
            PackageDependecyTable.ItemsSource = _packageDependency;
            XMLPAth.Text = Directory.GetCurrentDirectory(); 
            ConfigFilePath.Text = configPath;
        }


        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            string configurationFilePath = ConfigFilePath.Text;
            
            guiExecutive.GetProjects();
        }

        public void UpdateProjects(string server, List<string> projects)
        {
            if (projectsList.ContainsKey(server))
            {
                foreach (string project in projects)
                {
                    if (!projectsList[server].Contains(project))
                    {
                        projectsList[server].Add(project);
                        Dispatcher.Invoke(new Action<string,string>(addProject),
                                        System.Windows.Threading.DispatcherPriority.Background,
                                        server,project);
                    }

                }       
            }
            else 
            {
                projectsList[server] = projects;
                foreach (string project in projects)
                {                 
                    Dispatcher.Invoke(new Action<string, string>(addProject),
                                    System.Windows.Threading.DispatcherPriority.Background,
                                    server, project);
                }
            }
               
        }

        public void addProject(string server,string project)
        {
            Project p = new Project();
            p.name = Path.GetFileNameWithoutExtension(project) ;
            p.path = project;
            p.server = server;      
            _projects.Add(p);
            
        }

        public void addTypeDependency(string _parent, string _child)
        {
            Dependency p = new Dependency();
            p.child = _child;
            p.parent = _parent;
            p.relationship = "";
            if (!_typeDependency.Contains(p))
            _typeDependency.Add(p);
            
        }

        public void addPackageDependency(string _parent, string _child)
        {
            Dependency p = new Dependency();
            p.child = _child;
            p.parent = _parent;
            p.relationship = "";
            if (!_packageDependency.Contains(p))
            _packageDependency.Add(p);
           
            
        }
        private void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            // Get Selected Projects
            Dictionary<string,List<string>> selectedProjects = GetSelectedProjects();
            guiExecutive.Analyze(selectedProjects);
            Tabs.SelectedIndex = 1;
        }

        private Dictionary<string, List<string>> GetSelectedProjects()
        {
            Dictionary<string, List<string>> selectedProjects = new Dictionary<string,List<string>>();
            foreach (Project project in _projects)
            {
                if (project.selected)
                {
                    if (!selectedProjects.ContainsKey(project.server))
                    {
                        selectedProjects[project.server] = new List<string>();
                    }

                    selectedProjects[project.server].Add(project.path);
                }
            }
            return selectedProjects;
        }

        public void ClearResults()
        {
           _typeDependency.Clear();
            _packageDependency.Clear();
           // throw new NotImplementedException();
        }

        private void ShowTypeDependency(Dictionary<string, List<string>> typeDepencies)
        {
            foreach (string _parent in typeDepencies.Keys)
            {
                List<string> children = typeDepencies[_parent];

                foreach (string _child in children)
                {
                
                    Dispatcher.Invoke(new Action<string, string>(addTypeDependency),
                                   System.Windows.Threading.DispatcherPriority.Background,
                                   _parent, _child);
                   
                }
            }
        }

        private void ShowPackageDependency(Dictionary<string, List<string>> packageDepencies)
        {
            foreach (string _parent in packageDepencies.Keys)
            {
                List<string> children = packageDepencies[_parent];

                foreach (string _child in children)
                {
                    Dispatcher.Invoke(new Action<string, string>(addPackageDependency),
                                   System.Windows.Threading.DispatcherPriority.Background,
                                   _parent, _child);
                }
            }
        }


        private void OnlyPkg_Radio_Click(object sender, RoutedEventArgs e)
        {
                _typeDependency.Clear();
                ShowPackageDependency(table.GetPackageDependecy());            
        }


        private void OnlyType_Radio_Click(object sender, RoutedEventArgs e)
        {
                _packageDependency.Clear();
                ShowTypeDependency(table.GetTypeDependency());
        }

        private void All_Radio_Click(object sender, RoutedEventArgs e)
        {
         
                ShowTypeDependency(table.GetTypeDependency());
                ShowPackageDependency(table.GetPackageDependecy());
        }




        internal void SetRelaletionshipTab(RelationshipTable table)
        {
            this.table = table;
            Dictionary<string, List<string>> typeDepencies = table.GetTypeDependency();
            Dictionary<string, List<string>> packageDepencies = table.GetPackageDependecy();
            ShowTypeDependency(typeDepencies);
            ShowPackageDependency(packageDepencies);
        }


        private void LoadXML_Click(object sender, RoutedEventArgs e)
        {
            string currentXMLPath = XMLPAth.Text;
            string xmlFilePath = Path.Combine(currentXMLPath,"TypeDependecies.xml");

            if (File.Exists(xmlFilePath)) 
            {
                RelationshipTable _typeTable = RelationshipTable.loadFromXMLFile(xmlFilePath);
                All_Radio.IsChecked = true;
                SetRelaletionshipTab(_typeTable);
            }
            
        }

        private void SaveXML_Click(object sender, RoutedEventArgs e)
        {
            string currentXMLPath = XMLPAth.Text;
            string xmlFilePath = Path.Combine(currentXMLPath, "TypeDependecies.xml");
            if(table != null)
            table.saveToFile(xmlFilePath);
        }
    }

    
    public class Project
    {
              
        public string name { get; set; }
        public string path { get; set; }
        public string server { get; set; }
        public bool selected { get; set; }
    }

    public class Dependency : IEquatable<Dependency>

    {
        public string child { get; set; }
        public string parent { get; set; }
        public string relationship { get; set; }

        public bool Equals(Dependency other) 
        {
            return child.Equals(other.child) && parent.Equals(other.parent) && relationship.Equals(other.relationship);
        }
    }



}


