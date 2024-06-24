using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    public enum Language
    {
        Kr,
        En,
    }
    
    public class Languages
    {
        public string kr;
        public string en;

        public Languages(string _kr, string _en)
        {
            kr = _kr;
            en = _en;
        }
    }

    public List<Languages> languageList = new List<Languages>()
    {
        new Languages("열쇠1", "Key"),
        new Languages("전구", "Lightbulb"),
        new Languages("열쇠2", "Zigzag"),
        new Languages("무엇인지 알 수 없어 오브젝트로 변환합니다.", "I don't Know what it is, so change the item to an object"),
        new Languages("{0}인가?", "{0}?"),
    };

    public string ReturnTranslate(int idx)
    {
        switch (playInfo.language)
        {
            case Language.Kr:
                return languageList[idx].kr;
            case Language.En:
                return languageList[idx].en;
        }

        GameManager.ShowLog("Translate Error", Color.red);
        return "Translate Error";
    }
}
