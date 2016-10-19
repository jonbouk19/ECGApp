using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Globalization;
//using lambdas;
using System.Linq;//.Expressions;
using System.Collections.Generic;

public class Calculations : MonoBehaviour {
	
	//Text corresponding to the average heart rate,
	//average amplitude, and average period.
	public Text ahr, aa, ap, asd;
	public GameObject CalculationTexts;
	
	//Our variables relating to the AMPLITUDE
	private double avgAmplitude=0d,
	currentAmplitude=0d,
	amplitudeTotal=0d,    //the sum of the peaks
	amplitudeQuantity=0d, //the quantity of peaks
	peakThreshold = .07d,
	previousPeak=0d;
	private bool peakFound = false;
	
	//Our variables relating to the PERIOD
	private double avgPeriod=0d,
	currentPeriod=0d,
	periodTotal=0d,    //the sum of the periods
	time1=0d,
	time2=0d;
	private int periodQuantity = 0;	  //the quantity of periods
	
	//Our variable relating to the heart rate
	private double avgHeartRate=0d;	
	
	//Our variable relating to the standard deviation
	//private double[] periods = new double [10000];
	private List<double> list = new List<double>();
	private double sumOfSquaresOfDifferences=0d;
	private double standardDeviation=0d;
	//private double avgStandardDeviation=0d;
	private double val=0d;
	
	void Start()
	{
		CalculationTexts.SetActive(false);
	}
	
	public void DisplayCalculationTexts()
	{
		CalculationTexts.SetActive(true);
	}
	
	public void setThreshAmplitude(double chosenAmp) {
		peakThreshold = chosenAmp;
	}
	
	//This method calculates the average amplitude
	//by being incremented each time a peak is found
	//on the graph
	//AVG. AMPLITUDE = SUM OF PEAKS / TOTAL PEAKS
	public void CalculateAverageAmplitude(float x, float y)
	{
		if(y>=peakThreshold&&!peakFound)
		{
			if (y >= previousPeak)
			{
				previousPeak=y;
			}
			else
			{
				peakFound=true;
				
				currentAmplitude = previousPeak;
				amplitudeTotal+=currentAmplitude;
				amplitudeQuantity++;
				
				avgAmplitude = amplitudeTotal / amplitudeQuantity;				
				avgAmplitude = Math.Round(((double)(avgAmplitude)),3);	
				
				if (avgAmplitude.ToString()!="NaN")
				{
					aa.text = avgAmplitude.ToString();
				}
				
				//We can utilize this method to its fullest by also
				//helping period calculate itself.				
				CalculateAveragePeriod(x);
				
				//Now reset everything
				previousPeak=0d;
				currentAmplitude=0d;
			}
		}
		//Otherwise, if the graph falls below the peakThreshold after we found a peak,
		//reset it so it is ready to find another peak.
		else if (y<peakThreshold&&peakFound)
		{
			peakFound=false;			
		}		
	}
	
	
	//This method calculates the average period
	//AVG. PERIOD = SUM OF TIME BETWEEN TWO PEAKS / TOTAL TIME BETWEEN TWO PEAKS
	public void CalculateAveragePeriod(float timeTwo)
	{
		time2=timeTwo;
		
		//First amplitude hasn't happened yet in this case.
		if(time1==0d)
		{
			time1=timeTwo;
			return;
		}
		else
		{
			currentPeriod = time2 - time1;
			
			// store each period value into an array
			// useful for standard deviation calculation
			currentPeriod = Math.Round (((double)(currentPeriod)),3);
			//periods[periodQuantity] = currentPeriod;
			list.Add(currentPeriod);
			
			periodTotal+=currentPeriod;
			periodQuantity++;
			
			avgPeriod = periodTotal / periodQuantity;				
			avgPeriod = Math.Round(((double)(avgPeriod)),3);
			
			if (avgPeriod.ToString()!="NaN")
			{
				ap.text = avgPeriod.ToString();
			}
			
			CalculateAverageHeartRate();
			
			//Now shift the time values.
			time1=time2;
		}		
	}
	
	//This method calculates the average heart rate
	//by being called each time a line is drawn
	//on the graph.
	//AVG. HEART RATE = SUM OF Y VALUES / TOTAL Y VALUES
	public void CalculateAverageHeartRate()
	{
		avgHeartRate = Math.Round(((double)(60d/avgPeriod)),3);
		
		if (avgHeartRate.ToString()!="NaN")
		{
			ahr.text = avgHeartRate.ToString();
		}
		CalculateAverageStandardDeviation();
	}
	
	public void CalculateAverageStandardDeviation()
	{
		if (periodQuantity > 1) {
			double sumOfDerivation = 0d;
			foreach (double value in list)  
			{  
				sumOfDerivation += (value) * (value);  
			}  
			double sumOfDerivationAverage = sumOfDerivation / (list.Count - 1);  
			standardDeviation = Math.Sqrt(sumOfDerivation/(list.Count-1));  
			if (sumOfDerivationAverage.ToString() != "NaN") {
				asd.text = sumOfDerivationAverage.ToString();
			}
		}
	}
	
}
