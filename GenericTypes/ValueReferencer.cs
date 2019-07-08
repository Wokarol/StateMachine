using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueReferencer<T>
{
    public T Value { get; set; }
    public bool Valid => !typeof(T).IsClass || Value != null;
}
