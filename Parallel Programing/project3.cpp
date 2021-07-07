#include <iostream>
#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <cmath>
#include <omp.h>

unsigned int seed = 0;

omp_lock_t	Lock;
int		NumInThreadTeam;
int		NumAtBarrier;
int		NumGone;

int	NowYear;		// 2021 - 2026
int	NowMonth;		// 0 - 11

float	NowPrecip;		// inches of rain per month
float	NowTemp;		// temperature this month
float	NowHeight;		// grain height in inches
int	    NowNumDeer;		// number of deer in the current population
int     NowNumSasquatch;
float   sasquatchMagic = 0;

const float GRAIN_GROWS_PER_MONTH = 9.0;
const float ONE_DEER_EATS_PER_MONTH = 1.0;
const int ONE_SASQUATCH_EATS_MEAT_PER_MONTH = 1;
const float ONE_SASQUATCH_EATS_GRAIN_PER_MONTH = 1.75;

const float AVG_PRECIP_PER_MONTH = 7.0;	// average
const float AMP_PRECIP_PER_MONTH = 6.0;	// plus or minus
const float RANDOM_PRECIP = 2.0;	// plus or minus noise

const float AVG_TEMP = 60.0;	// average
const float AMP_TEMP = 20.0;	// plus or minus
const float RANDOM_TEMP = 10.0;	// plus or minus noise

const float MIDTEMP = 40.0;
const float MIDPRECIP = 10.0;

// print debugging messages?
#ifndef DEBUG
#define DEBUG	false
#endif

#define M_PI 3.14159265358979323846264338327950288

// function prototype
void	InitBarrier(int);
void	WaitBarrier();
void Deer();
void Grain();
void Watcher();
void Sasquatch();

float
SQR(float x)
{
    return x * x;
}

float
Ranf(unsigned int* seedp, float low, float high)
{
    float r = (float)rand();              // 0 - RAND_MAX

    return(low + r * (high - low) / (float)RAND_MAX);
}


int
Ranf(unsigned int* seedp, int ilow, int ihigh)
{
    float low = (float)ilow;
    float high = (float)ihigh + 0.9999f;

    return (int)(Ranf(seedp, low, high));
}

int main(int argc, char* argv[])
{
#ifndef _OPENMP
    fprintf(stderr, "No OpenMP support!\n");
    return 1;
#endif
    // starting date and time:
    NowMonth = 0;
    NowYear = 2021;

    // starting state (feel free to change this if you want):
    NowNumDeer = 1;
    NowNumSasquatch = 0;
    NowHeight = 1.;

    //The temperature and precipitation are a function of the particular month: 
    float ang = (30. * (float)NowMonth + 15.) * (M_PI / 180.);

    float temp = AVG_TEMP - AMP_TEMP * cos(ang);
    NowTemp = temp + Ranf(&seed, -RANDOM_TEMP, RANDOM_TEMP);

    float precip = AVG_PRECIP_PER_MONTH + AMP_PRECIP_PER_MONTH * sin(ang);
    NowPrecip = precip + Ranf(&seed, -RANDOM_PRECIP, RANDOM_PRECIP);
    if (NowPrecip < 0.)
        NowPrecip = 0.;

    InitBarrier(4);

    omp_set_num_threads(4);	// same as # of sections
#pragma omp parallel sections
    {
#pragma omp section
        {
            Deer();
        }

#pragma omp section
        {
            Grain();
        }

#pragma omp section
        {
            Watcher();
        }

#pragma omp section
        {
            Sasquatch();	// Big Foot
        }
    }    // implied barrier -- all functions must return in order
        // to allow any of them to get past here
}


// have the calling thread wait here until all the other threads catch up:
void
WaitBarrier()
{
    omp_set_lock(&Lock);
    {
        NumAtBarrier++;
        if (NumAtBarrier == NumInThreadTeam)
        {
            NumGone = 0;
            NumAtBarrier = 0;
            // let all other threads get back to what they were doing
// before this one unlocks, knowing that they might immediately
// call WaitBarrier( ) again:
            while (NumGone != NumInThreadTeam - 1);
            omp_unset_lock(&Lock);
            return;
        }
    }
    omp_unset_lock(&Lock);

    while (NumAtBarrier != 0);	// this waits for the nth thread to arrive

#pragma omp atomic
    NumGone++;			// this flags how many threads have returned
}

