using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kensai.Util.Extensions {
    public static class ObjectExtensions {
        public static void DestroyAll(this UnityEngine.Object instance, GameObject obj) {
            var children = new List<GameObject>();
            foreach (Transform child in obj.transform) children.Add(child.gameObject);
            children.ForEach(child => UnityEngine.Object.Destroy(child));
            UnityEngine.Object.Destroy(obj);
        }

        public static void DestroyAll(this GameObject instance) {
            var children = new List<GameObject>();
            foreach (Transform child in instance.transform) children.Add(child.gameObject);
            children.ForEach(child => UnityEngine.Object.Destroy(child));
            UnityEngine.Object.Destroy(instance);
        }

        public static void SetLayerRecursively(this GameObject instance, int layer) {
            instance.layer = layer;

            foreach (Transform child in instance.transform) {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        public static Transform Search(this Transform target, string name) {
            if (target.name == name) return target;

            for (int i = 0; i < target.childCount; ++i) {
                var result = Search(target.GetChild(i), name);
                if (result != null) return result;
            }

            return null;
        }
    }
}
