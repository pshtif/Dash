/*
 *	Created by:  Peter @sHTiF Stefcek
 */

namespace Dash
{
    public class StoreStateNodeModel : RetargetNodeModelBase
    {
        public StorageType storageType = StorageType.VARIABLE;
        
        public string storageName = "";
        
        public Parameter<bool> includeTarget = new Parameter<bool>(true);

        public bool storeActive = false;

        public bool storePosition = false;
    }
}