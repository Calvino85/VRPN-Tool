using UnityEngine;
using System.Collections;

public class PruebaMouse : MonoBehaviour {

	// Use this for initialization
	void Start () {
        VRPNEventManager.StartListeningAnalog(VRPNManager.Analog_Types.vrpn_Mouse, VRPNDeviceConfig.Device_Names.Mouse0, moveWithMouse);	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void moveWithMouse(string name, VRPNAnalog.AnalogReport report)
    {
        this.transform.position = new Vector3((float)report.channel[0]*10 - 5, (1-(float)report.channel[1])*10 - 5, this.transform.position.z);
    }
}
