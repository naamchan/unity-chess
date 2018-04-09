using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessTile : MonoBehaviour
{
    public int col;
    public int row;

    private Material permanentMaterial;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetPermanentMaterial(Material material)
    {
        permanentMaterial = material;
        meshRenderer.material = material;
    }

    public void SetTemporaryMaterial(Material material)
    {
        meshRenderer.material = material;
    }

    public void ResetMaterial()
    {
        meshRenderer.material = permanentMaterial;
    }
}
