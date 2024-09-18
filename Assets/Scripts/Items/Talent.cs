using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KoboldTools;

namespace Herman
{
    public interface ITalent : IItem
    {
        string Type { get; set; }
        List<string> Tags { get; set; }
    }

    [Serializable]
    public class Talent : ITalent, IEquatable<Talent>
    {
        [SerializeField] private string title;
        [SerializeField] private string description;
        [SerializeField] private int id;
        [SerializeField] private string type;
        [SerializeField] private List<string> tags;

        public string Type
        {
            get
            {
                return this.type;
            }

            set
            {
                this.type = value;
            }
        }
        public List<string> Tags
        {
            get
            {
                return this.tags;
            }

            set
            {
                this.tags = value;
            }
        }
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }
        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }
        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.title = value;
            }
        }
        public string LocalisedTitle
        {
            get
            {
                if (Localisation.instance.hasTextId(this.title))
                {
                    return Localisation.instance.getLocalisedText(this.title);
                }
                else
                {
                    return this.title;
                }
            }
        }

        public string LocalisedDescription
        {
            get
            {
                if (Localisation.instance.hasTextId(this.description))
                {
                    return Localisation.instance.getLocalisedText(this.description);
                }
                else
                {
                    return this.description;
                }
            }
        }

        public bool EquivalentTags(List<string> other)
        {
            return this.Tags.SequenceEqual(other);
        }
        public bool Equals(Talent other)
        {
            bool idEq = this.Id == other.Id;
            bool titleEq = this.Title == other.Title;
            bool descrEq = this.Description == other.Description;
            bool tyEq = this.Type == other.Type;
            bool tagEq = this.EquivalentTags(other.Tags);

            return idEq && titleEq && descrEq && tyEq && tagEq;
        }
        public override string ToString()
        {
            return String.Format("Talent(i={0}, t={1}, d={2}, ty={3}, tags={4})", this.Id, this.Title, this.Description, this.Type, this.Tags.ToString());
        }
    }
}
