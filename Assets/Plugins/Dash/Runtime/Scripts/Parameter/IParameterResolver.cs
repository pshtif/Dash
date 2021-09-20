/*
 *	Created by:  Peter @sHTiF Stefcek
 */

namespace Dash
{
    public interface IParameterResolver
    {
        bool hasErrorInResolving { get; }
        
        string errorMessage { get; }
        
        object Resolve(string p_name, IAttributeDataCollection p_collection = null, bool p_referenced = false);
        
        //V Resolve<V>(string p_name, IAttributeDataCollection p_collection = null);
    }
}