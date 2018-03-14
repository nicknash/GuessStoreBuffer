#!/usr/bin/env python
import os

results = open("results.csv", "w")
results.write("NumStores,MicrosecondsPerIteration\n")
for numStores in range(0, 90):
    print "Building with num stores = %(numStores)s"%vars()
    os.system("dotnet run %(numStores)s"%vars())
    os.system("g++ GuessSB.cpp -O3")
    for micsPerIteration in os.popen("./a.out"):
        results.write("%(numStores)s,%(micsPerIteration)s\n"%vars())
results.close()
