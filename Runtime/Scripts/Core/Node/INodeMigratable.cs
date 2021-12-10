/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;

namespace Dash
{
    public interface INodeMigratable
    {
        void Migrate();

        Type GetMigrateType();
    }
}