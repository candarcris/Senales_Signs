using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class GlitchAnim : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 _timeRange;
    [SerializeField] private float _timeWait = 0.2f;

    private Material _HologramMaterial;
    private int _hash_UseGlitch = Shader.PropertyToID("_UseGlitch");

    private void Start()
    {
        _HologramMaterial = GetComponent<Renderer>().material;
        StartCoroutine(StartGlitch());
    }

    private IEnumerator StartGlitch()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(_timeRange.x, _timeRange.y));
            _HologramMaterial.SetFloat(_hash_UseGlitch, 1);
            yield return new WaitForSeconds(_timeWait);
            _HologramMaterial.SetFloat(_hash_UseGlitch, 0);
        }
    }
}
