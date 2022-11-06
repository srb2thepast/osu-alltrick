using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace osuAT.Game.Skills
{
    /// <summary>
    /// Adds the method this attribute is applied to into the list of debug values in SkillAnalyzer.
    /// The method is attribute is applied to must return a tuple of <string,object>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class GetsDebugValueAttribute : Attribute
    {

    }

    /// <summary>
    /// Adds the field this attribute is applied into the list of debug values in SkillAnalyzer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class HiddenDebugValueAttribute : Attribute
    {

    }

}
