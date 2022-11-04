/*
 *	Created by:  Peter @sHTiF Stefcek
 */

namespace Dash
{
    public interface IInternalVariablesAccess
    {
        void AddVariable(Variable p_variable);
        void InvalidateLookup();
    }
}