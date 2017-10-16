using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFractalGenerator : MonoBehaviour {

    public bool populate;
    public int difficulty;
    public int maxDepth;
    private int depth;
    public float childScale;

    public Mesh mesh;
    public Material material;

    private static Vector3[] childDirections = { Vector3.right, Vector3.left, Vector3.forward, Vector3.back };
    private static Quaternion[] childOrientation = { Quaternion.Euler(0f, 0f, -90f), Quaternion.Euler(0f, 0f, 90f), Quaternion.Euler(90f, 0f, 0f), Quaternion.Euler(-90f, 0f, 0f) };

    public Mesh[] meshes;
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
        if (depth == 0)
        {
            gameObject.AddComponent<MeshFilter>().mesh = meshes[Random.Range(0, meshes.Length)];
            gameObject.AddComponent<MeshRenderer>().material = materials[Random.Range(0, materials.Length)];
            transform.rotation = Quaternion.Euler(0, -45, 0);
        }
        else
        {
            gameObject.AddComponent<MeshFilter>().mesh = mesh;
            gameObject.AddComponent<MeshRenderer>().material = material;
        }

        if (depth < maxDepth)
        {
            CreateChildren();
        }
    }

    private void CreateChildren()
    {
        Mesh childMesh = meshes[Random.Range(0, meshes.Length)];
        Material childMat = materials[Random.Range(0, materials.Length)];
        for (int i = 0; i < childDirections.Length; i++)
        {
            new GameObject("Fractal Child").AddComponent<EnemyFractalGenerator>().Initialize(this, i, childMesh, childMat, true);
        }
    }

    public void SetValues(int _diffLevel, bool _populate)
    {
        difficulty = _diffLevel;
        maxDepth = Random.Range(1, difficulty);
        populate = _populate;
    }

    private void Initialize(EnemyFractalGenerator parent, int childIndex, Mesh _mesh, Material _mat, bool _populate = false)
    {
        populate = _populate;
        mesh = _mesh;
        material = _mat;
        meshes = parent.meshes;
        materials = parent.materials;
        maxDepth = parent.maxDepth;
        depth = parent.depth + 1;
        childScale = parent.childScale;
        transform.parent = parent.transform;
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = childDirections[childIndex] * (0.5f + 0.5f * childScale);
        transform.rotation = parent.transform.rotation;
        if(populate)
        {
            Populate();
        }
    }
}