void Deer() {
    while (NowYear < 2027)
    {
        // compute a temporary next-value for this quantity
        // based on the current state of the simulation:
        int nextNumDeer = NowNumDeer;
        nextNumDeer -= NowNumSasquatch * ONE_SASQUATCH_EATS_MEAT_PER_MONTH;
        int carryingCapacity = (int)(NowHeight);
        if (nextNumDeer < carryingCapacity)
            nextNumDeer++;
        else
            if (nextNumDeer > carryingCapacity)
                nextNumDeer--;

        if (nextNumDeer < 0)
            nextNumDeer = 0;

            // DoneComputing barrier:
            WaitBarrier();
            NowNumDeer = nextNumDeer;

            // DoneAssigning barrier:
            WaitBarrier();
            //do nothing

            // DonePrinting barrier:
            WaitBarrier();
    }
}

void Grain() {
    while (NowYear < 2027)
    {
        // compute a temporary next-value for this quantity
        // based on the current state of the simulation:
        float tempFactor = exp(-SQR((NowTemp - MIDTEMP) / 10.));

        float precipFactor = exp(-SQR((NowPrecip - MIDPRECIP) / 10.));

        float nextHeight = NowHeight;
        nextHeight += tempFactor * precipFactor * GRAIN_GROWS_PER_MONTH;
        nextHeight -= ((float)NowNumDeer * ONE_DEER_EATS_PER_MONTH);
        nextHeight -= ((float)NowNumSasquatch * ONE_SASQUATCH_EATS_GRAIN_PER_MONTH);

        //Be sure to clamp nextHeight against zero, that is :
        if (nextHeight < 0.) nextHeight = 0.;

            // DoneComputing barrier:
            WaitBarrier();
            NowHeight = nextHeight;

            // DoneAssigning barrier:
            WaitBarrier();
            //do nothing

            // DonePrinting barrier:
            WaitBarrier();
    }
}

void  Watcher() {
    while (NowYear < 2027)
    {
        // compute a temporary next-value for this quantity
        // based on the current state of the simulation:
        //do nothing

            // DoneComputing barrier:
            WaitBarrier();
            //do nothing

            // DoneAssigning barrier:
            WaitBarrier();
            // print out result
            if (DEBUG) fprintf(stderr, "%d NowYear : %d NowMonth : %f NowPrecip : %f NowTemp : %f NowHeight : %d NowNumDeer\n\n",
                          NowYear, NowMonth, NowPrecip * 2.54, (5. / 9.) * (NowTemp - 32), NowHeight * 2.54, NowNumDeer);
            else
                fprintf(stderr, "%f %f %f %d %d\n",
                   NowPrecip * 2.54, (5. / 9.) * (NowTemp - 32), NowHeight * 2.54, NowNumDeer, NowNumSasquatch);

            // increment the date
            if (NowMonth == 12)
            {
                NowYear++;
                NowMonth = 0;
            }
            NowMonth++;
            
            //The temperature and precipitation are a function of the particular month: 
            float ang = (30. * (float)NowMonth + 15.) * (M_PI / 180.);

            float temp = AVG_TEMP - AMP_TEMP * cos(ang);
            NowTemp = temp + Ranf(&seed, -RANDOM_TEMP, RANDOM_TEMP);

            float precip = AVG_PRECIP_PER_MONTH + AMP_PRECIP_PER_MONTH * sin(ang);
            NowPrecip = precip + Ranf(&seed, -RANDOM_PRECIP, RANDOM_PRECIP) + sasquatchMagic;
            if (NowPrecip < 0.)
                NowPrecip = 0.;

            // DonePrinting barrier:
            WaitBarrier();
    }
}

void  Sasquatch() { // Sasquatch
    while (NowYear < 2027)
    {
        // compute a temporary next-value for this quantity
        // based on the current state of the simulation:
        
        int nextNumSasquatch = NowNumSasquatch;
        int carryingCapacity = NowNumDeer - 3;
        if (nextNumSasquatch < carryingCapacity)
            nextNumSasquatch++;
        else
            if (nextNumSasquatch > carryingCapacity)
                nextNumSasquatch--;

        if (nextNumSasquatch < 0)
            nextNumSasquatch = 0;

        if (nextNumSasquatch == 1)
            sasquatchMagic = 1.5;
        else
            sasquatchMagic = 0;

            // DoneComputing barrier:
            WaitBarrier();
            NowNumSasquatch = nextNumSasquatch;

            // DoneAssigning barrier:
            WaitBarrier();
        

            // DonePrinting barrier:
            WaitBarrier();
        
    }
}

// specify how many threads will be in the barrier:
//	(also init's the Lock)
void
InitBarrier(int n)
{
    NumInThreadTeam = n;
    NumAtBarrier = 0;
    omp_init_lock(&Lock);
}