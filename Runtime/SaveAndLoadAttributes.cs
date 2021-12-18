using System;

namespace AarquieSolutions.SaveAndLoadSystem
{
    public enum KeyDoesNotExistReturnType
    {
        DoNotReturn,
        DefaultValue,
        ConstructorValue
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SaveDataAttribute : Attribute
    {
        public string key;
        public KeyDoesNotExistReturnType keyDoesNotExistReturnType;
        public string fileName;
        public string path;
        public SaveDataAttribute(string key,
            KeyDoesNotExistReturnType keyDoesNotExistReturnType = KeyDoesNotExistReturnType.DoNotReturn)
        {
            this.key = key;
            this.keyDoesNotExistReturnType = keyDoesNotExistReturnType;
        }

        public SaveDataAttribute(string key, KeyDoesNotExistReturnType keyDoesNotExistReturnType,
            string fileName = null,
            string path = null) : this(key, keyDoesNotExistReturnType)
        {
            this.fileName = fileName;
            this.path = path;
        }
    }
}

