using System;
using System.Collections.Generic;

namespace Runemark.VisualEditor
{

    public interface ITypedNode 
    {
        List<Type> AllowedTypes { get; }
        Type Type { get; set; }

        Action OnTypeChanged { get; set; }

    }
}
