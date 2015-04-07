///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2015 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

namespace com.espertech.esper.epl.join.hint
{
    public class IndexHintInstructionIndexName : IndexHintInstruction
    {
        public IndexHintInstructionIndexName(String indexName)
        {
            IndexName = indexName;
        }

        public string IndexName { get; private set; }
    }
}