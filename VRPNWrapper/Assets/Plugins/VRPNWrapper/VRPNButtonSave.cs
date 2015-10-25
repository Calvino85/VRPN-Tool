/* ========================================================================
 * PROJECT: VRPN Tool
 * ========================================================================
 * 
 * Based on http://unity3d.com/es/learn/tutorials/modules/beginner/live-training-archive/persistence-data-saving-loading
 *
 * ========================================================================
 ** @author   Andrés Roberto Gómez (and-gome@uniandes.edu.co)
 *
 * ========================================================================
 *
 * VRPNButtonSave.cs
 *
 * usage: Must be added once for each button that is desired to record.
 * 
 * inputs:
 * ButtonType   - VRPN button device name from configuration file
 * ButtonName   - name assigned to the device in its configuration line
 *
 * Notes:
 *
 * ========================================================================*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class VRPNButtonSave : MonoBehaviour
{
    //Public Properties
    public VRPNManager.Button_Types ButtonType = VRPNManager.Button_Types.vrpn_Mouse;
    public VRPNDeviceConfig.Device_Names ButtonName = VRPNDeviceConfig.Device_Names.Mouse0;

    //Private Properties
    private VRPNButton.ButtonReports data = new VRPNButton.ButtonReports();
    private bool firstReport = true;
    private UInt32 firstTime_sec;
    private UInt32 firstTime_usec;
    private string path;

    //Public method that allows to start recording in the indicated path
    //It registers the listener method in the event manager
    public void StartRecording(string nPath)
    {
        path = nPath;
        data.deviceType = ButtonType.ToString();
        data.deviceName = ButtonName.ToString();
        VRPNEventManager.StartListeningButton(ButtonType.ToString(), ButtonName.ToString(), Record);
    }

    //This is the listener that is called by the event manager
    //It transforms and adds the received report to the reports list
    void Record(string name, VRPNButton.ButtonReport report)
    {
        if (firstReport)
        {
            firstTime_sec = report.msg_time.tv_sec;
            firstTime_usec = report.msg_time.tv_usec;
            firstReport = false;
        }
        if (report.msg_time.tv_usec < firstTime_usec)
        {
            report.msg_time.tv_sec = report.msg_time.tv_sec - (firstTime_sec + 1);
            report.msg_time.tv_usec = (report.msg_time.tv_usec + 1000000) - firstTime_usec;
        }
        else
        {
            report.msg_time.tv_sec = report.msg_time.tv_sec - firstTime_sec;
            report.msg_time.tv_usec = report.msg_time.tv_usec - firstTime_usec;
        }
        VRPNButton.ButtonReportNew newReport = new VRPNButton.ButtonReportNew();
        VRPNManager.TimeValNew newMsgTime = new VRPNManager.TimeValNew();
        newMsgTime.tv_sec = (int)report.msg_time.tv_sec;
        newMsgTime.tv_usec = (int)report.msg_time.tv_usec;
        newReport.msg_time = newMsgTime;
        newReport.button = report.button;
        newReport.state = report.state;
        data.list.Add(newReport);
    }

    //Public method that allows to stop recording
    //It saves the reports list in the indicated path
    public void StopRecording()
    {
        VRPNEventManager.StopListeningButton(ButtonType.ToString(), ButtonName.ToString(), Record);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path);

        bf.Serialize(file, data);
        file.Close();
        data = new VRPNButton.ButtonReports();
        firstReport = true;
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 100, 100, 30), "Grabar"))
        {
            StartRecording("C:/playerInfo.dat");
        }
        if (GUI.Button(new Rect(10, 140, 100, 30), "Parar"))
        {
            StopRecording();
        }
    }
}