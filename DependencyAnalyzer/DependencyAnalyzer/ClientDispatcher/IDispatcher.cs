using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DependencyAnalyzer
{
    // Make this a separate thread that Dequeu the messages and let them process
    // If the message queue is empty, sleep

    public abstract class  IDispatcher 
    { 
       public abstract void Dispatch(Message msg);

    }

}
