using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DitherEvent : TriggerEvent
{
    [SerializeField] private Material _ditherMaterial;
    [SerializeField] private List<Material> _ditherMaterials;
    [SerializeField] private MeshRenderer _ditherMesh;
    [SerializeField] private List<MeshRenderer> _ditherMeshes;
    [SerializeField] private float _ditherOpacity;
    [SerializeField] private int _ditherMeshIndex;
    private int _hash_dither = Shader.PropertyToID("_OPACITY");
    [SerializeField] private bool _userIndex;

    private void Start()
    {
        if (_userIndex)
        {
            _ditherMaterial = _ditherMesh.materials[_ditherMeshIndex];
        }
        foreach (var mesh in _ditherMeshes)
        {
            _ditherMaterials.Add(mesh.materials[0]);
        }
        _ditherMaterial.SetFloat(_hash_dither, _ditherOpacity);
    }

    protected override void DoTriggerEvent()
    {
        _ditherMaterial.SetFloat(_hash_dither, 0.5f);
        foreach (var mesh in _ditherMeshes)
        {
            mesh.materials[0].SetFloat(_hash_dither, 0.5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            DoTriggerEvent();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            _ditherMaterial.SetFloat(_hash_dither, 1f);
            foreach (var mesh in _ditherMeshes)
            {
                mesh.materials[0].SetFloat(_hash_dither, 1f);
            }
        }
    }
}