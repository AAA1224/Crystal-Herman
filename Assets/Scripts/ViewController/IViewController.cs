using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace KoboldTools
{
    public interface IViewController
    {
        void onSetModel(System.Object newModel);
        void onRemoveModel(System.Object modelTypeToRemove);
    }
}
