using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attribute for setting a vector by clicking on the screen in editor mode.
/// </summary>
public class VectorPickerAttribute : PropertyAttribute {
    public readonly bool relative;

    /// <summary>Works a lot like the color picker, except for vectors.</summary>
    /// <param name="relative">Make the final vector relative the transform?</param>
    public VectorPickerAttribute(bool relative = false) {
        this.relative = relative;
    }
}
