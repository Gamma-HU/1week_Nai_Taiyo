using System;
using System.Reflection;
using UnityEngine;

public static class ExtensionMethods
{
    public static void CopyCollider2D(this Collider2D targetCollider, Collider2D sourceCollider)
    {

        if (targetCollider == null || sourceCollider == null)
        {
            Debug.LogError("Either target or source does not have a Collider2D component.");
            return;
        }

        Type type = sourceCollider.GetType();
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            field.SetValue(targetCollider, field.GetValue(sourceCollider));
        }
    }
}
