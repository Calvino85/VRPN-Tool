/* ========================================================================
 * PROJECT: UART
 * ========================================================================
 * Portions of this work are built on top of VRPN which was developed by
 *   Russell Taylor
 *   University of North Carolina
 * http://www.cs.unc.edu/Research/vrpn/
 *
 * We acknowledge the CISMM project at the University of North Carolina at Chapel Hill, supported by NIH/NCRR
 * and NIH/NIBIB award #2P41EB002025, for their ongoing  * support and maintenance of VRPN.
 *
 * Portions of this work are also built on top of the VideoWrapper,
 * a BSD licensed video access library for MacOSX and Windows.
 * VideoWrapper is available at SourceForge via 
 * http://sourceforge.net/projects/videowrapper/
 *
 * Copyright of VideoWrapper is
 *     (C) 2003-2010 Georgia Tech Research Corportation
 *
 * Copyright of the new and derived portions of this work
 *     (C) 2010 Georgia Tech Research Corporation
 *
 * This software released under the Boost Software License 1.0 (BSL1.0), so as to be 
 * compatible with the VRPN software distribution:
 *
 * Permission is hereby granted, free of charge, to any person or organization obtaining a copy 
 * of the software and accompanying documentation covered by this license (the "Software") to use, 
 * reproduce, display, distribute, execute, and transmit the Software, and to prepare derivative 
 * works of the Software, and to permit third-parties to whom the Software is furnished to do so,
 * all subject to the following:
 *
 * The copyright notices in the Software and this entire statement, including the above license grant,
 * this restriction and the following disclaimer, must be included in all copies of the Software, in
 * whole or in part, and all derivative works of the Software, unless such copies or derivative works
 * are solely in the form of machine-executable object code generated by a source language processor.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT
 * LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT.
 * IN NO EVENT SHALL THE COPYRIGHT HOLDERS OR ANYONE DISTRIBUTING THE SOFTWARE BE LIABLE FOR ANY DAMAGES OR 
 * OTHER LIABILITY, WHETHER IN CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *
 * For further information regarding UART, please contact 
 *   Blair MacIntyre
 *   <blair@cc.gatech.edu>
 *   Georgia Tech, School of Interactive Computing
 *   85 5th Street NW
 *   Atlanta, GA 30308
 *
 * For further information regarding VRPN, please contact 
 *   Russell M. Taylor II
 *   <taylor@cs.unc.edu>
 *   University of North Carolina, 
 *   CB #3175, Sitterson Hall,
 *   Chapel Hill, NC 27599-3175
 *
 * ========================================================================
 ** @author   Alex Hill (ahill@gatech.edu)
 *  @modified by    Andr�s Roberto G�mez (and-gome@uniandes.edu.co)
 * ========================================================================
 *
 * VRPNConfig.cs
 *
 * Usage: Add this script to the main camera
 * 
 * ServerAddress - use an existing server instead of starting one
 * ConfigFile    - VRPN configuration file to use
 * ShowDebug     - redirects Console output to a text box onscreen
 *
 * Notes:
 * the MotionNode console server must be manually started before use
 * see the installation documentation provided with MotionNode for details
 *
 * a configuration file is created at Assets/parsed_vrpn.cfg from the given config file
 * - only those trackers active in the scene are included in the parsed config file
 * the VRPNTrackerConfig.cs script is automatically generated to provide dropdown lists for tracker names
 * - play the scene after adding new tracker names to vrpn.cfg to regenerate the VRPNTrackerConfig.cs script
 * - changes in VRPNTracker dropdowns will appear after running a second time or reimporting VRPNTrackerConfig.cs
 *
 * ========================================================================*/

using UnityEngine;
using System.Collections; 
using System;
using System.Runtime.InteropServices;
using System.IO;

public class VRPNManager : MonoBehaviour 
{   

	[ StructLayout( LayoutKind.Sequential )]
    [Serializable]
    public struct TimeVal
	{
		public UInt32 tv_sec;
		public UInt32 tv_usec;
	}

