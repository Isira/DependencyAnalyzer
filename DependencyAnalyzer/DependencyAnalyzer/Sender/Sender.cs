using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Xml.Serialization;

namespace DependencyAnalyzer
{
    public class Sender
    {
           
        ICommunicator CreateClientChannel(string url)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            EndpointAddress address = new EndpointAddress(url);
            ChannelFactory<ICommunicator> factory =
              new ChannelFactory<ICommunicator>(binding, address);
            return factory.CreateChannel();
        }



        public void Send(Message msg)
        {
            ICommunicator _service = CreateClientChannel(msg.dst.AbsoluteUri);
            _service.PostMessage(msg);       
        }

        public void Send(List<Message> messages)
        {
            foreach(Message msg in messages)
            {
                msg.ShowMessage();
                Send(msg);
            }
        }
        static void Main(string[] args)
        {
        }
    }



    
}
