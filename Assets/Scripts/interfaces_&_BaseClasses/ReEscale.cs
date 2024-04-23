using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReEscale
{
    public static float Normalize(float valorObjetivo, float rangoMinOriginal, float rangoMaxOriginal, float rangoMinNuevo, float rangoMaxNuevo)
    {
        return (valorObjetivo - rangoMinOriginal) * (rangoMaxNuevo - rangoMinNuevo) / (rangoMaxOriginal - rangoMinOriginal) + rangoMinNuevo;
    }
}
