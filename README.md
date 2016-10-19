# ECGApp

During my senior year, I worked with an electrical engineer to create a low cost ECG alternative for fish embryo.

If you download and extract ECGApp.zip, there is an executable you can run. It is important that the executable and the other directory remain in the same location relative to each other. However, a desktop shortcut could be created for the app if desired.

You can either run the app with the executable located inside the zip folder or if you download Unity you can run the Unity scene file called Main. One added advantage of running the Unity file is to take advantage of the built in frame by frame playback.

For a demo, you can run the app with file name "a", threshold 0.3, and speed 3. The app will graph the ECG and calculate statistics which update simultaneously. The graph is always zoomed in at a three second time interval. One advantage of using Unity is the ability for a programmable camera view. Therefore, it is possible to adjust the user view by moving the camera view with respect to time after the initial three seconds. Once the graphing is complete, the user can scroll to see the entire graph.

The Unity Framework with C# was used to create the app due to its optimized graphics, programmable camera view, user interface, scripting API, and frame by frame playback. Also, Unity is 100% free and is a very popular framework used for creating gaming apps. One notable feature is the ability to create one app and export it to iPhone, Android, Blackberry, Windwos, Mac, and Linux.

The app is implemented with one class to perform the calculations (Calculations) and one class for handling the data (DataHandler). The DataHandler handles the functionality and uses Calculations to perform the calculations for the statstics. Once the user clicks the "Start Graph" button or presses enter, StartGraph in DataHandler is called which then calles the coroutine Graph. Within this method, the methods of the Calculations class are called. The code is commeneted to explain its functionality in further depth.
