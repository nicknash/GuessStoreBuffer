# Guessing Store Buffer Capacity

This repo contains some code that can be used for observing the latency effect of a full store-buffer on x86/x64.

The way I have rigged this up is a bit of a Heath Robinson arrangement: A C# program generates some assembly inside a C++ program.
A python script can then be used to repeatedly invoke this, to generate measurements.
