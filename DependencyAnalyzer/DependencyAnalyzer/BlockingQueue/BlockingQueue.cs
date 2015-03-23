//////////////////////////////////////////////////////////////////////////
// BlockingQueue.cs Provides threadsafe operations on a generic queue   //
// ver 1.0                                                              //
// Language:    C#, 2013, .Net Framework 4.5                            //
// Platform:    Macbook Pro, Win 7.0                                    //
// Application: CSE681, Project #4, Fall 2014 
//     
// Author:      Isira Samarasekera (315)5717375, issamara@syr.edu       //
//////////////////////////////////////////////////////////////////////////
/*
 * Module Operations:
 * ------------------
 * This module defines the following class:
 *     BlockingQueue: Provides thread safe enQ, dQ and size methods on a generic queue
 */
/* Required Files:
 *   BlockingQueue.cs 
 *   
 * Build command:
 *   csc  BlockingQueue.cs 
 *                      
 * Maintenance History:
 * --------------------
 * ver 1.0 : 19 Nov 2014
 * - first release
 * 
 * Note:
 *  BlockingQueue doesnt contain a main function as its a generic class.
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DependencyAnalyzer
{
    public class BlockingQueue<T>
    {
        private Queue<T> blockingQ;
        object locker_ = new object();


        //----< constructor >--------------------------------------------

        public BlockingQueue()
        {
            blockingQ = new Queue<T>();
        }

        //----< enqueue a string >---------------------------------------
        public void enQ(T msg)
        {
            lock (locker_)  // uses Monitor
            {
                blockingQ.Enqueue(msg);
                Monitor.Pulse(locker_);
            }
        }

        //----< dequeue a T >---------------------------------------
        public T deQ()
        {
            T msg = default(T);
            lock (locker_)
            {
                while (this.size() == 0)
                {
                    Monitor.Wait(locker_);
                }
                msg = (T)blockingQ.Dequeue();
                return msg;
            }
        }


        //----< return number of elements in queue >---------------------
        public int size()
        {
            int count;
            lock (locker_) { count = blockingQ.Count; }
            return count;
        }
        //----< purge elements from queue >------------------------------

    }

}
