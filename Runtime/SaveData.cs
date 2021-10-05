using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum KeyDoesNotExistReturnType { DoNotReturn, DefaultValue, ConstructorValue}

[AttributeUsage(AttributeTargets.Field)]
public class SaveData : PropertyAttribute 
{
    public string key;
    public KeyDoesNotExistReturnType keyDoesNotExistReturnType;
    public string fileName;
    public string path;

    public SaveData(string key, KeyDoesNotExistReturnType keyDoesNotExistReturnType = KeyDoesNotExistReturnType.DoNotReturn)
    {
        this.key = key;
        this.keyDoesNotExistReturnType = keyDoesNotExistReturnType;
    }

    public SaveData(string key, KeyDoesNotExistReturnType keyDoesNotExistReturnType, string fileName = null, string path = null) : this(key, keyDoesNotExistReturnType)
    {
        this.fileName = fileName;
        this.path = path;
       
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class ContainsDataToBeSaved : Attribute {}

