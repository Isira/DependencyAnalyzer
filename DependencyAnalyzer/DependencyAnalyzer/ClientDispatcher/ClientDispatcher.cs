using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyAnalyzer
{
public class ClientDispatcher : IDispatcher 
{
        ClientUIHanlder handler;
        private ClientUIHanlder uiHandler;
        private GUIDisplayExecutive display;
        
        public ClientDispatcher(ClientUIHanlder _handler) 
        {
            handler = _handler;
        }

        public ClientDispatcher(ClientUIHanlder uiHandler,GUIDisplayExecutive display)
        {
            // TODO: Complete member initialization
            this.uiHandler = uiHandler;
            this.display = display;
        }
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

        private void onResultsReceived(Message msg)
        {
            Console.WriteLine("Analyze Result Received");
        }


        void OnListOfProjectsReceived(Message msg)
        { 
            List<string> projects = Utillity.ConvertToObject<List<string>>(msg.body);
            display.ProjectsReceived(msg.src,projects);      
        }

        static void Main(string[] args)
        {
        }


    }
}
