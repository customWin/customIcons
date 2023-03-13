using System;

namespace customIcons.Exceptions
{
    [Serializable]
    class customIconPackException : Exception
    {
        public customIconPackException() { }

        public customIconPackException(string name)
            : base(String.Format("Could not parse customIconPack: {0}", name))
        {

        }
    }
}