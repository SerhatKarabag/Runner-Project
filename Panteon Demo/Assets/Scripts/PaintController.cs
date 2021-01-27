using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PaintController : MonoBehaviour
{
    /* Singleton design pattern */
    public static PaintController instance;

    private Mesh paintMesh; // Mesh of wall.
    private Vector3[] paintVertices; // Vertices of mesh. 
    private Color[] verticesColor; // Color array of vertices.
    
    /* Floats */
    private float vertexToPaint; // count of vertex we will paint.
    private float paintedVertex; // count of vertex we painted.

    private void Awake()
    {
        SingletonDesignPattern(); // One instance will be enough.
        instance.enabled = false; // Enable when parkour end.
    }
    private void Start()
    {
        paintMesh = GetComponent<MeshFilter>().mesh; // Get the wall mesh which we will paint.
        paintVertices = paintMesh.vertices; // Get the vertices of mesh.
        vertexToPaint = paintVertices.Length; // We will use this to show the percentage of painted wall.
        paintedVertex = 0; // We haven't painted the wall yet.
        verticesColor = new Color[paintVertices.Length]; // Need color for each vertex.
        DefaultWallColor(Color.white); // Set wall color to white as default.
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Send ray from camera view on mouse position.
            if (Physics.Raycast(ray, out RaycastHit hit, 100f)) // Max range is 100f.
            {
                if (hit.transform.CompareTag("Paint"))
                {
                    int[] triangles = paintMesh.triangles; // Triangles is an integer array which holds vertex index of triangles. 
                    /*
                     For example, we hit 50th triangle.
                     The vertex point indexes of 50th triangle are 150,151,152.  
                     */
                    int vertexIndexFirst = triangles[hit.triangleIndex * 3 + 0];
                    int vertexIndexSecond = triangles[hit.triangleIndex * 3 + 1];
                    int vertexIndexThird = triangles[hit.triangleIndex * 3 + 2];
                    int[] vertexArray = new int[3] { vertexIndexFirst, vertexIndexSecond, vertexIndexThird };
                    CalculatePaintPercantage(vertexArray);
                    /* Paint these vertex to red */
                    verticesColor[vertexIndexFirst] = Color.red;
                    verticesColor[vertexIndexSecond] = Color.red;
                    verticesColor[vertexIndexThird] = Color.red;
                    /* Change the color array of wall mesh */
                    paintMesh.colors = verticesColor; 
                }
            }
        }
    }

    private void DefaultWallColor(Color color) // Helper function to reset wall color.
    {
        for (int i = 0; i < verticesColor.Length; i++)
        {
            verticesColor[i] = color;
        }
        paintMesh.colors = verticesColor;
    }

    private void CalculatePaintPercantage(int[] vertices) // Calculates what percentage of the wall is painted.
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            if (verticesColor[vertices[i]] == Color.white) // Check if white right now. This vertice will be red. So its not wrong to say "it was white". 
            {
                paintedVertex++; // increase painted vertex count.
                float percentage = paintedVertex / vertexToPaint; // calculate percentage.
                UIController.instance.PercentageFillingImage.fillAmount = percentage; // Fill the image.
                UIController.instance.PercentageFillingImage.color = Color.Lerp(Color.white, Color.red, percentage); // Color will turn white to red between 0-100.
                UIController.instance.PercentageText.text = "% " +  (int)(percentage * 100); //  Write it to ui text. 
                if (percentage==1)
                {
                    StartCoroutine(GodJobDude()); // Writes "good job" instead of percentage.
                }
            }
        }
    }

    private IEnumerator GodJobDude() // Some sense of humor.
    {
        yield return new WaitForSeconds(1f);
        UIController.instance.PercentageText.text = "GOOD JOB!";
        yield return new WaitForSeconds(1f);
        UIController.instance.playButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "NEXT"; // I used one button. So i change text of button to use dynamicly. 
        UIController.instance.Menu.SetActive(true); // Activate menu panel.
    }
    private void SingletonDesignPattern()
    {
        if (PaintController.instance == null)
        {
            PaintController.instance = this;
        }
        else
        {
            if (PaintController.instance != this)
            {
                Destroy(this);
            }
        }
    }
}
