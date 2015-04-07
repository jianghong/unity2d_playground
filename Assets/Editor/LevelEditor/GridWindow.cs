using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using System.IO;
public class GridWindow : EditorWindow
{
	Grid grid;

    private Vector2 scrollPos = Vector2.zero;
    private bool isMouseDown = false;


    [MenuItem("Window/BurnnerGames/LevelEditor 2D")]
    public static void ShowWindow()
    {
        EditorWindow window = EditorWindow.GetWindow(typeof(GridWindow), false, "Level Editor 2D");
        window.maxSize = new Vector2(245f, 860f);
        window.minSize = new Vector2(245f, 680f);
    }

	void Init()
	{
		grid = (Grid)FindObjectOfType(typeof(Grid));
        SceneView.onSceneGUIDelegate = GridUpdate;
	}


    void GridUpdate(SceneView sceneview)
    {
        if (!grid || !grid.gridEnabled)
            return;

    
        Event e = Event.current;
        int controlID = 0;
        EventType eventType =0;

        Ray r = Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
        Vector3 mousePos = r.origin;

        if (grid.mode == 0)
        {
            if (isMouseDown)
            {
                controlID = GUIUtility.GetControlID(FocusType.Passive);
                eventType = e.GetTypeForControl(controlID);

            }
            if (e.type == EventType.mouseDown)
            {
                if (e.button == 0)
                {
                    GUIUtility.hotControl = 0;
                    isMouseDown = true;

                }
            }
            if (eventType == EventType.mouseUp)
            {
                if (e.button == 0)
                {
                    isMouseDown = false;
                    GUIUtility.hotControl = controlID;
                }
            }
        }


        if (grid.mode == 1)
        {
            if (e.button == 0 && grid.gridEnabled)
            {

                if (e.type != EventType.mouseDown || !grid.hideOtherLayer)
                    return;
                if (grid.currentLayerTransform.gameObject.GetComponent<LevelEditorLayer>().locked)
                    grid.currentLayerTransform.gameObject.GetComponent<LevelEditorLayer>().UnLock();

                if (Selection.activeGameObject)
                {
                    if (Selection.activeGameObject.transform.parent == grid.currentLayerTransform)
                    {
           
                        Undo.DestroyObjectImmediate(Selection.activeGameObject);
                    }
                }
            }

            return;
        }


        if (e.button == 0 && grid.gridEnabled)
        {


            if (e.type == EventType.mouseDown)
            {

                GameObject obj;
                Object prefab = grid.selectedObj;

                if (!prefab)
                {
                    prefab = grid.prefabs[0];
                    grid.selectedObj = (GameObject)prefab;
                }

                if (prefab)
                {

                    Vector3 aligned = Vector3.zero;

                    switch (grid.snapToGrid)
                    {
                        case 0:
                            aligned = new Vector3(Mathf.Floor(mousePos.x / grid.width) * grid.width + grid.width / 2.0f,
                                                     Mathf.Floor(mousePos.y / grid.height) * grid.height + grid.height / 2.0f, 0.0f);
                            break;
                        case 1:
                            aligned = new Vector3(mousePos.x, mousePos.y, 0.0f);
                            break;
                        case 2:
                            aligned = new Vector3(Mathf.Floor(mousePos.x / grid.width) * (grid.width) + grid.width / 2.0f,
                                                     Mathf.Floor(mousePos.y / grid.height) * (grid.height / 2) + grid.height / 2.0f, 0.0f);
                            break;
                    }

                    if(grid.lastObj)
                        if (aligned == grid.lastObj.transform.position)
                         return;

                    Undo.IncrementCurrentGroup();
                    obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                   
                    obj.transform.position = aligned;

                    if (grid.currentLayerTransform == null)
                        AddLayer();

                    if (obj.GetComponent<SpriteRenderer>())
                        obj.GetComponent<SpriteRenderer>().sortingOrder = grid.currentLayer;

                    obj.transform.parent = grid.currentLayerTransform;

                    if (grid.locked)
                        obj.hideFlags = obj.hideFlags | HideFlags.NotEditable;

                    grid.lastObj = obj;

                    Undo.RegisterCreatedObjectUndo(obj, "Create " + obj.name);
                }
                return;
            }
        }



        if (grid.gridEnabled)
        {

            if (eventType == EventType.mouseDrag && grid.snapToGrid == 0 && isMouseDown)
            {

                GameObject obj;
                Object prefab = grid.selectedObj;

                if (!prefab)
                {
                    prefab = grid.prefabs[0];
                    grid.selectedObj = (GameObject)prefab;
                }

                if (prefab)
                {

                    Vector3 aligned = Vector3.zero;

                    aligned = new Vector3(Mathf.Floor(mousePos.x / grid.width) * grid.width + grid.width / 2.0f,
                                                     Mathf.Floor(mousePos.y / grid.height) * grid.height + grid.height / 2.0f, 0.0f);
                    if (aligned == grid.lastObj.transform.position)
                        return;

                    Undo.IncrementCurrentGroup();
                    obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);



                    obj.transform.position = aligned;

                    if (grid.currentLayerTransform == null)
                        AddLayer();

                    if (obj.GetComponent<SpriteRenderer>())
                        obj.GetComponent<SpriteRenderer>().sortingOrder = grid.currentLayer;

                    obj.transform.parent = grid.currentLayerTransform;

                    if (grid.locked)
                        obj.hideFlags = obj.hideFlags | HideFlags.NotEditable;

                    grid.lastObj = obj;

                    Undo.RegisterCreatedObjectUndo(obj, "Create " + obj.name);
                }
                return;
            }
        }

