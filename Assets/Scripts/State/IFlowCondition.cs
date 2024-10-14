using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace KoboldTools
{
    public interface IFlowCondition
    {
        bool conditionMet { get; }
    }
}
