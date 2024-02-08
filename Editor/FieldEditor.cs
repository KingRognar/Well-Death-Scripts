using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class FieldEditor : EditorWindow
{
    int height, width, depth, curDepth = 0;

    VisualElement root;



    [MenuItem("Window/UI Toolkit/FieldEditor")]
    public static void ShowExample()
    {
        FieldEditor wnd = GetWindow<FieldEditor>();
        wnd.titleContent = new GUIContent("FieldEditor");
    }

    public void CreateGUI()
    {
        root = rootVisualElement;
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/FieldEditor.uss");
        root.styleSheets.Add(styleSheet);

        var buttonsBlock = AddElement<VisualElement>(root, "buttonsBlock");

        var depthDownBtn = AddElement<Button> (buttonsBlock, "depthButton");
        depthDownBtn.clicked += DepthDown;
        depthDownBtn.text = "v";

        var newMapBtn = AddElement<Button>(buttonsBlock, "newMapButton");
        newMapBtn.clicked += GetNewMap;
        newMapBtn.text = "Get new data";

        var depthUpBtn = AddElement<Button>(buttonsBlock, "depthButton");
        depthUpBtn.clicked += DepthUp;
        depthUpBtn.text = "^";

        var baseBlock = new VisualElement();
        baseBlock.AddToClassList("baseBlock");
        root.Add(baseBlock);
    }

    private void GetNewMap()
    {
        if (!EditorApplication.isPlaying)
            return;

        height = Field_Scr.mapHeight;
        width = Field_Scr.mapWidth;
        depth = Field_Scr.mapDepth;
        curDepth = 0;

        UpdateMapVisuals();
    }
    private void DepthUp()
    {
        if (curDepth == depth-1)
            return;
        curDepth++;
        UpdateMapVisuals();
    }
    private void DepthDown()
    {
        if (curDepth == 0)
            return;
        curDepth--;
        UpdateMapVisuals();
    }
    public void UpdateMapVisuals()
    {
        var baseBlock = root.ElementAt(1);
        baseBlock.Clear();

        var label = new Label(height.ToString() + "; " + width.ToString() + "; " + depth.ToString());
        baseBlock.Add(label);
        label = new Label("Current Depth: " + curDepth);
        baseBlock.Add(label);

        for (int i = 0; i < height; i++)
        {
            var row = AddElement<VisualElement>(baseBlock, "row");
            for (int j = 0; j < width; j++)
            {
                Vector3Int vePos = new Vector3Int(j, i, curDepth);
                VisualElement ve;
                Label lab;
                int pathVal;
                switch (Field_Scr.GetMapCell(vePos).objID) // TODO: поменять на айдишники из базы
                {
                    case -200:
                        AddElement<VisualElement>(row, "wallBlock");
                        break;
                    case -100:
                        AddElement<VisualElement>(row, "obstacleBlock");
                        break;
                    case 300:
                        AddElement<VisualElement>(row, "playerBlock");
                        break;
                    case 400:
                        ve = AddElement<VisualElement>(row, "enemyBlock");
                        pathVal = Field_Scr.GetMapCell(vePos).pathValue;
                        if (pathVal > 0)
                        {
                            lab = new Label(pathVal.ToString());
                            lab.style.backgroundColor = new Color(0, 0, 0, 0);
                            ve.Add(lab);
                        }
                        break;
                    default:
                        ve = AddElement<VisualElement>(row, "freeBlock");
                        pathVal = Field_Scr.GetMapCell(vePos).pathValue;
                        if (pathVal > 0)
                        {
                            lab = new Label(pathVal.ToString());
                            lab.style.backgroundColor = new Color(0, 0, 0, 0);
                            ve.Add(lab);
                        }
                        break;
                }
            }
        }
    }

    T AddElement<T>(VisualElement baseElem, string className) where T : VisualElement, new()
    {
        var elem = new T();
        elem.AddToClassList(className);
        baseElem.Add(elem);
        return elem;
    }
}