#include <stdio.h>
#include <math.h>
#include <iostream>
#include <cmath>
#include <stdlib.h>
#include <omp.h>

#define XMIN     -1.
#define XMAX      1.
#define YMIN     -1.
#define YMAX      1.

#define N	0.70

// print debugging messages?
#ifndef DEBUG
#define DEBUG	false
#endif

// setting the number of threads:
#ifndef NUMT
#define NUMT		    8
#endif

// setting the number of nodes:
#ifndef NUMNODES
#define NUMNODES		1250
#endif

float Height(int, int);	// function prototype

int main(int argc, char* argv[])
{
#ifndef _OPENMP
	fprintf(stderr, "No OpenMP support!\n");
	return 1;
#endif

	omp_set_num_threads(NUMT);	// set the number of threads to use in parallelizing the for-loop:`

		// the area of a single full-sized tile:
		// (not all tiles are full-sized, though)

		float fullTileArea = (((XMAX - XMIN) / (float)(NUMNODES - 1)) *
			((YMAX - YMIN) / (float)(NUMNODES - 1)));
		if (DEBUG)	fprintf(stderr, "fullTileArea = %f \n", fullTileArea);
	// sum up the weighted heights into the variable "volume"
	// using an OpenMP for loop and a reduction:
		float volume = 0;
		double time0 = omp_get_wtime();
		int fullTileHit = 0;
		int sideTileHit = 0;
		int cornerTileHit = 0;

#pragma omp parallel for collapse(2) default(none), reduction(+:volume)
	for (int iv = 0; iv < NUMNODES; iv++)
	{
		for (int iu = 0; iu < NUMNODES; iu++)
		{
			// corner volume
			if ((iv == 0 && iu == 0) || (iv == 0 && iu == NUMNODES - 1) || (iv == NUMNODES - 1 && iu == 0) || (iv == NUMNODES - 1 && iu == NUMNODES - 1))
			{
				volume += (fullTileArea / 4) * Height(iu, iv);
				if (DEBUG) cornerTileHit += 1;
				if (DEBUG)	fprintf(stderr, "CORNER height = %f iu = %d iv %d\n", Height(iu, iv), iu, iv);
			}
			// side volume
			else if (((iv == 0 || iv == NUMNODES - 1) && iu < NUMNODES - 1 && iu != 0) || ((iu == 0 || iu == NUMNODES - 1) && iv < NUMNODES - 1 && iv != 0))
			{
				volume += (fullTileArea / 2) * Height(iu, iv);
				if (DEBUG) sideTileHit += 1;
				if (DEBUG)	fprintf(stderr, "SIDE   height = %f iu = %d iv %d\n", Height(iu, iv), iu, iv);
			}
			// full-sized tile volume
			else
			{
				volume += fullTileArea * Height(iu, iv);
				if (DEBUG) fullTileHit += 1;
				if (DEBUG)	fprintf(stderr, "FULL   height = %f iu = %d iv %d\n", Height(iu, iv), iu, iv);
			}
		}
	}
	if (DEBUG)	fprintf(stderr, "fullTileHit = %d ; sideTileHit = %d ; cornerTileHit = %d\n", fullTileHit, sideTileHit, cornerTileHit);
	volume *= 2;
	double time1 = omp_get_wtime();
	double megaHeightPerSecond = (double)NUMNODES * NUMNODES / (time1 - time0) / 1000.;
	fprintf(stderr, "%2d threads ; %8d numnodes ; volume = %6.2f% ; megaheight/sec = %6.2lf\n",
		NUMT, NUMNODES, volume, megaHeightPerSecond);
}

float Height(int iu, int iv)	// iu,iv = 0 .. NUMNODES-1
{
	float x = -1. + 2. * (float)iu / (float)(NUMNODES - 1);	// -1. to +1.
	float y = -1. + 2. * (float)iv / (float)(NUMNODES - 1);	// -1. to +1.
	float xn = pow(fabs(x), (double)N);
	float yn = pow(fabs(y), (double)N);
	float r = 1. - xn - yn;
	if (r <= 0.)
		return 0.;
	float height = pow(r, 1. / (float)N);
	return height;
}