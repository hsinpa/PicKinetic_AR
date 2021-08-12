using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PicKinetic.Model
{
    public abstract class IModelManager : MonoBehaviour
    {
        public List<object> models;

        public virtual void SetUp() {

        }

        public T GetModel<T>()
        {
            return (T)models.Find(x => x.GetType() == typeof(T));
        }
    }
}