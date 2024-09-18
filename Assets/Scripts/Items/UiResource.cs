using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Herman
{
    [Serializable]
    public class TypeSpritePair {
        public string type;
        public Sprite sprite;
    }

    [Serializable]
    public class TagSpritePair {
        public string tag;
        public Sprite sprite;
    }

    [CreateAssetMenu(fileName = "NewUiResource", menuName = "New UI Resource")]
    public class UiResource : ScriptableObject {
        public List<TypeSpritePair> typeSprites;
        public List<TagSpritePair> tagSprites;
        public List<Color> entityTypeColors;
        public List<Color> currencyColors;

        public Sprite GetSpriteByType(string type)
        {
            return this.typeSprites.FindAll(e => e.type == type).Select(e => e.sprite).FirstOrDefault();
        }

        public Sprite GetSpriteByTags(List<string> tags)
        {
            return this.tagSprites.FindAll(e => tags.LastOrDefault() == e.tag).Select(e => e.sprite).FirstOrDefault();
        }

        public Color GetColorByEntityType(EntityType entry)
        {
            if (this.entityTypeColors.Count == 3)
            {
                switch (entry)
                {
                    case EntityType.RESOURCE:
                        return this.entityTypeColors[0];

                    case EntityType.NEED:
                        return this.entityTypeColors[1];

                    default:
                        return this.entityTypeColors[2];
                }
            }
            else
            {
                Debug.Log("Expected exactly three different colors to indicate resource or need.");
                return Color.magenta;
            }
        }

        public Color GetColorByBalance(CurrencyValue entry)
        {
            if (this.currencyColors.Count == 3)
            {
                if (entry.value == 0)
                {
                    return this.currencyColors[2];
                }
                else
                {
                    switch (entry.GetCurrency())
                    {
                        case Currency.FIAT:
                            return this.currencyColors[0];

                        case Currency.Q:
                            return this.currencyColors[1];

                        default:
                            return Color.magenta;
                    }
                }
            }
            else
            {
                Debug.Log("Expected exactly three different colors to indicate balance and currency.");
                return Color.magenta;
            }
        }
    }
}
