using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class LevelEditorLayer : MonoBehaviour
{
    public bool locked = false;
    public void Lock()
    {
        locked = true;
        LockObject(gameObject, true);
    }

    public void UnLock()
    {
        locked = false;
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

}
