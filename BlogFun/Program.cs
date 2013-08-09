using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace BlogFun
{
    class Program
    {
        static void Main(string[] args)
        {
            Queue<Post> pubQueue = new Queue<Post>();
            FucCSDN CSDN = new FucCSDN();
            PubPost Publisher = new PubPost();
            ThreadPool.QueueUserWorkItem(new WaitCallback(CSDN.Process), pubQueue);
            ThreadPool.QueueUserWorkItem(new WaitCallback(Publisher.Process), pubQueue);
            Console.WriteLine("AlL Done");
            Console.ReadLine();
        }
    }
}
