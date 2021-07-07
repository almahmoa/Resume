#include "simd.p5.h"

#ifndef ARRAYSIZE
#define ARRAYSIZE		5000000
#endif

#ifndef NUM_TRIES
#define NUM_TRIES		20
#endif          

float A[ARRAYSIZE], B[ARRAYSIZE], C[ARRAYSIZE];

int main()
{
#ifndef _OPENMP
    fprintf(stderr, "No OpenMP support!\n");
    return 1;
#endif

    int NUMT = 1;
    float maxMegaMultsPerSecond = 0;
    float maxMegaMultsPerSecond2 = 0;
    int NUM_ELEMENTS_PER_CORE = 0;

    double time4 = omp_get_wtime();
    {
        SimdMul(A, B, C, ARRAYSIZE);
    }
    double time5 = omp_get_wtime();
    double time6 = omp_get_wtime();
    {
        NonSimdMul(A, B, C, ARRAYSIZE);
    }
    double time7 = omp_get_wtime();

    double megaMultsPerSecond3 = (float)ARRAYSIZE / (time5 - time4) / 1000000.;
    double megaMultsPerSecond4 = (float)ARRAYSIZE / (time7 - time6) / 1000000.;
    fprintf(stderr, "%d %6.2lf %6.2lf\n", 0, megaMultsPerSecond3, megaMultsPerSecond4);

    for (int g = 0; g < 3; g++)
    {
        omp_set_num_threads(NUMT);
        maxMegaMultsPerSecond = 0;
        maxMegaMultsPerSecond2 = 0;
        NUM_ELEMENTS_PER_CORE = ARRAYSIZE / NUMT;
        for (int t = 0; t < NUM_TRIES; t++)
        {
            double time0 = omp_get_wtime();
#pragma omp parallel
            {
                int first = omp_get_thread_num() * NUM_ELEMENTS_PER_CORE;
                SimdMul(&A[first], &B[first], &C[first], NUM_ELEMENTS_PER_CORE);
            }
            double time1 = omp_get_wtime();
            double time2 = omp_get_wtime();
#pragma omp parallel
            {
                int first = omp_get_thread_num() * NUM_ELEMENTS_PER_CORE;
                NonSimdMul(&A[first], &B[first], &C[first], NUM_ELEMENTS_PER_CORE);
            }
            double time3 = omp_get_wtime();

            double megaMultsPerSecond = (float)ARRAYSIZE / (time1 - time0) / 1000000.;
            if (megaMultsPerSecond > maxMegaMultsPerSecond)
                maxMegaMultsPerSecond = megaMultsPerSecond;
            double megaMultsPerSecond2 = (float)ARRAYSIZE / (time3 - time2) / 1000000.;
            if (megaMultsPerSecond2 > maxMegaMultsPerSecond2)
                maxMegaMultsPerSecond2 = megaMultsPerSecond2;
        }

        fprintf(stderr, "%d %6.2lf %6.2lf\n", NUMT, maxMegaMultsPerSecond, maxMegaMultsPerSecond2);
        NUMT += NUMT;
    }
}