    //Serializable time report (UInt32 is not serializable)
    [Serializable]
    public struct TimeValNew
    {
        public int tv_sec;
        public int tv_usec;
    }

    public string ServerAddress = "";
	public string ConfigFile = "vrpn.cfg";
	public static bool debug_flag = false;
	public bool ShowDebug = false;
	public string[] ConfigLines;
	public static bool initialized = false;
	
	private StringWriter debug_writer;
	private string[] debug_text;
	private int debug_index = 0;
	private int debug_buffer_pos = 0;
	private int debug_buffer_max = 500;
	private int debug_lines = 8;
		   
	[DllImport ("VRPNWrapper")]
	private static extern void VRPNServerStart(string file, string location);

	[DllImport ("VRPNWrapper")]
	private static extern void VRPNServerStop();

	[DllImport ("VRPNWrapper")]
	private static extern void VRPNServerLoop();
	
	[DllImport ("VRPNWrapper")]
	private static extern void VRPNSetOutput(int handle);
	
	void Start()
	{
        //Debug.Log("VRPNManager: Start");
        //create parsed vrpn config file from ConfigFile and store as configuration lines
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
			ParseConfigFile();
			
		// Set up debug window
		if (ShowDebug) 
		{
		 	debug_text = new string[debug_buffer_max];
			debug_writer = new StringWriter();
		 	Console.SetOut(debug_writer);
		}
	 	debug_flag = ShowDebug;
		
	 	//create vrpn config file of active trackers from configuration lines
		string filepath = Plugins.ApplicationDataPath() + "/unity_vrpn.cfg";
		CreateConfigFile(filepath);
        
        //Right now there are problems with the local server (started by the app), so empty server address is not allowed
        if (ServerAddress.Equals(""))
        {
            ServerAddress = "localhost";
        }

		// Start Server (or get access to external server)
		VRPNServerStart(filepath,ServerAddress);
	 	initialized = true;
	}
	
