using UnityEditor;
using UnityEngine;

public class MaterialShape : ScriptableObject {
    public bool useSlope = true;
    // universal
    public GameObject floor;
    public GameObject revcorner;
    public GameObject corner;
    public GameObject revcornerMoulding;
    public GameObject revcornerGutter;
    public GameObject cornerMoulding;
    public GameObject cornerGutter;
    public GameObject endMoulding;
    public GameObject endGutter;
    public GameObject tunnelPillarSlant;
    // slope
    public GameObject lowerSlope;
    public GameObject upperSlope;
    public GameObject lowerCurve;
    public GameObject upperCurve;
    public GameObject tunnelCurve;
    public GameObject tunnelSlope;
    public GameObject tunnelSlopeDouble;
    public GameObject tunnelSlopeLedge;
    // thin noslope
    public GameObject end;
    public GameObject thinSlope;
    public GameObject thinSpiral;
}

public class MakeScriptableObject {
    [MenuItem("Assets/Create/MaterialShape", false)]
    public static void CreateMyAsset() {
        MaterialShape asset = ScriptableObject.CreateInstance<MaterialShape>();

        GameObject context = (GameObject)Selection.activeObject;

        string path =  "Assets/New Material Shape.asset";
        if (context != null) {
            path = AssetDatabase.GetAssetPath(context);
            path = path.Substring(0, path.Length - 4) + ".asset";
            asset.floor = context.transform.Find("Floor")?.gameObject;
            asset.revcorner = context.transform.Find("Revcorner")?.gameObject;
            asset.corner = context.transform.Find("Corner")?.gameObject;
            asset.revcornerMoulding = context.transform.Find("Revcorner Baseboard")?.gameObject;
            asset.revcornerMoulding = context.transform.Find("Revcorner Moulding")?.gameObject;
            asset.revcornerGutter = context.transform.Find("Revcorner Gutter")?.gameObject;
            asset.cornerMoulding = context.transform.Find("Corner Baseboard")?.gameObject;
            asset.cornerMoulding = context.transform.Find("Corner Moulding")?.gameObject;
            asset.cornerGutter = context.transform.Find("Corner Gutter")?.gameObject;
            asset.endMoulding = context.transform.Find("End Baseboard")?.gameObject;
            asset.endGutter = context.transform.Find("End Gutter")?.gameObject;
            asset.tunnelPillarSlant = context.transform.Find("Tunnel Pillar Slant")?.gameObject;
            asset.lowerSlope = context.transform.Find("Lower Slope")?.gameObject;
            asset.upperSlope = context.transform.Find("Upper Slope")?.gameObject;
            asset.lowerCurve = context.transform.Find("Lower Curve")?.gameObject;
            asset.upperCurve = context.transform.Find("Upper Curve")?.gameObject;
            asset.tunnelCurve = context.transform.Find("Tunnel Curve")?.gameObject;
            asset.tunnelSlope = context.transform.Find("Tunnel Slope")?.gameObject;
            asset.tunnelSlopeDouble = context.transform.Find("Tunnel Slope Double")?.gameObject;
            asset.tunnelSlopeLedge = context.transform.Find("Tunnel Slope Ledge")?.gameObject;
            asset.end = context.transform.Find("End")?.gameObject;
            asset.thinSlope = context.transform.Find("Thin Slope")?.gameObject;
            asset.thinSpiral = context.transform.Find("Thin Spiral")?.gameObject;
        }

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}