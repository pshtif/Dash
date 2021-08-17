/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;

namespace Dash
{
    [Serializable]
    public class DashPrefabInfo
    {
        public string name;

        public bool enablePooling = false;
        
        public int count;

        public bool prewarm = false;

        static public DashPrefabInfo GetDefault()
        {
            var instance = new DashPrefabInfo();
            instance.name = "default";
            instance.count = 1;

            return instance;
        }
    }
}