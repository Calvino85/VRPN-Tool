/* ========================================================================
 * PROJECT: VRPN Tool
 * ========================================================================
 * 
 * 
 *
 * ========================================================================
 ** @author   Andrés Roberto Gómez (and-gome@uniandes.edu.co)
 *
 * ========================================================================
 *
 * VRPNEdit.cs
 *
 * usage: Must be added once in the scene to enable the VRPNEditor functionality
 * It comes in VRPNEventManager prefab.
 * 
 * inputs:
 *
 * Notes:
 *
 * ========================================================================*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class VRPNEdit : MonoBehaviour {

    //Public properties
    public bool isPlaying = false;

    //Device Dictionaries
    private Dictionary<string, VRPNTrackerRecordings> VRPNTrackerDevice = new Dictionary<string, VRPNTrackerRecordings>();
    private Dictionary<string, VRPNAnalogRecordings> VRPNAnalogDevice = new Dictionary<string, VRPNAnalogRecordings>();
    private Dictionary<string, VRPNButtonRecordings> VRPNButtonDevice = new Dictionary<string, VRPNButtonRecordings>();
    
    // Use this for initialization
    void Start () {
        StartPlaying();
    }
	
	// Update is called once per frame
	void Update () {
        if (isPlaying)
        {
            bool keepPlaying = false;
            foreach (KeyValuePair<string, VRPNTrackerRecordings> pair in VRPNTrackerDevice)
            {
                List<VRPNTrackerRecording>.Enumerator e = pair.Value.recordings.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Update();
                    keepPlaying = keepPlaying || e.Current.isPlaying;
                }
            }
            foreach (KeyValuePair<string, VRPNAnalogRecordings> pair in VRPNAnalogDevice)
            {
                List<VRPNAnalogRecording>.Enumerator e = pair.Value.recordings.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Update();
                    keepPlaying = keepPlaying || e.Current.isPlaying;
                }
            }
            foreach (KeyValuePair<string, VRPNButtonRecordings> pair in VRPNButtonDevice)
            {
                List<VRPNButtonRecording>.Enumerator e = pair.Value.recordings.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Update();
                    keepPlaying = keepPlaying || e.Current.isPlaying;
                }
            }
            if (!keepPlaying)
            {
                isPlaying = false;
            }
        }
    }

    //Public method that allows to start playing
    public void StartPlaying()
    {
        foreach (KeyValuePair<string, VRPNTrackerRecordings> pair in VRPNTrackerDevice)
        {
            List<VRPNTrackerRecording>.Enumerator e = pair.Value.recordings.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.StartPlaying();
            }
        }
        foreach (KeyValuePair<string, VRPNAnalogRecordings> pair in VRPNAnalogDevice)
        {
            List<VRPNAnalogRecording>.Enumerator e = pair.Value.recordings.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.StartPlaying();
            }
        }
        foreach (KeyValuePair<string, VRPNButtonRecordings> pair in VRPNButtonDevice)
        {
            List<VRPNButtonRecording>.Enumerator e = pair.Value.recordings.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.StartPlaying();
            }
        }
        isPlaying = true;
    }

    //Public method that allows to stop playing
    public void StopPlaying()
    {
        foreach (KeyValuePair<string, VRPNTrackerRecordings> pair in VRPNTrackerDevice)
        {
            List<VRPNTrackerRecording>.Enumerator e = pair.Value.recordings.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.StopPlaying();
            }
        }
        foreach (KeyValuePair<string, VRPNAnalogRecordings> pair in VRPNAnalogDevice)
        {
            List<VRPNAnalogRecording>.Enumerator e = pair.Value.recordings.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.StopPlaying();
            }
        }
        foreach (KeyValuePair<string, VRPNButtonRecordings> pair in VRPNButtonDevice)
        {
            List<VRPNButtonRecording>.Enumerator e = pair.Value.recordings.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.StopPlaying();
            }
        }
        isPlaying = false;
    }

    /* ========================================================================
     * Methods to add recordings
     * ========================================================================*/

    //Public method that allows to add a tracker recording
    public void AddTracker(float timeTracker, string path)
    {
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            VRPNTracker.TrackerReports data = (VRPNTracker.TrackerReports)bf.Deserialize(file);

            file.Close();

            VRPNTrackerRecording recording = new VRPNTrackerRecording(timeTracker, data);

            VRPNTrackerRecordings test;
            if (VRPNTrackerDevice.TryGetValue(data.deviceType + " " + data.deviceName, out test))
            {
                test.recordings.Add(recording);
            }
            else
            {
                test = new VRPNTrackerRecordings();
                test.recordings.Add(recording);
                VRPNTrackerDevice.Add(data.deviceType + " " + data.deviceName, test);
            }
        }
    }

    //Public method that allows to add an analog recording
    public void AddAnalog(float timeAnalog, string path)
    {
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            VRPNAnalog.AnalogReports data = (VRPNAnalog.AnalogReports)bf.Deserialize(file);

            file.Close();

            VRPNAnalogRecording recording = new VRPNAnalogRecording(timeAnalog, data);

            VRPNAnalogRecordings test;
            if (VRPNAnalogDevice.TryGetValue(data.deviceType + " " + data.deviceName, out test))
            {
                test.recordings.Add(recording);
            }
            else
            {
                test = new VRPNAnalogRecordings();
                test.recordings.Add(recording);
                VRPNAnalogDevice.Add(data.deviceType + " " + data.deviceName, test);
            }
        }
    }

    //Public method that allows to add a button recording
    public void AddButton(float timeButton, string path)
    {
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            VRPNButton.ButtonReports data = (VRPNButton.ButtonReports)bf.Deserialize(file);

            file.Close();

            VRPNButtonRecording recording = new VRPNButtonRecording(timeButton, data);

            VRPNButtonRecordings test;
            if (VRPNButtonDevice.TryGetValue(data.deviceType + " " + data.deviceName, out test))
            {
                test.recordings.Add(recording);
            }
            else
            {
                test = new VRPNButtonRecordings();
                test.recordings.Add(recording);
                VRPNButtonDevice.Add(data.deviceType + " " + data.deviceName, test);
            }
        }
    }

    /* ========================================================================
     * Methods to remove recordings
     * ========================================================================*/

    //Public method that allows to remove a tracker recording
    public void RemoveTracker(float timeTracker, string tracker)
    {
        VRPNTrackerRecordings test;
        if (VRPNTrackerDevice.TryGetValue(tracker, out test))
        {
            List<VRPNTrackerRecording>.Enumerator e = test.recordings.GetEnumerator();
            VRPNTrackerRecording recording = null;
            while (e.MoveNext())
            {
                if (e.Current.reportTime == timeTracker)
                {
                    recording = e.Current;
                    break;
                }
            }
            if (recording != null)
            {
                test.recordings.Remove(recording);
            }
            if (test.recordings.Count == 0)
            {
                VRPNTrackerDevice.Remove(tracker);
            }
        }
    }

    //Public method that allows to remove an analog recording
    public void RemoveAnalog(float timeAnalog, string analog)
    {
        VRPNAnalogRecordings test;
        if (VRPNAnalogDevice.TryGetValue(analog, out test))
        {
            List<VRPNAnalogRecording>.Enumerator e = test.recordings.GetEnumerator();
            VRPNAnalogRecording recording = null;
            while (e.MoveNext())
            {
                if (e.Current.reportTime == timeAnalog)
                {
                    recording = e.Current;
                    break;
                }
            }
            if (recording != null)
            {
                test.recordings.Remove(recording);
            }
            if (test.recordings.Count == 0)
            {
                VRPNAnalogDevice.Remove(analog);
            }
        }
    }

    //Public method that allows to remove a button recording
    public void RemoveButton(float timeButton, string tracker)
    {
        VRPNButtonRecordings test;
        if (VRPNButtonDevice.TryGetValue(tracker, out test))
        {
            List<VRPNButtonRecording>.Enumerator e = test.recordings.GetEnumerator();
            VRPNButtonRecording recording = null;
            while (e.MoveNext())
            {
                if (e.Current.reportTime == timeButton)
                {
                    recording = e.Current;
                    break;
                }
            }
            if (recording != null)
            {
                test.recordings.Remove(recording);
            }
            if (test.recordings.Count == 0)
            {
                VRPNButtonDevice.Remove(tracker);
            }
        }
    }

    /* ========================================================================
     * Methods to change the time for recordings
     * ========================================================================*/

    //Public method that allows to change the time for a tracker recording
    public void ChangeTimeTracker(float timeTracker, float newTimeTracker, string tracker)
    {
        VRPNTrackerRecordings test;
        if (VRPNTrackerDevice.TryGetValue(tracker, out test))
        {
            List<VRPNTrackerRecording>.Enumerator e = test.recordings.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.reportTime == timeTracker)
                {
                    e.Current.reportTime = newTimeTracker;
                    break;
                }
            }
        }
    }

    //Public method that allows to change the time for an analog recording
    public void ChangeTimeAnalog(float timeAnalog, float newTimeAnalog, string analog)
    {
        VRPNAnalogRecordings test;
        if (VRPNAnalogDevice.TryGetValue(analog, out test))
        {
            List<VRPNAnalogRecording>.Enumerator e = test.recordings.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.reportTime == timeAnalog)
                {
                    e.Current.reportTime = newTimeAnalog;
                    break;
                }
            }
        }
    }

    //Public method that allows to change the time for a button recording
    public void ChangeTimeButton(float timeButton, float newTimeButton, string button)
    {
        VRPNButtonRecordings test;
        if (VRPNButtonDevice.TryGetValue(button, out test))
        {
            List<VRPNButtonRecording>.Enumerator e = test.recordings.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.reportTime == timeButton)
                {
                    e.Current.reportTime = newTimeButton;
                    break;
                }
            }
        }
    }

    /* ========================================================================
     * Methods to enable and disable sensors for a tracker recording
     * ========================================================================*/

    //Public method that allows to enable a sensor for a tracker recording   
    public void EnableTrackerSensor(float timeTracker, string tracker, int sensor)
    {
        VRPNTrackerRecordings test;
        if (VRPNTrackerDevice.TryGetValue(tracker, out test))
        {
            List<VRPNTrackerRecording>.Enumerator e = test.recordings.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.reportTime == timeTracker)
                {
                    int testInt;
                    if (e.Current.sensorsDisabled.TryGetValue(sensor, out testInt))
                    {
                        e.Current.sensorsDisabled.Remove(sensor);
                    }
                    break;
                }
            }
        }
    }

    //Public method that allows to disable a sensor for a tracker recording   
    public void DisableTrackerSensor(float timeTracker, string tracker, int sensor)
    {
        VRPNTrackerRecordings test;
        if (VRPNTrackerDevice.TryGetValue(tracker, out test))
        {
            List<VRPNTrackerRecording>.Enumerator e = test.recordings.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.reportTime == timeTracker)
                {
                    int testInt;
                    if (!e.Current.sensorsDisabled.TryGetValue(sensor, out testInt))
                    {
                        e.Current.sensorsDisabled.Add(sensor, sensor);
                    }
                    break;
                }
            }
        }
    }
}
