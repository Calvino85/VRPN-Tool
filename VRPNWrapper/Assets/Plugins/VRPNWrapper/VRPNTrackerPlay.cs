using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class VRPNTrackerPlay : MonoBehaviour {

    private VRPNTracker.TrackerReports data;
    private bool playing = false;
    private bool firstReport = true;
    private float firstTime;
    private List<VRPNTracker.TrackerReportNew>.Enumerator e;
    private VRPNTracker.TrackerReportNew actualReport;
    private VRPNTracker.TrackerReportNew lastReport;

    public void StartPlaying(string nPath)
    {
        if (File.Exists(nPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(nPath, FileMode.Open);
            data = (VRPNTracker.TrackerReports)bf.Deserialize(file);

            file.Close();

            playing = true;

            e = data.list.GetEnumerator();
        }
    }

    void Update()
    {
        if(playing)
        {
            float actualTime;
            float actualReportTime = 0f;
            bool moreReports = true;

            if (firstReport)
            {
                firstReport = false;
                firstTime = Time.time;
                if (e.MoveNext())
                {
                    actualReport = e.Current;
                }
                else
                {
                    playing = false;
                    moreReports = false;
                    firstReport = true;
                }
            }

            actualTime = Time.time - firstTime;

            while (moreReports)
            {
                actualReportTime = actualReport.msg_time.tv_sec + (actualReport.msg_time.tv_usec / 1000000f);
                if (actualReportTime <= actualTime)
                {
                    lastReport = e.Current;
                    if (e.MoveNext())
                    {
                        actualReport = e.Current;
                    }
                    else
                    {
                        moreReports = false;
                        playing = false;
                        firstReport = true;
                    }
                }
                else
                {
                    VRPNTracker.TrackerReport newReport = new VRPNTracker.TrackerReport();
                    VRPNManager.TimeVal newMsgTime = new VRPNManager.TimeVal();
                    newMsgTime.tv_sec = (UInt32)lastReport.msg_time.tv_sec;
                    newMsgTime.tv_usec = (UInt32)lastReport.msg_time.tv_usec;
                    newReport.msg_time = newMsgTime;
                    newReport.pos = lastReport.pos;
                    newReport.quat = lastReport.quat;
                    newReport.sensor = lastReport.sensor;
                    VRPNEventManager.TriggerEventTracker(data.deviceType, data.deviceName, newReport);
                    moreReports = false;
                    Debug.Log(newMsgTime.tv_sec + "." + newMsgTime.tv_usec);
                }
            }
        }
    }

    public void StopPlaying()
    {
        playing = false;
        firstReport = true;
        e = data.list.GetEnumerator();
    }

    public void PausePlaying()
    {
        playing = !playing;
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 180, 100, 30), "Reproducir"))
        {
            StartPlaying("C:/playerInfo.dat");
        }
        if (GUI.Button(new Rect(10, 220, 100, 30), "Parar"))
        {
            StopPlaying();
        }
        if (GUI.Button(new Rect(10, 260, 100, 30), "Pausar"))
        {
            PausePlaying();
        }
    }
}
