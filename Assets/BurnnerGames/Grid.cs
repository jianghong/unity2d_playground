using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Grid : MonoBehaviour 
{

	public float width = 1.2f;
	public float height = 1.2f;
	
    public int snapToGrid = 0;
	public Color color = Color.white;
    public List<GameObject> prefabs = new List<GameObject>();
    public GameObject selectedObj;
    public GameObject lastObj;

    public bool locked = false;
    public bool gridEnabled = false;
    public bool showGrid = true;
    public bool hideOtherLayer = false;
    public bool complexView = true;
    public int mode = 0;
    public int currentLayer = 0;
    public List<Transform> layerList = new List<Transform>();
    public Transform currentLayerTransform;



    public void Lock()
    {
        LockObject(gameObject, true);
    }

    public void UnLock()
    {
        UnlockObject(gameObject, true);
    }

    void LockObject(GameObject targetObject, bool recursive)
    {
        
        targetObject.hideFlags = targetObject.hideFlags | HideFlags.NotEditable;

        if (recursive)
        {
            foreach (Transform child in targetObject.transform)
                LockObject(child.gameObject, true);
        }
    }

    void UnlockObject(GameObject targetObject, bool recursive)
    {
        targetObject.hideFlags = targetObject.hideFlags & ~HideFlags.NotEditable;

        if (recursive)
        {
            foreach (Transform child in targetObject.transform)
                UnlockObject(child.gameObject, true);
        }
    }

	void OnDrawGizmos()
	{

        if (!showGrid)
            return;

        if (width < 0.1f) width = 0.1f;
        if (height < 0.1f)height = 0.1f;
    
		Vector3 pos = Camera.current.transform.position;
		Gizmos.color = color;
		
		for (float y = pos.y - 800.0f; y < pos.y + 800.0f; y+= height)
        {
            Gizmos.DrawLine(new Vector3(-1000000.0f, Mathf.Floor(y/height) * height, 0.0f),
                            new Vector3(1000000.0f, Mathf.Floor(y/height) * height, 0.0f));
        }

        for (float x = pos.x - 1200.0f; x < pos.x + 1200.0f; x+= width)
        {
           
            Gizmos.DrawLine(new Vector3(Mathf.Floor(x/width) * width, -1000000.0f, 0.0f),
                            new Vector3(Mathf.Floor(x/width) * width, 1000000.0f, 0.0f));
        }
	}
}
