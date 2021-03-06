#include "stdafx.h"
#include <iostream>

int main()
{
	using namespace std;
	long *p = new long[1024 << 7];

	for (int i = 0; i < 1024 << 7; ++i)
	{
		p[i] = i;
	}

	LARGE_INTEGER start;
	QueryPerformanceCounter(&start);
	const int numTrials = 10000000;
	for (int i = 0; i < numTrials; ++i)
	{
		// REPLACE-TOKEN
	}
	LARGE_INTEGER end;
	QueryPerformanceCounter(&end);
	LARGE_INTEGER freq;
	QueryPerformanceFrequency(&freq);
	double micsPerIteration = (end.QuadPart - start.QuadPart) * 1e6/(freq.QuadPart*numTrials);
	cout << micsPerIteration << endl;
	return 0;
}

