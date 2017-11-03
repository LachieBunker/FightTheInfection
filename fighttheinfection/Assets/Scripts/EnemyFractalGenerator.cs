using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFractalGenerator : MonoBehaviour {

    public bool populate;
    public int difficulty;
    public int maxDepth;
    private int depth = -1;
    public float childScale;

    public GameObject shapeObject;
    public Material material;

    public Vector3[] childDirections;// = { Vector3.right, Vector3.left, Vector3.forward, Vector3.back };
    public ChildPos[] childPos;// = { Quaternion.Euler(0f, 0f, -90f), Quaternion.Euler(0f, 0f, 90f), Quaternion.Euler(90f, 0f, 0f), Quaternion.Euler(-90f, 0f, 0f) };

    public GameObject[] shapes;
    public Material[] materials;

    // Use this for initialization
    void Start()
    {
        /*
        if(depth == 0 && populate)
        {
            gameObject.AddComponent<MeshFilter>().mesh = meshes[Random.Range(0, meshes.Length)];
            gameObject.AddComponent<MeshRenderer>().material = materials[Random.Range(0, materials.Length)];
        }
        else if(populate)
        {
            gameObject.AddComponent<MeshFilter>().mesh = mesh;
            gameObject.AddComponent<MeshRenderer>().material = material;
        }
        
        if (depth < maxDepth && populate == true)
        {
            CreateChildren();
        }*/
    }

    public void Populate()
    {
        if (depth == -1)
        {
            //gameObject.AddComponent<MeshFilter>().mesh = shapes[Random.Range(0, shapes.Length)];
            //gameObject.AddComponent<MeshRenderer>().material = materials[Random.Range(0, materials.Length)];
            transform.rotation = Quaternion.Euler(0, -45, 0);
        }
        else
        {
            //gameObject.AddComponent<MeshFilter>().mesh = shapeObject;
            gameObject.AddComponent<MeshRenderer>().material = material;
        }

        if (depth < maxDepth)
        {
            CreateChildren();
        }
    }

    private void CreateChildren()
    {
        GameObject childMesh = shapes[Random.Range(0, shapes.Length)];
        Material childMat = materials[Random.Range(0, materials.Length)];
        if(depth == -1)
        {
            Instantiate(childMesh, transform.position, Quaternion.identity).GetComponent<EnemyFractalGenerator>().InitializeBaseFractal(this, childMesh, childMat, true);
        }
        else
        {
            for (int i = 0; i < childDirections.Length; i++)
            {
                if(depth <= 0)
                {
                    Instantiate(childMesh, childPos[i].transform, Quaternion.identity).GetComponent<EnemyFractalGenerator>().Initialize(this, i, childMesh, childMat, true);
                }
                else if(depth > 0 && childPos[i].transform.x >= 0)
                {
                    Instantiate(childMesh, childPos[i].transform, Quaternion.identity).GetComponent<EnemyFractalGenerator>().Initialize(this, i, childMesh, childMat, true);
                }
                
            }
        }
        
    }

    public void SetValues(int _diffLevel, bool _populate)
    {
        difficulty = _diffLevel;
        maxDepth = Random.Range(1, difficulty);
        populate = _populate;
    }

    private void Initialize(EnemyFractalGenerator parent, int childIndex, GameObject _shape, Material _mat, bool _populate = false)
    {
        populate = _populate;
        shapeObject = _shape;
        material = _mat;
        shapes = parent.shapes;
        materials = parent.materials;
        maxDepth = parent.maxDepth;
        depth = parent.depth + 1;
        childScale = parent.childScale;
        transform.parent = parent.transform;
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = parent.childPos[childIndex].transform * (0.5f + 0.5f * childScale);
        transform.rotation = parent.transform.rotation;
        transform.localRotation = Quaternion.Euler(parent.childPos[childIndex].orientation.x, parent.childPos[childIndex].orientation.y, parent.childPos[childIndex].orientation.z);
        if(populate)
        {
            Populate();
        }
    }

    private void InitializeBaseFractal(EnemyFractalGenerator parent, GameObject _shape, Material _mat, bool _populate = false)
    {
        populate = _populate;
        shapeObject = _shape;
        material = _mat;
        shapes = parent.shapes;
        materials = parent.materials;
        maxDepth = parent.maxDepth;
        depth = parent.depth + 1;
        childScale = parent.childScale;
        transform.parent = parent.transform;
        transform.rotation = parent.transform.rotation;
        if (populate)
        {
            Populate();
        }
    }
}

[System.Serializable]
public struct ChildPos
{
    public Vector3 transform;
    public Vector3 orientation;
}
