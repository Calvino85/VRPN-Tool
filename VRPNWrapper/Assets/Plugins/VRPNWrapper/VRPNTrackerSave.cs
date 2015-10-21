using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class VRPNTrackerSave : MonoBehaviour {
    public VRPNManager.Tracker_Types TrackerType = VRPNManager.Tracker_Types.vrpn_Tracker_RazerHydra;
    public VRPNDeviceConfig.Device_Names TrackerName = VRPNDeviceConfig.Device_Names.Tracker0;

    private VRPNTracker.TrackerReports data = new VRPNTracker.TrackerReports();

    private bool firstReport = true;
    private UInt32 firstTime_sec;
    private UInt32 firstTime_usec;

    private string path;

    public void StartRecording(string nPath)
    {
        path = nPath;
        data.deviceType = TrackerType.ToString();
        data.deviceName = TrackerName.ToString();
        VRPNEventManager.StartListeningTracker(TrackerType.ToString(), TrackerName.ToString(), Record);
    }

    void Record(string name, VRPNTracker.TrackerReport report)
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
        VRPNTracker.TrackerReportNew newReport = new VRPNTracker.TrackerReportNew();
        VRPNManager.TimeValNew newMsgTime = new VRPNManager.TimeValNew();
        newMsgTime.tv_sec = (int)report.msg_time.tv_sec;
        newMsgTime.tv_usec = (int)report.msg_time.tv_usec;
        newReport.msg_time = newMsgTime;
        newReport.pos = report.pos;
        newReport.quat = report.quat;
        newReport.sensor = report.sensor;
        data.list.Add(newReport);
    }

    public void StopRecording()
    {
        VRPNEventManager.StopListeningTracker(TrackerType.ToString(), TrackerName.ToString(), Record);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path);

        bf.Serialize(file, data);
        file.Close();
        data = new VRPNTracker.TrackerReports();
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