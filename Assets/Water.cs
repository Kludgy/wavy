using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Water : MonoBehaviour
{
    public GameObject waterAtomPrefab;
    public int count = 101;
    public float spacing = 1;
    public float amplitude = 0.25f;
    public float phase = 0;
    public float speed = 3f;
    
    (int i, GameObject atom)[] atoms;
    float currentTime;

    // Start is called before the first frame update
    void Start()
    {
        atoms = Enumerable
            .Range(0, count)
            .Select(
                i =>
                {
                    var atom = Instantiate(
                        waterAtomPrefab,
                        Vector3.zero,
                        Quaternion.identity
                    );
                    atom.transform.parent = transform;
                    atom.transform.localPosition = atomPos(i, 0);
                    return (i, atom);
                }
            )
            .ToArray();
    }

    float atomX(int i) => (float)i * spacing - (float)count * spacing / 2f;
    float atomHeight(float x, float t) => (float)Math.Sin(phase + x + t * speed) * amplitude;
    Vector3 atomPos(int i, float t) => new Vector3(atomX(i), atomHeight(atomX(i), t), 0);

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        foreach (var atom in atoms)
        {
            atom.atom.transform.localPosition = atomPos(atom.i, currentTime);
        }
    }
}