	void OnDisable()
	{
		//Debug.Log("VRPNManager: OnDisable");
		// TODO: investigate why the server doesn't shut down properly
		VRPNServerStop();
		if (ShowDebug) 
		{
			StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput());
			Console.SetOut(standardOutput);
		}
	}
	
	void Update()
	{
		//Debug.Log("VRPNManager: Update");	
		VRPNServerLoop();
		if (ShowDebug) 
		{
			debug_writer.Flush();
			string buffer = debug_writer.ToString();
			if (buffer.Length > 0) 
			{
   			 	StringReader strReader = new StringReader(buffer);
   			 	string line;
				while(true) 
				{
					line = strReader.ReadLine();
					if (line == null || line.Length == 0)
						break;
					debug_text[debug_buffer_pos] = line;
					debug_buffer_pos++;
					if (debug_index < 0 && debug_index > debug_lines - debug_buffer_max)
						debug_index--;
					if (debug_buffer_pos >= debug_buffer_max)
						debug_buffer_pos = 0;
				}
				debug_writer.Close();
				debug_writer = new StringWriter();
				Console.SetOut(debug_writer);
			}
			//if ( Time.time - ((int)(Time.time * 10.0f))/10.0f <= Time.deltaTime )
			//	Console.WriteLine(System.String.Format("Debug Seconds: {0:F1}", Time.time));
		}
	}
	
	void ParseConfigFile()
	{
		//Debug.Log("VRPNManager: ParseConfigFile");		
		int index = 0;
		ArrayList potential_names = new ArrayList();
		try
		{
			//recreate config file from txt file in Assets/Resources
  			string filepath = Plugins.CreateFileFromAsset(ConfigFile);
			//print ("configuration read file: " + filepath);
			FileStream reads = new FileStream(filepath, System.IO.FileMode.Open, FileAccess.Read);
			StreamReader sr = new StreamReader(reads);
			string line = sr.ReadLine();
			
			bool write = false;
			while ((line = sr.ReadLine()) != null) 
			{
				if (write && (line.StartsWith("#") || line == ""))
					write = false;
				bool active = line.StartsWith("vrpn_");
				if (active || line.StartsWith("#vrpn_"))
				{
					string[] splitResult = line.Split((char[])null,StringSplitOptions.RemoveEmptyEntries);
					if (!potential_names.Contains(splitResult[1]))
						potential_names.Add(splitResult[1]);
					if (active)
					{
						try
						{
							Enum.Parse(typeof(Tracker_Types),splitResult[0]);
							write = true;
						} catch {}
					}
				}
				if (write)
				{
					string[] holder = ConfigLines;
					ConfigLines = new string[index+1];
					Array.Copy(holder,ConfigLines,index);
					ConfigLines[index++] = line;
				}
			}				
			reads.Close(); 
			bool changed = false;
			index = 0;
			potential_names.Sort();
			foreach(string s in potential_names)
			{
				try
				{
					if (index != (int)Enum.Parse(typeof(VRPNDeviceConfig.Device_Names),s))
						changed = true;
				} catch
				{
					changed = true;
				}
				index++;
			}
			if (changed)
			{
				//Debug.Log("updating device names in VRPNDeviceConfig.cs");
				string filename = Application.dataPath + "/../Assets/Plugins/VRPNWrapper/VRPNDeviceConfig.cs";
				FileStream write_stream = new FileStream(filename, System.IO.FileMode.Create, FileAccess.Write);
				StreamWriter sw = new StreamWriter(write_stream);
				sw.WriteLine("using System;");
				sw.WriteLine("public class VRPNDeviceConfig");
				sw.WriteLine("{");
				sw.Write("	public enum Device_Names { ");
				bool first = true;
				foreach(string s in potential_names)
				{
					if (!first)
						sw.Write(",");
					sw.Write("\n		" + s);
					first = false;
				}
				sw.Write(" };\n");
				sw.WriteLine("}");
				sw.Flush();
				sw.Close();
			}
		}
		catch (FileNotFoundException e)
		{
			throw(e);
		}
	}
	
	void CreateConfigFile(string file)
	{
		//Debug.Log("VRPNManager: CreateConfigFile");
		try
		{
			FileStream write_stream = new FileStream(file, System.IO.FileMode.Create, FileAccess.Write);
			StreamWriter sw = new StreamWriter(write_stream);
			for (int index=0; index<ConfigLines.Length; index++) 
			{
				if (ConfigLines[index].StartsWith("vrpn_"))
				{
					string[] splitResult = ConfigLines[index].Split((char[])null,StringSplitOptions.RemoveEmptyEntries);
					foreach (VRPNTracker tracker in FindObjectsOfType(typeof(VRPNTracker)))
					{
						if (tracker.enabled && splitResult[0] == tracker.TrackerType.ToString() && splitResult[1] == tracker.TrackerName.ToString())
						{
							sw.WriteLine(ConfigLines[index]);
							break;
						}
					}
				}
				else
					sw.WriteLine(ConfigLines[index]);
			}
			sw.Flush();
			sw.Close();
		}
		catch (FileNotFoundException e)
		{
			throw(e);
		}
	}

	void OnGUI ()
	{
		//Debug.Log("VRPNManager: OnGUI");	
		if (ShowDebug) 
		{
			GUI.skin.box.alignment = TextAnchor.LowerLeft;
			debug_index = (int)(GUI.VerticalSlider(new Rect(395, 10, 10, (debug_lines-1)*15+10),
									(float)debug_index, debug_lines-debug_buffer_max, 0));
			string line = "";
			for (int i=debug_lines; i>0; i--) 
			{
				int index = debug_buffer_pos + debug_index - i;
				if (index < 0)
					index += debug_buffer_max;
				line += debug_text[index];
				if (i > 1)
					line += "\n";
			}
			GUI.Box(new Rect(10, 10, 400, (debug_lines-1)*15+10), line);					
		}
	}
	
	
	public static bool TimeValGreater(ref TimeVal tv1, ref TimeVal tv2)
	{
		//Debug.Log("VRPNManager: TimeValGreater");

		
		if (tv1.tv_sec > tv2.tv_sec) 
			return true;
		if ((tv1.tv_sec == tv2.tv_sec) && (tv1.tv_usec > tv2.tv_usec)) 
			return true;
		return false;
   }
   
   public enum Tracker_Types {
        vrpn_Tracker_RazerHydra
        //vrpn_3DConnexion_Navigator,
        //vrpn_3DConnexion_SpaceBall5000,
        //vrpn_3DConnexion_SpaceExplorer,
        //vrpn_3DConnexion_SpaceMouse,
        //vrpn_3DConnexion_Traveler,
        //vrpn_3DMicroscribe,
        //vrpn_5dt,
        //vrpn_5dt16,
        //vrpn_Analog_USDigital_A2,
        //vrpn_Auxiliary_Logger_Server_Generic,
        //vrpn_Button_5DT_Server,
        //vrpn_Button_NI_DIO24,
        //vrpn_Button_PinchGlove,
        //vrpn_Button_Python,
        //vrpn_Button_SerialMouse,
        //vrpn_CerealBox,
        //vrpn_Dial_Example,
        //vrpn_DirectXFFJoystick,
        //vrpn_DirectXRumblePad,
        //vrpn_GlobalHapticsOrb,
        //vrpn_Imager_Stream_Buffer,
        //vrpn_ImmersionBox,
        //vrpn_JoyFly,
        //vrpn_Joylin,
        //vrpn_Joystick,
        //vrpn_Joywin32,
        //vrpn_Keyboard,
        //vrpn_Magellan,
        //vrpn_Mouse,
        //vrpn_National_Instruments,
        //vrpn_NI_Analog_Output,
        //vrpn_nikon_controls,
        //vrpn_Phantom,
        //vrpn_Poser_Analog,
        //vrpn_Radamec_SPI,
        //vrpn_raw_SGIBox,
        //vrpn_SGIBOX,
        //vrpn_Spaceball,
        //vrpn_Tek4662,
        //vrpn_TimeCode_Generator,
        //vrpn_Tng3,
        //vrpn_Tracker_3DMouse,
        //vrpn_Tracker_3Space,
        //vrpn_Tracker_AnalogFly,
        //vrpn_Tracker_ButtonFly,
        //vrpn_Tracker_Crossbow,
        //vrpn_Tracker_DTrack,
        //vrpn_Tracker_Dyna,
        //vrpn_Tracker_Fastrak,
        //vrpn_Tracker_Flock,
        //vrpn_Tracker_Flock_Parallel,
        //vrpn_Tracker_GPS,
        //vrpn_Tracker_InterSense,
        //vrpn_Tracker_Liberty,
        //vrpn_Tracker_MotionNode,
        //vrpn_Tracker_NULL,
        //vrpn_Tracker_PhaseSpace,
        //vrpn_VPJoystick,
        //vrpn_Wanda,
        //vrpn_WiiMote,
        //vrpn_XInputGamepad,
        //vrpn_Xkeys_Desktop,
        //vrpn_Xkeys_Jog_And_Shuttle,
        //vrpn_Xkeys_Joystick,
        //vrpn_Xkeys_Pro,
        //vrpn_Zaber
    }; 
  
	public enum Button_Types {
        vrpn_Mouse,
        vrpn_3DConnexion_Navigator,
        vrpn_Tracker_RazerHydra
        //vrpn_Button_5DT_Server,
        //vrpn_Button_NI_DIO24,
        //vrpn_Button_PinchGlove,
        //vrpn_Button_Python,
        //vrpn_Button_SerialMouse,
        //vrpn_WiiMote,
        //vrpn_XInputGamepad
    };
	
	public enum Analog_Types
	{
        vrpn_Mouse,
        vrpn_3DConnexion_Navigator,
        vrpn_Tracker_RazerHydra
        //vrpn_WiiMote,
        //vrpn_XInputGamepad
    };

}
