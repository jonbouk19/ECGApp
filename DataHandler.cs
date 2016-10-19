using UnityEngine;
using System.Collections;
using System.Text;
using System.IO; 
using UnityEngine.UI;
using Vectrosity;

public class DataHandler : MonoBehaviour {
	
	//Our text file with its associated data
	private TextAsset data;
	
	//The string pulled from our data, initially blank.
	private string dataText="";
	
	//Three string arrays for our data:
	//1.) The x AND y values (one line per element in the array)
	private string[] values;
	
	//2.) The x values (time in ms)
	//Can store up to 50,000 elements
	private string[] xValues = new string[50000];
	
	//3.) The y values (heart-rate in bpm)
	//Can store up to 50,000 elements
	private string[] yValues = new string[50000];
	
	//We need to create an array for the points on our graph:
	//Can store up to 50,000 elements
	private GameObject[] points = new GameObject[50000];
	
	//A generic point we will reuse (All points have the same appearance)
	private GameObject singlePointPrefab;
	
	//A generic line we will reuse (It's iust one line; we'll iust keep extending it)
	private VectorLine myLine;
	public GameObject origin;
	
	//The one button needed to control the program
	public Button startButton;
	
	//UI text for displaying progress to the user
	public Text debugText;
	
	//UI slider element to scroll left/right on graph
	public Slider slider;
	
	//UI Text input element for the user to insert the name of the file
	public InputField input, graphSpeedInput, ampThreshInput;
	
	private bool graphStarted=false;
	
	//So we can dynamically update the x-axis labels.
	public Text currentSecond, previousSecond, previousPreviousSecond;
	
	//Change this variable with caution. The minimum value is 1, meaning
	//EVERY point is graphed from the data. For example, changing it to 5 means
	//it will graph every other FIVE points from the data. The lower the value, the more
	//accurate the data but slower the graphing is. The higher the value, the less
	//accurate the data but faster graphing. Right now, we force the max to be 5.
	private int graphingSpeed=1;
	
	void Start()
	{
		//iust to let the program know what a point is ahead of time
		singlePointPrefab = Resources.Load<GameObject>("point");
		
		//Force the canvas to render via world-space for scaling purposes
		VectorLine.canvas.renderMode = RenderMode.WorldSpace;
		
		//Our initial line should be from the origin to the origin (thus not an actual line)
		myLine = VectorLine.SetLine(Color.green,
		                            new Vector2(origin.transform.position.x,origin.transform.position.y),
		                            new Vector2(origin.transform.position.x,origin.transform.position.y));
		myLine.lineWidth=.015f;	
		
		slider.minValue=0f;
		slider.maxValue=0f;
		slider.gameObject.SetActive(false);
	}
	
	//With all of the points plotted, we can now draw the graph
	//using the following coroutine:
	public void StartGraph()
	{
		int result;
		
		//If the user entered a valid int into the graph speed input:
		if(int.TryParse (graphSpeedInput.text, out result))
		{
			int desiredSpeed = int.Parse(graphSpeedInput.text);
			
			if (desiredSpeed<1)
			{
				graphingSpeed=1;
				graphSpeedInput.text="Graph Speed: 1";
			}
			else if (desiredSpeed>5)
			{
				graphingSpeed=5;
				graphSpeedInput.text="Graph Speed: 5";
			}
			else
			{
				graphingSpeed=desiredSpeed;
				graphSpeedInput.text="Graph Speed: "+desiredSpeed;
			}
		}
		else
		{
			graphingSpeed=1;
			graphSpeedInput.text="Graph Speed: 1";
		}
		
		double result2, desiredThresh;
		
		//If the user entered a valid int into the graph speed input:
		if (double.TryParse (ampThreshInput.text, out result2)) {
			desiredThresh = double.Parse (ampThreshInput.text);
			ampThreshInput.text = "Amp Thresh: " + desiredThresh;
		} 
		else {
			desiredThresh = .07d;
			ampThreshInput.text = "Amp Thresh: 0.07";
		}
		this.GetComponent<Calculations> ().setThreshAmplitude (desiredThresh);
		
		if(input.text=="")
		{
			debugText.text = "Please input filename.";
		}
		else
		{
			//MUST BE LOCATED IN THE UNITY ASSETS/RESOURCES FOLDER!		
			data=(TextAsset)Resources.Load(input.text);
			if (data==null)
			{
				debugText.text = "File not found.";
				return;
			}
			else
			{
				//Pull the text from the data and save it
				//in a string for us to manipulate.
				dataText = data.text;
				
				//First, we split off our values per line
				values = dataText.Split('\n');
				startButton.interactable=false;
				input.interactable=false;
				graphSpeedInput.interactable=false;
				ampThreshInput.interactable=false;
				
				graphStarted=true;
				this.GetComponent<Calculations>().DisplayCalculationTexts();
				
				StartCoroutine(Graph());
			}
		}
		
	}
	
	//For convenience; if the user presses enter it will start the graph (after input is validated)
	void Update()
	{
		if(Input.GetKey(KeyCode.Return)&&!graphStarted)StartGraph();				
	}
	
	IEnumerator Graph()
	{
		//First, we split off our values per line
		values = dataText.Split('\n');
		
		//Plot all of the points on the graph and
		//display it in realtime
		for(int i=0; i<values.Length; i+=graphingSpeed)
		{    
			//To skip the first 3 lines and very last line, we say:
			if (i>2&&i<values.Length-1)
			{			
				int xLength=values[i].IndexOf('\t');			
				if (xLength!=-1)xValues[i] = values[i].Substring(0,xLength);
				
				int yStart = values[i].LastIndexOf('\t')+1;
				int yLength = values[i].Length - yStart;
				if (yLength!=-1)yValues[i] = values[i].Substring(yStart,yLength);
				
				Vector3 pos = new Vector3(float.Parse(xValues[i]),float.Parse(yValues[i]),0f);
				points[i] = Instantiate(singlePointPrefab, pos, new Quaternion()) as GameObject;		
				
				debugText.text = (i+2) + "/" + values.Length;
				
				this.GetComponent<Calculations>().CalculateAverageAmplitude(float.Parse(xValues[i])/1000f,float.Parse(yValues[i]));
				
				//Add another line between the last point and the next.
				myLine.points2.Add (new Vector2((points[i].transform.position.x/1000f),points[i].transform.position.y));
				
				
				if((points[i].transform.position.x/1000f)>3f)
				{
					this.transform.position = new Vector3(this.transform.position.x+
					                                      ((points[i].transform.position.x/1000f)-
					 (points[i-graphingSpeed].transform.position.x/1000f)),
					                                      this.transform.position.y,
					                                      this.transform.position.z);
					
					currentSecond.text = (points[i].transform.position.x/1000f).ToString();
					previousSecond.text = ((points[i].transform.position.x/1000f)-1f).ToString();
					previousPreviousSecond.text = ((points[i].transform.position.x/1000f)-2f).ToString();
					
				}
				
				//If the graph is complete, allow the user to scroll left/right
				if((i+2)==values.Length)
				{
					slider.gameObject.SetActive(true);		
					slider.maxValue=(points[i].transform.position.x/1000f);
					slider.value=slider.maxValue-3f;
				}
				
				myLine.Draw();
				
				yield return null;	
			}							
		}			
	}
	
	//This method simply moves the camera to view the graph
	public void Slide()
	{
		float value = slider.value;
		
		this.transform.position = new Vector3(1.84f+value,
		                                      this.transform.position.y,
		                                      this.transform.position.z);
		
		currentSecond.text = "";
		previousSecond.text = "";
		previousPreviousSecond.text = "";
	}
}
