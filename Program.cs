using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace GuessStoreBuffer
{
    unsafe class Program
    {
        static void Main(string[] args)
        {
            var target = new int[100];
            long y = 0;
            var results = File.CreateText("timings.csv");
            results.WriteLine("numStores,TimeMicroseconds,MicrosecondsPerStore");
            for(int i = 0; i < target.Length; ++i)
            {
                y += target[i];
            }
            fixed (int* p = target) // TODO, try aligning this on a 128-byte boundary
            {
                for (int numStores = 20; numStores < 80; ++numStores) // 42 entry store buffer on haswell.
                {
                    long begin = Stopwatch.GetTimestamp();
                    int numTrials = 10000000;
                    for (int i = 0; i < numTrials; ++i)
                    {
                        for (int j = 0; j < numStores; ++j)
                        {
                            *(p + j) = numStores;
                        }
                        for (int j = 0; j < 150; ++j)
                        {
                            if (i > 10000000)
                            {
                                break;
                            }
                        }
                    }

                    long end = Stopwatch.GetTimestamp();
                    var totalTime = (end - begin) / (double)Stopwatch.Frequency;
                    Console.WriteLine($"numStores={numStores} average time to complete = {totalTime / numTrials}, target[2] = {target[2]}, y = {y}");
                    var avgIterationTime = totalTime / numTrials * 1e6;
                    var avgStoreTime = avgIterationTime / numStores;
                    results.WriteLine($"{numStores},{avgIterationTime},{avgStoreTime}");
                }
            }
            results.Close();
        }
    }
}