        if (e.button == 2 && e.type == EventType.mouseDown)
        {
           

            GameObject delete = null;
            if (Selection.activeGameObject != null)
                delete = Selection.activeGameObject;
            else
                if (!grid.lastObj)
                    return;
                else
                    delete = grid.lastObj;

            
            if(delete.transform.parent == grid.currentLayerTransform)
            DestroyImmediate(delete);
        }
     
    }

    void AddLayer()
    {
  
        string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));

        path = path.Substring(0, path.LastIndexOf('/'));
        path = path.Substring(0, path.LastIndexOf('/'));
        path = path.Substring(0, path.LastIndexOf('/') + 1);
        
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(path + "BurnnerGames/LevelLayer.prefab", typeof(GameObject)));
        grid.layerList.Add(obj.transform);
        grid.currentLayer = grid.layerList.Count-1;
        obj.name = obj.name + " " + grid.currentLayer.ToString(); 
        obj.transform.parent = grid.transform;

        if (grid.currentLayerTransform != null)
            grid.currentLayerTransform.gameObject.GetComponent<LevelEditorLayer>().Lock();

        grid.currentLayerTransform = obj.transform;
        grid.currentLayerTransform.gameObject.GetComponent<LevelEditorLayer>().UnLock();
    }

    void DeleteLayer()
    {

        if (!EditorUtility.DisplayDialog("Delete current layer?",
            "Are you sure you want to delete the current layer?\nthis can not be undone!", "Yes", "No"))
            return;

        grid.layerList.Remove(grid.currentLayerTransform);
        DestroyImmediate(grid.currentLayerTransform.gameObject);

        if (0 < grid.currentLayer)
            grid.currentLayer--;
        else
            if(0 < grid.layerList.Count -1)
                grid.currentLayer = grid.layerList.Count-1;

        grid.currentLayerTransform = grid.layerList[grid.currentLayer];
        grid.currentLayerTransform.gameObject.GetComponent<LevelEditorLayer>().UnLock();

        if (grid.hideOtherLayer)
            HideLayerHelper(true);

    }

    void SwitchLayer(bool back)
    {
        if (back)
        {
            if (grid.currentLayer == 0) return;

            grid.currentLayerTransform.gameObject.GetComponent<LevelEditorLayer>().Lock();
            grid.currentLayer--;
            grid.currentLayerTransform = grid.layerList[grid.currentLayer];
            grid.currentLayerTransform.gameObject.GetComponent<LevelEditorLayer>().UnLock();

        }
        else
        {
            if (grid.currentLayer == grid.layerList.Count-1) return;

            grid.currentLayerTransform.gameObject.GetComponent<LevelEditorLayer>().Lock();
            grid.currentLayer++;
            grid.currentLayerTransform = grid.layerList[grid.currentLayer];
            grid.currentLayerTransform.gameObject.GetComponent<LevelEditorLayer>().UnLock();
        }


        if (grid.hideOtherLayer)
            HideLayerHelper(true);

    }
	void OnGUI()
	{
        Init();

        if (grid)
        {

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Layer: " + grid.currentLayer.ToString(), GUILayout.Width(80));

            if (GUILayout.Button(new GUIContent(LeftIcon)))
                SwitchLayer(true);

            if (GUILayout.Button(new GUIContent(RightIcon)))
                SwitchLayer(false);

            if (GUILayout.Button(new GUIContent(AdIconB)))
                AddLayer();

            if (GUILayout.Button(new GUIContent(BinIcon)))
                DeleteLayer();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (grid.gridEnabled)
            {
                if (GUILayout.Button((new GUIContent(PauseIcon))))
                {

                    HideLayerHelper(false);
                    grid.gridEnabled = false;
                }
            }
            else
            {
                if (GUILayout.Button((new GUIContent(ForwardIcon))))
                    grid.gridEnabled = true;
            }

            if (grid.mode == 0)
            {
                if (GUILayout.Button(BrushIcon))
                {
         
                    grid.mode = 1;
                    grid.hideOtherLayer = true;
                    HideLayerHelper(true);
                }
            }
            else
            {
                if (GUILayout.Button(EraserIcon))
                    grid.mode = 0;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (grid.showGrid)
            {
                if (GUILayout.Button("Hide Grid"))
                    grid.showGrid = false;
            }
            else
            {
                if (GUILayout.Button("Show Grid"))
                    grid.showGrid = true;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            switch (grid.snapToGrid)
            {
                case 0:
                    if (GUILayout.Button("Snap to Grid"))
                        grid.snapToGrid = 1;
                    break;
                case 1:
                    if (GUILayout.Button("Free Hand"))
                        grid.snapToGrid = 0;
                    break;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (!grid.hideOtherLayer)
            {
                if (GUILayout.Button("Hide other Layers"))
                    HideLayerHelper(true);
            }
            else
            {
                if (GUILayout.Button("Show other Layers"))
                    HideLayerHelper(false);
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent(LeftIcon)))
                MoveLast(0);

            if (GUILayout.Button(new GUIContent(RightIcon)))
                MoveLast(1);

            if (GUILayout.Button(new GUIContent(UpIcon)))
                MoveLast(2);

            if (GUILayout.Button(new GUIContent(DownIcon)))
                MoveLast(3);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Grid Settings"))
            {
                GridSettings window = (GridSettings)EditorWindow.GetWindow(typeof(GridSettings));
                window.Init();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (grid.complexView)
            {
                if (GUILayout.Button("ComplexView"))
                    grid.complexView = false;
      
            }
            else
            {
                if (GUILayout.Button("Simple View"))
                    grid.complexView = true;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(350), GUILayout.ExpandWidth(true));

            int prefCounter = 0;
            
            if (grid.complexView)
            {
                foreach (GameObject obj in grid.prefabs)
                {

                    if (obj)
                    {


                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.LabelField(obj.name, GUILayout.Width(130));



                        EditorGUILayout.BeginHorizontal();


                        if (grid.selectedObj == obj)
                        {
                            if (GUILayout.Button(new GUIContent(FlagIcon, "Select Prefab"), GUILayout.Width(64)))
                                Select(obj);
                        }
                        else
                        {

                            if (GUILayout.Button(new GUIContent(CheckIcon, "Select Prefab"), GUILayout.Width(64)))
                                Select(obj);
                        }



                        if (GUILayout.Button(new GUIContent(BinIcon, "Delete Prefab from list"), GUILayout.Width(64)))
                        {
                            Delete(obj);
                            break;

                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();

                        Texture2D prefabIcon = AssetPreview.GetAssetPreview(obj);

                        if (prefabIcon == null)
                        {
                            if (GUILayout.Button(EmptyIcon, GUILayout.Height(50), GUILayout.Width(50)))
                                Select(obj);
                        }
                        else
                        {
                            if (GUILayout.Button(prefabIcon, GUILayout.Height(50), GUILayout.Width(50)))
                                Select(obj);
                        }


                        EditorGUILayout.BeginVertical();
                        if (GUILayout.Button(new GUIContent(UpIcon), GUILayout.Width(25), GUILayout.Height(25)))
                        {
                            MovePrefab(true, prefCounter);
                            break;
                        }
                        if (GUILayout.Button(new GUIContent(DownIcon), GUILayout.Width(25), GUILayout.Height(25)))
                        {
                            MovePrefab(false, prefCounter);
                            break;
                        }
                        EditorGUILayout.EndVertical();


                        EditorGUILayout.EndHorizontal();

                        prefCounter++;

                    }
                }
            }
            else
            {
                int ticker = 0;
                foreach (GameObject obj in grid.prefabs)
                {

                    if (obj)
                    {

                        if (ticker == 0)
                            EditorGUILayout.BeginHorizontal();
                  
                        Texture2D prefabIcon = AssetPreview.GetAssetPreview(obj);

                        if (prefabIcon == null)
                        {
                           if( GUILayout.Button(EmptyIcon, GUILayout.Height(50), GUILayout.Width(50)))
                                  Select(obj);
                        }
                        else
                        {
                            if(GUILayout.Button(prefabIcon, GUILayout.Height(50), GUILayout.Width(50)))
                                Select(obj);
                        }

                        if (ticker == 3)
                        {
                            EditorGUILayout.EndHorizontal();
                            ticker = 0;
                        }
                        else
                        {
                            ticker++;
                        }

                        prefCounter++;

                    }
                }

               if(ticker != 0)
                   EditorGUILayout.EndHorizontal();

            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (grid.selectedObj != null)
            {
                EditorGUILayout.LabelField(grid.selectedObj.name);
            }


            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
          
            if(grid.selectedObj != null)
            {
                Texture2D prefabIcon2 = AssetPreview.GetAssetPreview(grid.selectedObj);

                if (prefabIcon2 == null)
                    GUILayout.Button(EmptyIcon, GUILayout.Height(80));
                else
                    GUILayout.Button(prefabIcon2, GUILayout.Height(80));

            }
               

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent(AdIcon, "Select the prefabs in the project view and then hit the button.")))
                CreateNewPrefab();
            if (GUILayout.Button(new GUIContent(BinIconB)))
                RemovePrefabs();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("");
            EditorGUILayout.EndHorizontal();

     
            EditorUtility.SetDirty(grid);
        }
        else
        {
            if(GUILayout.Button("Create Grit"))
            {
                string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));

                path = path.Substring(0, path.LastIndexOf('/'));
                path = path.Substring(0, path.LastIndexOf('/'));
                path = path.Substring(0, path.LastIndexOf('/')+1);
                GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(path + "BurnnerGames/Grid.prefab", typeof(GameObject)));
                obj.transform.position = new Vector3(0, 0, 0);
            }
        }

	}


    void MovePrefab(bool up, int index)
    {
        GameObject temp;
       
        temp = grid.prefabs[index];
       

        if (up)
        {
            if (index == 0)
                return;
            grid.prefabs.Remove(grid.prefabs[index]);
            index--;
         
        }
        else
        {
            if (index == grid.prefabs.Count - 1)
                return;
            grid.prefabs.Remove(grid.prefabs[index]);
            index++;
        }
       
        grid.prefabs.Insert(index, temp);
        
    }

    void HideLayerHelper(bool hide)
    {
        grid.hideOtherLayer = hide;
        foreach (Transform ly in grid.layerList)
        {
            if (ly == grid.currentLayerTransform)
            {
                ly.gameObject.SetActive(true);
                continue;
            }

            if (hide)
                ly.gameObject.SetActive(false);
            else
                ly.gameObject.SetActive(true);

        }
    }

    void RemovePrefabs()
    {
        if (!EditorUtility.DisplayDialog("Remove all Prefabs?",
                        "Are you sure you want to remove all prefabs from the list?\nthis can not be undone!", "Yes", "No"))
            return;
        grid.prefabs = new List<GameObject>();
     
    }
    void MoveLast(int direction)
    {

        Vector3 aligned = Vector3.zero;

        GameObject obj = null;

        if (Selection.activeGameObject != null)
            obj = Selection.activeGameObject;
        else
            obj = grid.lastObj;

        if (direction == 0)
        {
             aligned = new Vector3(Mathf.Floor((obj.transform.position.x / grid.width)-grid.width) * grid.width + grid.width / 2.0f,
                                           Mathf.Floor(obj.transform.position.y / grid.height) * grid.height + grid.height / 2.0f, 0.0f);
        }
        if (direction == 1)
        {
            aligned = new Vector3(Mathf.Floor((obj.transform.position.x / grid.width) + grid.width) * grid.width + grid.width / 2.0f,
                                          Mathf.Floor(obj.transform.position.y / grid.height) * grid.height + grid.height / 2.0f, 0.0f);
        }

        if (direction == 2)
        {
            aligned = new Vector3(Mathf.Floor((obj.transform.position.x / grid.width)) * grid.width + grid.width / 2.0f,
                                          Mathf.Floor((obj.transform.position.y / grid.height)+grid.height) * grid.height + grid.height / 2.0f, 0.0f);
        }
        if (direction == 3)
        {
            aligned = new Vector3(Mathf.Floor((obj.transform.position.x / grid.width)) * grid.width + grid.width / 2.0f,
                                          Mathf.Floor((obj.transform.position.y / grid.height) - grid.height) * grid.height + grid.height / 2.0f, 0.0f);
        }
        Undo.RecordObject(obj.transform, "Move Transform");
        obj.transform.position = aligned;
    }

    void LockObj()
    {
        if (grid.locked)
        {
            grid.locked = false;
            grid.UnLock();
        }
        else
        {
            grid.locked = true;
            grid.Lock();
        }

    }

    void Select(GameObject select)
    {
        grid.selectedObj = select;
    }
    void Delete(GameObject delete)
    {
        grid.prefabs.Remove(delete);

    }

    void CreateNewPrefab()
    {
        Object[] selectedAsset = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets);
        foreach (Object obj in selectedAsset)
        {
            grid.prefabs.Add((GameObject)obj);
        }
        OnGUI();
    }


    private Texture2D emptyIcon;
    public Texture2D EmptyIcon
    {
        get
        {
            if ((UnityEngine.Object)emptyIcon == (UnityEngine.Object)null)
                emptyIcon = AssetDatabase.LoadAssetAtPath(IconsPath + "empty.png", typeof(Texture2D)) as Texture2D;

            return emptyIcon;
        }
    }

    private Texture2D leftIcon;
    public Texture2D LeftIcon
    {
        get
        {
            if ((UnityEngine.Object) leftIcon == (UnityEngine.Object)null)
                leftIcon = AssetDatabase.LoadAssetAtPath(IconsPath + "arrowLeft.png", typeof(Texture2D)) as Texture2D;
    
            return leftIcon;
        }
    }

    private Texture2D rightIcon;
    public Texture2D RightIcon
    {
        get
        {
            if ((UnityEngine.Object)rightIcon == (UnityEngine.Object)null)
                rightIcon = AssetDatabase.LoadAssetAtPath(IconsPath + "arrowRight.png", typeof(Texture2D)) as Texture2D;
    
            return rightIcon;
        }
    }

    private Texture2D downIcon;
    public Texture2D DownIcon
    {
        get
        {
            if ((UnityEngine.Object)downIcon == (UnityEngine.Object)null)
                downIcon = AssetDatabase.LoadAssetAtPath(IconsPath + "arrowDown.png", typeof(Texture2D)) as Texture2D;
           
            return downIcon;
        }
    }


    private Texture2D upIcon;
    public Texture2D UpIcon
    {
        get
        {
            if ((UnityEngine.Object) upIcon == (UnityEngine.Object)null)
                upIcon = AssetDatabase.LoadAssetAtPath(IconsPath + "arrowUp.png", typeof(Texture2D)) as Texture2D;

            return upIcon;
        }
    }

    private Texture2D adIcon;
    public Texture2D AdIcon
    {
        get
        {
            if ((UnityEngine.Object)adIcon == (UnityEngine.Object)null)
                adIcon = AssetDatabase.LoadAssetAtPath(IconsPath + "plus.png", typeof(Texture2D)) as Texture2D;

            return adIcon;
        }
    }

    private Texture2D adIconB;
    public Texture2D AdIconB
    {
        get
        {
            if ((UnityEngine.Object)adIconB == (UnityEngine.Object)null)
                adIconB = AssetDatabase.LoadAssetAtPath(IconsPath + "plusB.png", typeof(Texture2D)) as Texture2D;

            return adIconB;
        }
    }


    private Texture2D checkIcon;
    public Texture2D CheckIcon
    {
        get
        {
            if ((UnityEngine.Object)checkIcon == (UnityEngine.Object)null)
                checkIcon = AssetDatabase.LoadAssetAtPath(IconsPath + "checkmark.png", typeof(Texture2D)) as Texture2D;

            return checkIcon;
        }
    }


    private Texture2D binIcon;
    public Texture2D BinIcon
    {
        get
        {
            if ((UnityEngine.Object)binIcon == (UnityEngine.Object)null)
                binIcon = AssetDatabase.LoadAssetAtPath(IconsPath + "trashcanOpen.png", typeof(Texture2D)) as Texture2D;

            return binIcon;
        }
    }

    private Texture2D binIconB;
    public Texture2D BinIconB
    {
        get
        {
            if ((UnityEngine.Object)binIconB == (UnityEngine.Object)null)
                binIconB = AssetDatabase.LoadAssetAtPath(IconsPath + "trashcanOpenB.png", typeof(Texture2D)) as Texture2D;

            return binIconB;
        }
    }

    private Texture2D eraserIcon;
    public Texture2D EraserIcon
    {
        get
        {
            if ((UnityEngine.Object)eraserIcon == (UnityEngine.Object)null)
                eraserIcon = AssetDatabase.LoadAssetAtPath(IconsPath + "toolEraser.png", typeof(Texture2D)) as Texture2D;

            return eraserIcon;
        }
    }

    private Texture2D flagIcon;
    public Texture2D FlagIcon
    {
        get
        {
            if ((UnityEngine.Object)flagIcon == (UnityEngine.Object)null)
                flagIcon = AssetDatabase.LoadAssetAtPath(IconsPath + "flag.png", typeof(Texture2D)) as Texture2D;

            return flagIcon;
        }
    }

    private Texture2D pointerIcon;
    public Texture2D PointerIcon
    {
        get
        {
            if ((UnityEngine.Object)pointerIcon == (UnityEngine.Object)null)
                pointerIcon = AssetDatabase.LoadAssetAtPath(IconsPath + "pointer.png", typeof(Texture2D)) as Texture2D;

            return pointerIcon;
        }
    }

    private Texture2D brushIcon;
    public Texture2D BrushIcon
    {
        get
        {
            if ((UnityEngine.Object)brushIcon == (UnityEngine.Object)null)
                brushIcon = AssetDatabase.LoadAssetAtPath(IconsPath + "toolBrush.png", typeof(Texture2D)) as Texture2D;

            return brushIcon;
        }
    }


    private Texture2D pauseIcon;
    public Texture2D PauseIcon
    {
        get
        {
            if ((UnityEngine.Object)pauseIcon == (UnityEngine.Object)null)
                pauseIcon = AssetDatabase.LoadAssetAtPath(IconsPath + "pause.png", typeof(Texture2D)) as Texture2D;

            return pauseIcon;
        }
    }

    private Texture2D forwardIcon;
    public Texture2D ForwardIcon
    {
        get
        {
            if ((UnityEngine.Object)forwardIcon == (UnityEngine.Object)null)
                forwardIcon = AssetDatabase.LoadAssetAtPath(IconsPath + "forward.png", typeof(Texture2D)) as Texture2D;

            return forwardIcon;
        }
    }

    private string iconsPath;
    private string IconsPath
    {
        get
        {
            if (string.IsNullOrEmpty(iconsPath))
            {
  
                string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
                path = path.Substring(0, path.LastIndexOf('/')+1);
                iconsPath = path + "Icons/";

            }

            return iconsPath;
        }
    }
}

