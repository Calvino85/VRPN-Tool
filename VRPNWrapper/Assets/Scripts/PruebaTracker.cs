using UnityEngine;
using System.Collections;

public class PruebaTracker : MonoBehaviour {

    public bool isShowingButton = false;
    public bool isShowingAnalog = false;
    public bool isShowingTracker = false;
    public Transform capsula1;
    public Transform capsula2;
    public Transform capsula3;
    public Transform capsula4;
    public Transform capsula5;
    public Transform capsula6;
    public Transform capsula7;
    public Transform capsula8;
    private Vector3 capsula1Old;
    private Vector3 capsula2Old;
    private Vector3 capsula3Old;
    private Vector3 capsula4Old;
    private Vector3 capsula5Old;
    private Vector3 capsula6Old;
    private Vector3 capsula7Old;
    private Vector3 capsula8Old;

    // Use this for initialization
    void Start()
    {
        VRPNEventManager.StartListeningButton(VRPNManager.Button_Types.vrpn_Tracker_RazerHydra, VRPNDeviceConfig.Device_Names.Tracker0, changeButton);
        VRPNEventManager.StartListeningAnalog(VRPNManager.Analog_Types.vrpn_Tracker_RazerHydra, VRPNDeviceConfig.Device_Names.Tracker0, changeAnalog);
        VRPNEventManager.StartListeningTracker(VRPNManager.Tracker_Types.vrpn_Tracker_RazerHydra, VRPNDeviceConfig.Device_Names.Tracker0, changeTracker);
        capsula1Old = capsula1.localScale;
        capsula2Old = capsula2.localScale;
        capsula3Old = capsula3.localScale;
        capsula4Old = capsula4.localScale;
        capsula5Old = capsula5.localScale;
        capsula6Old = capsula6.localScale;
        capsula7Old = capsula7.localScale;
        capsula8Old = capsula8.localScale;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void changeButton(string name, VRPNButton.ButtonReport report)
    {
        if (isShowingButton)
        {
            Debug.Log("Name: " + name + " Button: " + report.button + " State:" + report.state);
        }

        switch (report.button)
        {
            case 0:
                if (report.state == 0)
                {
                    capsula3.localScale = capsula3Old;
                }
                else if (report.state == 1)
                {
                    capsula3.localScale = new Vector3(capsula3Old.x, capsula3Old.y * 2, capsula3Old.z);
                }
                break;
            case 1:
                if (report.state == 0)
                {
                    capsula2.localScale = capsula2Old;
                }
                else if (report.state == 1)
                {
                    capsula2.localScale = new Vector3(capsula2Old.x, capsula2Old.y * 2, capsula2Old.z);
                }
                break;
            case 2:
                if (report.state == 0)
                {
                    capsula4.localScale = capsula4Old;
                }
                else if (report.state == 1)
                {
                    capsula4.localScale = new Vector3(capsula4Old.x, capsula4Old.y * 2, capsula4Old.z);
                }
                break;
            case 3:
                if (report.state == 0)
                {
                    capsula1.localScale = capsula1Old;
                }
                else if (report.state == 1)
                {
                    capsula1.localScale = new Vector3(capsula1Old.x, capsula1Old.y * 2, capsula1Old.z);
                }
                break;
            case 4:
                if (report.state == 0)
                {
                    capsula5.localScale = capsula5Old;
                }
                else if (report.state == 1)
                {
                    capsula5.localScale = new Vector3(capsula5Old.x, capsula5Old.y * 2, capsula5Old.z);
                }
                break;
            case 5:
                if (report.state == 0)
                {
                    capsula7.localScale = capsula7Old;
                }
                else if (report.state == 1)
                {
                    capsula7.localScale = new Vector3(capsula7Old.x, capsula7Old.y * 2, capsula7Old.z);
                }
                break;
            case 6:
                if (report.state == 0)
                {
                    capsula6.localScale = capsula6Old;
                }
                else if (report.state == 1)
                {
                    capsula6.localScale = new Vector3(capsula6Old.x, capsula6Old.y * 2, capsula6Old.z);
                }
                break;
            default:
                break;
        }
    }

    void changeAnalog(string name, VRPNAnalog.AnalogReport report)
    {
        if (isShowingAnalog)
        {
            string text;
            text = "Name: " + name;
            for (int i = 0; i < report.num_channel; i++)
            {
                text = text + " Channel " + i + ": " + report.channel[i];
            }
            Debug.Log(text);
        }

        capsula6.localScale = new Vector3(capsula6Old.x * (1 + (float)report.channel[0]), capsula6Old.y, capsula6Old.z * (1 + (float)report.channel[1]));
        capsula8.localScale = new Vector3(capsula8Old.x, capsula8Old.y * (1 + (float)report.channel[2]), capsula8Old.z);
    }

    void changeTracker(string name, VRPNTracker.TrackerReport report)
    {
        if (isShowingTracker)
        {
            string text;
            text = "Name: " + name + " Sensor: " + report.sensor;
            for (int i = 0; i < report.pos.Length; i++)
            {
                text = text + " Pos " + i + ": " + report.pos[i];
            }
            for (int i = 0; i < report.quat.Length; i++)
            {
                text = text + " Quat " + i + ": " + report.quat[i];
            }
            Debug.Log(text);
        }
        if (report.sensor == 0)
        {
            this.transform.position = new Vector3((float)report.pos[0] * 10, (float)report.pos[2] * 10, (float)report.pos[1] * 10);
            this.transform.localRotation = new Quaternion(-1 * (float)report.quat[0], -1 * (float)report.quat[2], -1 * (float)report.quat[1], (float)report.quat[3]);
        }
    }
}
