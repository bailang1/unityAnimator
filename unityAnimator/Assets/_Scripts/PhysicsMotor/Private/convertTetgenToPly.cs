#if UNITY_EDITOR
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class convertTetgenToPly : MonoBehaviour
{
    //Class that convert a .face and .node to a .ply to be able of being read by 3ds max to set colors

    #region InEditorVariables
    public TextAsset nodeText;
    public TextAsset facesText;
    public TextAsset outputText;
    #endregion

    private float[] nodesPos;
    private int[] facesIndex;
    private int numberOfFaces;
    private int numberOfNodes;

    void Start()
    {
        string[] lines = Regex.Split(nodeText.text, "\\s*\\n+\\s*");
        string[] elements = Regex.Split(lines[0], "\\s+");
        numberOfNodes = System.Int32.Parse(elements[0]);
        nodesPos = new float[3 * numberOfNodes];
        for (int i = 1; i < lines.Length; i++)
        {
            if (!(lines[i].StartsWith("#")))
            { 
                elements = Regex.Split(lines[i], "\\s+");
                if (elements.Length >= 4)
                {
                    int actualNodePos = System.Int32.Parse(elements[0]) * 3;
                    nodesPos[actualNodePos] = float.Parse(elements[1]);
                    nodesPos[actualNodePos + 1] = float.Parse(elements[2]);
                    nodesPos[actualNodePos + 2] = float.Parse(elements[3]);
                }
                else
                {
                    print("Unexpected format in line: " + i + ": " + lines[i]);
                }
            }
        }

        lines = Regex.Split(facesText.text, "\\s*\\n+\\s*");
        elements = Regex.Split(lines[0], "\\s+");
        numberOfFaces = System.Int32.Parse(elements[0]);
        facesIndex = new int[3 * numberOfFaces];
        for (int i = 1; i < lines.Length; i++)
        {
            if (!(lines[i].StartsWith("#")))
            {
                elements = Regex.Split(lines[i], "\\s+");
                if (elements.Length >= 4)
                {
                    int actualFacePos = System.Int32.Parse(elements[0]) * 3;
                    facesIndex[actualFacePos] = System.Int32.Parse(elements[1]);
                    facesIndex[actualFacePos + 1] = System.Int32.Parse(elements[2]);
                    facesIndex[actualFacePos + 2] = System.Int32.Parse(elements[3]);
                }
                else

                    print("Unexpected format in line: " + i + ": " + lines[i]);
            }
        }

        string result =
            "ply" +
            "\nformat ascii 1.0" +
            "\nelement vertex " + numberOfNodes +
            "\nproperty float x" +
            "\nproperty float y" +
            "\nproperty float z" +
            "\nelement face " + numberOfFaces +
            "\nproperty list uchar int vertex_index" +
            "\nend_header";
        for (int i = 0, l = nodesPos.Length-2; i < l; i+=3)
        {
            result += "\n" + nodesPos[i] + " " + nodesPos[i + 1] + " " + nodesPos[i + 2];
        }
        for (int i = 0, l = facesIndex.Length-2; i < l; i += 3)
        {
            result += "\n3 " + facesIndex[i] + " " + facesIndex[i + 1] + " " + facesIndex[i + 2];
        }
        File.WriteAllLines(UnityEditor.AssetDatabase.GetAssetPath(outputText), Regex.Split(result, "\\n"));
        UnityEditor.EditorUtility.SetDirty(outputText);
        print("conversion finalizada");
    }

}
#endif
