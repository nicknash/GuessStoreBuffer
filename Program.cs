using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace GuessStoreBuffer
{
    class Program
    {
        static void Main(string[] args)
        {
            var target = new int[100];
            long oneMillisecondInTicks = Stopwatch.Frequency/1000;
            long y = 0;
            var results = File.CreateText("timings.csv");
            results.WriteLine("numStores,TimeMicroseconds,MicrosecondsPerStore");
            for(int i = 0; i < target.Length; ++i)
            {
                y += target[i];
            }
            for (int numStores = 20; numStores < 80; ++numStores) // 42 entry store buffer on haswell.
            {
                long begin = Stopwatch.GetTimestamp();    
                /*int numTrials = 500000;
                // MAYBE HINTS at 42?
                for (int i = 0; i < numTrials; ++i)
                {
                    for (int j = 0; j < numStores; ++j)
                    {
                        target[j] = numStores;
                    }
                    for (int j = 0; j < 1500; ++j)
                    {
                        y += numStores ^ j + i;
                    }
                }
                */

                /*
                // Maybe also hints at 42 
                int numTrials = 10000000;
                for (int i = 0; i < numTrials; ++i)
                {
                    for (int j = 0; j < numStores; ++j)
                    {
                        target[j] = numStores;
                    }
                    for (int j = 0; j < 15; ++j)
                    {
                        y += numStores ^ j + i;
                    }
                }
                */        

                
                // 30,31,32,33-notch     
                int numTrials = 10000000;
                for (int i = 0; i < numTrials; ++i)
                {
                    for (int j = 0; j < numStores; ++j)
                    {
                        target[0] = numStores;
                    }
                    for (int j = 0; j < 150; ++j)
                    {
                        y += numStores ^ j + i;
                    }
                }
                
           
                long end = Stopwatch.GetTimestamp();
                var totalTime = (end - begin)/(double)Stopwatch.Frequency;
                Console.WriteLine($"numStores={numStores} average time to complete = {totalTime/numTrials}, target[2] = {target[2]}, y = {y}");
                var avgIterationTime = totalTime/numTrials*1e6;
                var avgStoreTime = avgIterationTime / numStores;
                results.WriteLine($"{numStores},{avgIterationTime},{avgStoreTime}");
            }
            results.Close();
        }
    }
}
