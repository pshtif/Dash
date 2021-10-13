/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;

namespace Dash
{
    [Serializable]
    public class PrefabInfo
    {
        public string name;

        public bool enablePooling = false;
        
        public int count;

        public bool prewarm = false;

        static public PrefabInfo GetDefault()
        {
            var instance = new PrefabInfo();
            instance.name = "default";
            instance.count = 1;

            return instance;
        }
    }
}