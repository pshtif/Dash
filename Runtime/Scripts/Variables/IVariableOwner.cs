/*
 *	Created by:  Peter @sHTiF Stefcek
 */

namespace Dash
{
    public interface IVariableOwner
    {
        void MarkDirty();
        
        IVariableBindable Bindable { get; }
    }
}