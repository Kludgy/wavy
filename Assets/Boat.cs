using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Collections;

public class Boat : MonoBehaviour
{
    public GameObject boatAtomPrefab;
    public GameObject paddlerPrefab;
    public Water water;
    public int seatRows = 10;
    public float rowSpacing = 1;
    public int bowRows = 2;
    public float  bowHeight = 0.5f;
    public int sternRows = 2;
    public float sternHeight = 0.5f;
    public float paddlerHeightOffset = 1;
    public float paddlerDepthOffset = -0.6f;
    public float paddlerDrive = 1;
    public float groupDrivePower = 1.2f;
    public float dragCoefficient = 0.5f;

    (int i, GameObject atom)[] atoms;
    (int i, GameObject paddler)[] paddlers;

    [ReadOnly]
    public float speed;

    int totalRows => seatRows + bowRows + sternRows;
    int numPaddlersInDrivePhase => paddlers.Where(paddler => paddler.paddler.GetComponent<Paddler>().isInDrivePhase).Count();

    // Start is called before the first frame update
    void Start()
    {
        var paddleActionKeys = new[] { "1", "1", "1", "1", "2", "2", "2", "3", "3", "3" };

        atoms = Enumerable
            .Range(0, seatRows+bowRows+sternRows)
            .Select(
                i =>
                {
                    var atom = Instantiate(
                        boatAtomPrefab,
                        Vector3.zero,
                        Quaternion.identity
                    );
                    atom.transform.parent = transform;
                    atom.transform.localPosition = atomPos(i);
                    return (i, atom);
                }
            )
            .ToArray();

        paddlers = Enumerable
            .Range(0, seatRows)
            .Select(
                i =>
                {
                    var paddler = Instantiate(
                        paddlerPrefab,
                        Vector3.zero,
                        Quaternion.identity
                    );
                    paddler.transform.parent = transform;
                    paddler.transform.localPosition = paddlerPos(i);
                    paddler.GetComponent<Paddler>().actionKeyName = paddleActionKeys[i % paddleActionKeys.Length];
                    return (i, paddler);
                }
            )
            .ToArray();
    }

    float paddlerX(int i) => (float)i * rowSpacing - (float)totalRows / 2f + (float)sternRows * rowSpacing;
    Vector3 paddlerPos(int i) => new Vector3(paddlerX(i), paddlerHeightOffset, paddlerDepthOffset);

    float atomX(int i) => (float)i * rowSpacing - (float)totalRows / 2f;
    float atomY(int i)
    {
        if (i < sternRows)
        {
            return sternHeight;
        }
        else if (i >= sternRows + seatRows)
        {
            return bowHeight;
        }
        else
        {
            return 0;
        }
    }
    Vector3 atomPos(int i) => new Vector3(atomX(i), atomY(i), 0f);

    // Update is called once per frame
    void Update()
    {
        var boatDrive = (float)Math.Pow((double)numPaddlersInDrivePhase * paddlerDrive, this.groupDrivePower);

        speed += boatDrive * Time.deltaTime;
        speed *= (float)Math.Pow(dragCoefficient, Time.deltaTime);

        water.phase = water.phase + water.spacing * speed * Time.deltaTime;
    }
}
