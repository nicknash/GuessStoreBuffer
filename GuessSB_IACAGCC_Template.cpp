#include <iostream>
#include <fstream>
#include <sys/time.h>
#include "iaca-mac/iacaMarks.h"


double time_diff_millis(struct timeval start_time, struct timeval end_time)
{
	return 1000 * (end_time.tv_sec - start_time.tv_sec) + (end_time.tv_usec - start_time.tv_usec) / 1000.0;
}

int main()
{
	using namespace std;
	long *p = new long[1024 << 7];

	for (int i = 0; i < 1024 << 7; ++i)
	{
		p[i] = i;
	}

	struct timeval start;
	gettimeofday(&start, NULL);
	const int numTrials = 10000000;
	for (int i = 0; i < numTrials; ++i)
	{
		IACA_START
		// REPLACE-TOKEN
	}
	IACA_END
	struct timeval end;
	gettimeofday(&end, NULL);
	double micsPerIteration = time_diff_millis(start, end) / numTrials * 1e3;
	cout << micsPerIteration << endl;
	return 0;
}