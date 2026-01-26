using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PrefabPainter : EditorWindow
{
    private List<GameObject> prefabs = new List<GameObject>();
    private int selectedIndex = -1;
    private GameObject parent;
    private bool eraseMode = false;
    private bool isEnablePainter = true;
    private float heightOffset = 0f;

    private GameObject previewInstance;

    [MenuItem("Tools/Surface Object Painter")]
    public static void ShowWindow() => GetWindow<PrefabPainter>("Prefab Painter");

    private void OnGUI()
    {
        GUILayout.Label("Prefab Painter Tool", EditorStyles.boldLabel);
        isEnablePainter = GUILayout.Toggle(isEnablePainter, isEnablePainter ? "Enable" : "Disable", "Button");

        // drag & drop prefabs into the window
        GUILayout.Label("Drag prefabs here:");
        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drop prefabs here");
        if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
        {
            if (dropArea.Contains(evt.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (Object obj in DragAndDrop.objectReferences)
                    {
                        if (obj is GameObject go && !prefabs.Contains(go))
                        {
                            prefabs.Add(go);
                        }
                    }
                }
                evt.Use();
            }
        }

        // display prefab thumbnails
        GUILayout.Label("Select Prefab:");
        int columns = 4;
        int rows = Mathf.CeilToInt(prefabs.Count / (float)columns);
        float thumbSize = 64;

        for (int y = 0; y < rows; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < columns; x++)
            {
                int i = y * columns + x;
                if (i >= prefabs.Count) break;

                GameObject prefab = prefabs[i];
                Texture2D preview = AssetPreview.GetAssetPreview(prefab);
                if (preview == null) preview = AssetPreview.GetMiniThumbnail(prefab);

                GUIStyle style = new GUIStyle(GUI.skin.button);
                if (i == selectedIndex) style.normal.background = Texture2D.grayTexture;

                if (GUILayout.Button(preview, style, GUILayout.Width(thumbSize), GUILayout.Height(thumbSize)))
                {
                    selectedIndex = i;
                }
            }
            GUILayout.EndHorizontal();
        }
        // Parent field and other options
        parent = (GameObject)EditorGUILayout.ObjectField("Parent", parent, typeof(GameObject), true);
        heightOffset = EditorGUILayout.FloatField("Height Offset", heightOffset);
        eraseMode = GUILayout.Toggle(eraseMode, eraseMode ? "Erase Mode" : "Place Mode", "Button");
    }

    private void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        DestroyPreview();
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!isEnablePainter) return;

        Event e = Event.current;
        if (e == null || selectedIndex < 0) return;

        GameObject prefab = prefabs[selectedIndex];

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        // draw preview
        Vector3 pos = hit.point + hit.normal * heightOffset;
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
        if (!eraseMode){   
            DrawPrefabPreview(prefab, pos, rot);
        }

        // place / erase objects
        if ((e.type == EventType.MouseDown) && e.button == 0)
        {
            if (eraseMode)
            {
                Collider[] hits = Physics.OverlapSphere(pos, 0.5f);

                foreach (var h in hits){
                    PaintedMarker marker = h.GetComponentInParent<PaintedMarker>();
                    if (marker != null)
                    {
                        Undo.DestroyObjectImmediate(marker.gameObject);
                    }
                }
                    
            }
            else if (prefab != null)
            {
                GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                Undo.RegisterCreatedObjectUndo(newObj, "Place Prefab");

                if (parent != null) newObj.transform.parent = parent.transform;

                newObj.transform.position = pos;
                newObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                newObj.AddComponent<PaintedMarker>();
            }

            e.Use();
        }

        HandleUtility.Repaint();
    }

    private void DestroyPreview()
    {
        if (previewInstance != null) DestroyImmediate(previewInstance);
    }

    private void DrawPrefabPreview(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        // 3D meshes
        MeshFilter[] meshes = prefab.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter mf in meshes)
        {
            if (mf.sharedMesh == null) continue;
            Material mat = mf.GetComponent<MeshRenderer>()?.sharedMaterial ?? AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
            Graphics.DrawMesh(mf.sharedMesh, position * 1f, rotation * mf.transform.localRotation, mat, 0);
        }

        // sprites
        SpriteRenderer[] sprites = prefab.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in sprites)
        {
            if (sr.sprite == null) continue;

            Vector3 spritePos = position + rotation * sr.transform.localPosition;
            Quaternion spriteRot = rotation * sr.transform.localRotation;

            // use a built-in sprite material
            Material spriteMat = sr.sharedMaterial != null ? sr.sharedMaterial : new Material(Shader.Find("Sprites/Default"));
            spriteMat.mainTexture = sr.sprite.texture;

            // get sprite size in world units
            Vector2 size = sr.sprite.rect.size / sr.sprite.pixelsPerUnit;

            // quad vertices
            Mesh quad = new Mesh();
            quad.vertices = new Vector3[]
            {
                new Vector3(-size.x/2, -size.y/2, 0),
                new Vector3(size.x/2, -size.y/2, 0),
                new Vector3(-size.x/2, size.y/2, 0),
                new Vector3(size.x/2, size.y/2, 0)
            };

            quad.uv = new Vector2[]
            {
                new Vector2(sr.sprite.rect.x / sr.sprite.texture.width, sr.sprite.rect.y / sr.sprite.texture.height),
                new Vector2((sr.sprite.rect.x + sr.sprite.rect.width)/sr.sprite.texture.width, sr.sprite.rect.y / sr.sprite.texture.height),
                new Vector2(sr.sprite.rect.x / sr.sprite.texture.width, (sr.sprite.rect.y + sr.sprite.rect.height)/sr.sprite.texture.height),
                new Vector2((sr.sprite.rect.x + sr.sprite.rect.width)/sr.sprite.texture.width, (sr.sprite.rect.y + sr.sprite.rect.height)/sr.sprite.texture.height)
            };

            quad.triangles = new int[] { 0, 2, 1, 2, 3, 1 };

            Graphics.DrawMesh(quad, Matrix4x4.TRS(spritePos, spriteRot, Vector3.one), spriteMat, 0);
        }
    }
}
