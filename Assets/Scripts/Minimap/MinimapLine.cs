using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapLine : MonoBehaviour
{
    LineRenderer line;
    GameObject _origin;

    // Update is called once per frame
    void Update()
    {
        if(_origin != null)
            line.SetPosition(0, _origin.transform.position);
    }

    public void SetLine(GameObject _origin, GameObject destination)
    {
        this._origin = _origin;

        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.SetPosition(0, this._origin.transform.position);
        line.SetPosition(1, destination.transform.position);
        Debug.Log(destination.transform.position, destination);
    }
}
