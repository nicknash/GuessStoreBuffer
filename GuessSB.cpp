#include <iostream>
#include <fstream>
#include <sys/time.h>

double time_diff_millis(struct timeval start_time, struct timeval end_time)
{
    return 1000*(end_time.tv_sec - start_time.tv_sec) + (end_time.tv_usec - start_time.tv_usec) / 1000.0;
}

int main()
{
	using namespace std;
	long* p = new long[1024];

	for(int i = 0; i < 1024; ++i)
	{
		p[i] = i;
	}

	long pa = (long) p;
	long *q = (long*) (pa + (128 - (pa & 0x7F)));


	ofstream results;
  	results.open("timings.csv", ios::out);
	results << "NumStores,TimeInMicroseconds" << endl;
	
	for (int numStores = 20; numStores < 88; ++numStores) // On my Haswell (42 entry store buffer) this produces a clear latency spike at numStores == 43
    {
    	struct timeval start;
    	gettimeofday(&start, NULL);
		const int numTrials = 1000000;
        for (int i = 0; i < numTrials; ++i)
        {
        	for (int j = 0; j < numStores; ++j)
            {
            	q[j] = j;
            }
            for (int j = 0; j < 550; ++j)
            {
            	asm("nop");
            }
        }
 		struct timeval end;
    	gettimeofday(&end, NULL);
    	double micsPerIteration = time_diff_millis(start, end)/numTrials * 1e3;
        long total = 0;
    	for(int i = 0; i < 1024; ++i)
    	{	
    		total += p[i];
    	}
    	cout << "NumStores=" << numStores << " total = " << total << " micsPerIteration = " << micsPerIteration << endl;
    	results << numStores << "," << micsPerIteration << endl;
	}
	results.flush();
	results.close();
	return 0;
}