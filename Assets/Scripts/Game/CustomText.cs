using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MyStuff;

[ExecuteInEditMode]
public class CustomText : MonoBehaviour {
    #region Variables
    public string text;
    public WordAlignment alignment = WordAlignment.Right;
    public float size = 10.0f;
    public Letters letters = new Letters();
    GameObject[] instLetters = new GameObject[0];
    MaterialPropertyBlock material;
    new SpriteRenderer renderer;

    [System.Serializable]
    public enum WordAlignment { Right, Center, Left }
    [System.Serializable]
    public class Letters {
        public GameObject letterBase;
        public Sprite[] A;
        public Sprite[] B;
        public Sprite[] C;
        public Sprite[] D;
        public Sprite[] E;
        public Sprite[] F;
        public Sprite[] G;
        public Sprite[] H;
        public Sprite[] I;
        public Sprite[] J;
        public Sprite[] K;
        public Sprite[] L;
        public Sprite[] M;
        public Sprite[] N;
        public Sprite[] O;
        public Sprite[] P;
        public Sprite[] Q;
        public Sprite[] R;
        public Sprite[] S;
        public Sprite[] T;
        public Sprite[] U;
        public Sprite[] V;
        public Sprite[] W;
        public Sprite[] X;
        public Sprite[] Y;
        public Sprite[] Z;
        public Sprite[] charS; //space
        public Sprite[] charE; //exclaimation point
        public Sprite[] charQ; //question mark
        public Sprite[] charP; //period
        public Sprite[] charC; //comma

        public bool ContainsInvalid(char[] chars) {
            foreach(char c in chars) {
                Sprite[] letterArray = GetLetterArray(c);
                if(letterArray == null || letterArray.Length == 0)
                    return true;
            }
            return false;
        }
        
        public GameObject MakeLetter(char c) {
            GameObject letter = Instantiate(letterBase);
            letter.GetComponent<SpriteRenderer>().sprite = MyFunctions.RandomItem(GetLetterArray(c));
            return letter;
        }

        public Sprite[] GetLetterArray(char c) {
            c = char.ToUpper(c);
            switch(c) {
                case 'A': return A;
                case 'B': return B;
                case 'C': return C;
                case 'D': return D;
                case 'E': return E;
                case 'F': return F;
                case 'G': return G;
                case 'H': return H;
                case 'I': return I;
                case 'J': return J;
                case 'K': return K;
                case 'L': return L;
                case 'M': return M;
                case 'N': return N;
                case 'O': return O;
                case 'P': return P;
                case 'Q': return Q;
                case 'R': return R;
                case 'S': return S;
                case 'T': return T;
                case 'U': return U;
                case 'V': return V;
                case 'W': return W;
                case 'X': return X;
                case 'Y': return Y;
                case 'Z': return Z;
                case ' ': return charS;
                case '!': return charE;
                case '?': return charQ;
                case '.': return charP;
                case ',': return charC;
                default: return charS;
            }
        }
    }
    #endregion

    #region Update
    private void OnValidate() { update = true; }
    private void Awake() {
        material = new MaterialPropertyBlock();
        instLetters = MyFunctions.GetChildren(gameObject);
    }

    bool update = false;
    private void Update() {
        if(update) UpdateText(text);
        update = false;
    }
    #endregion

    #region Functions
    public void UpdateText() { UpdateText(text); }
    public void UpdateText(string text) {
        char[] characters = text.ToCharArray();
        if(!letters.ContainsInvalid(characters)) {
            foreach(GameObject letter in instLetters) DestroyImmediate(letter);

            this.text = text;
            Vector2 start = new Vector2(0, 0);
            float length = characters.Length * size;

            switch(alignment) {
                case WordAlignment.Right:
                    start.x = size / 2;
                    break;
                case WordAlignment.Center:
                    start.x = size / 2 - (length / 2);
                    break;
                case WordAlignment.Left:
                    start.x = size / 2 - length;
                    break;
            }

            instLetters = new GameObject[characters.Length];
            for(int j = 0; j < characters.Length; j++) {
                instLetters[j] = letters.MakeLetter(characters[j]);
                instLetters[j].transform.parent = gameObject.transform;
                instLetters[j].transform.localPosition = start;
                start.x += size;
            }
        }
        else Debug.LogError("'UpdateText(string text)' passed a character that had no definition within 'letters'.");
    }

    private bool _highlighted = false;
    public bool highlighted {
        get { return _highlighted; }
        set {
            if(value != _highlighted && material != null) {
                _highlighted = value;

                int hl = 0;
                if(value) hl = 1;

                foreach(GameObject letter in instLetters) {
                    renderer = letter.GetComponent<SpriteRenderer>();
                    renderer.GetPropertyBlock(material);
                    material.SetInt("_Highlighted", hl);
                    renderer.SetPropertyBlock(material);
                }
            }
        }
    }
    #endregion
}
