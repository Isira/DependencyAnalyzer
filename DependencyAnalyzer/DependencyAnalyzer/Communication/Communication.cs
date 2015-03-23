//////////////////////////////////////////////////////////////////////////
// Communication.cs Provides Server and Client classes  for listen and  //
//                                                      send messages   //
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
 *     Sender: Post the message in a threadsafe blocking queue, continuously running thread takes one message at a time
 *     and sends it through a channel.
 *     Receiver: Extended from ICommunication class, can open a channel, Post messages and close the channel.
 */

/* Required Files:
 *   BlockingQueue.cs , IDependencyAnalyzerService.cs  
 *   
 * Build command:
 *   csc  Communication.cs BlockingQueue.cs IDependencyAnalyzerService.cs 
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

namespace DependencyAnalyzer
{
        public class Receiver : ICommunicator
        {
            static BlockingQueue<Message> rcvBlockingQ = null;
            ServiceHost service = null;

            public Receiver()
            {
                if (rcvBlockingQ == null)
                    rcvBlockingQ = new BlockingQueue<Message>();
            }

            public void Close()
            {
                service.Close();
            }

            //  Create ServiceHost for Communication service

            public void CreateRecvChannel(string address)
            {
                BasicHttpBinding binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = Int32.MaxValue;
                Uri baseAddress = new Uri(address);
                service = new ServiceHost(typeof(Receiver), baseAddress);
                service.AddServiceEndpoint(typeof(ICommunicator), binding, baseAddress);
                service.Open();
            }

            // Implement service method to receive messages from other Peers

            public void PostMessage(Message msg)
            {
                rcvBlockingQ.enQ(msg);
            }

            // Implement service method to extract messages from other Peers.
            // This will often block on empty queue, so user should provide
            // read thread.

            public Message GetMessage()
            {
                return rcvBlockingQ.deQ();
            }


#if(TEST_COMM)
  
            static void Main(string[] args)
            {
                Receiver recvr = new Receiver();
                recvr.CreateRecvChannel(args[0]);

                Sender sender = new Sender(args[0], args[1]);
                Message msg = new Message();

                sender.PostMessage(msg);

            }
#endif

        }
        ///////////////////////////////////////////////////
        // client of another Peer's Communication service

        public class Sender
        {
            ICommunicator channel;
            string lastError = "";
            BlockingQueue<Message> sndBlockingQ = null;
            Thread sndThrd = null;
            int tryCount = 0, MaxCount = 10;
            string localUri;

            // Processing for sndThrd to pull msgs out of sndBlockingQ
            // and post them to another Peer's Communication service

            void ThreadProc()
            {
                while (true)
                {
                    Message msg = sndBlockingQ.deQ();
                    try
                    {
                        channel.PostMessage(msg);
                    }
                    catch (Exception ) 
                    {
                        Console.WriteLine("Exception occured While Posting Message, Discarding Message");
                    }
                    
                    if (msg.body == "quit")
                        break;
                }
            }


            // Create Communication channel proxy, sndBlockingQ, and
            // start sndThrd to send messages that client enqueues

            public Sender(string dstUrl,string srcUrl)
            {
                localUri = srcUrl;
                if (TryToConnect(dstUrl))
                    Start();
            }

            public void Start()
            {
                sndBlockingQ = new BlockingQueue<Message>();
                sndThrd = new Thread(ThreadProc);
                sndThrd.IsBackground = true;
                sndThrd.Start();     
            }

            public bool TryToConnect(string url)
            {
                while (true)
                {
                    try
                    {
                        CreateSendChannel(url);
                        tryCount = 0;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        if (++tryCount < MaxCount)
                            Thread.Sleep(100);
                        else
                        {
                            lastError = ex.Message;
                            break;
                        }
                    }
                }
                return false;
            }

            // Create proxy to another Peer's Communicator

            private void CreateSendChannel(string address)
            {
                EndpointAddress baseAddress = new EndpointAddress(address);
                BasicHttpBinding binding = new BasicHttpBinding();
                ChannelFactory<ICommunicator> factory
                  = new ChannelFactory<ICommunicator>(binding, address);
                channel = factory.CreateChannel();
            }

            // Sender posts message to another Peer's queue using
            // Communication service hosted by receipient via sndThrd

            public void PostMessage(Message msg)
            {
                sndBlockingQ.enQ(msg);
            }

            public void Close()
            {
                ChannelFactory<ICommunicator> temp = (ChannelFactory<ICommunicator>)channel;
                temp.Close();
            }

            public static bool Send(Message msg,string localUri) 
            {
                Sender sender = new Sender(msg.dst,localUri);
                sender.PostMessage(msg);
                return true;
            }


        }
    
}
