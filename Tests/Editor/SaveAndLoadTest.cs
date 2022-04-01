using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AarquieSolutions.SaveAndLoadSystem;
using NUnit.Framework.Internal;

public class SaveAndLoadTest
{
    [Test]
    public void SavingAndLoadingStringData()
    {
        SaveLoadSystem.Save("Test", "Test string");
        Assert.AreEqual("Test string", SaveLoadSystem.Load("Test")); 
    }
    
    [Test]
    public void SavingAndLoadingDateAndTimeData()
    {
        DateTime dateTime = new DateTime(2020,8, 25, 0, 1, 1);
        
        SaveLoadSystem.Save("TestDateTime", dateTime);
        DateTime loadedDateTime = (DateTime)SaveLoadSystem.Load("TestDateTime");
        
        Assert.AreEqual(dateTime, loadedDateTime); 
    }

    [Test]
    public void SavingAndLoadingStringDataUsingCustomPath()
    {
        SaveLoadSystem.Save("CustomPath", "Test custom path","TestFileName","TestPathName");
        Assert.AreEqual("Test custom path", SaveLoadSystem.Load("CustomPath")); 
    }
    
    [Test]
    public void LoadingNotExistentData()
    {
        Assert.AreEqual("", SaveLoadSystem.Load("AKeyThatDoesn'tExists", "")); 
    }

    [Test]
    public void DeleteKey()
    {
        string key = "DeleteKey";
        SaveLoadSystem.Save(key, "Delete Test string");
        Assert.AreEqual("Delete Test string", SaveLoadSystem.Load(key)); 
        
        SaveLoadSystem.Delete(key);
        Assert.AreEqual("Load Default Value",SaveLoadSystem.Load(key, "Load Default Value"));
    }
}
