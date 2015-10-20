using UnityEngine;
using System.Collections;

public class PruebaPosicion : MonoBehaviour {

	// Use this for initialization
	void Start () {
        VRPNEventManager.StartListeningTracker(VRPNManager.Tracker_Types.vrpn_Tracker_RazerHydra.ToString(), VRPNDeviceConfig.Device_Names.Tracker0.ToString(), CambiarPosicion);
	}

    void CambiarPosicion(string name, VRPNTracker.TrackerReport report)
    {
        this.transform.position = new Vector3((float)report.pos[0], (float)report.pos[1], (float)report.pos[2]);
    }
}
