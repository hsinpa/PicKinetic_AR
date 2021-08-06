using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace Hsinpa.UIStyle {
    [CreateAssetMenu(fileName = "UIStyleSheet", menuName = "UIStyle/StyleSheet", order = 1)]
    public class UIStylesheet : ScriptableObject
    {
        [SerializeField]
        private List<ColorSet> _ColorSets = new List<ColorSet>();
        public List<ColorSet> ColorSets => this._ColorSets;

        public ColorSet GetColorSet(string p_tag) {
            return _ColorSets.Find(x => x.tag == p_tag);
        }

        [System.Serializable]
        public struct ColorSet {
            public string tag;
            public Color color;

            public bool isValid => !string.IsNullOrEmpty(tag);
        }
    }
}
