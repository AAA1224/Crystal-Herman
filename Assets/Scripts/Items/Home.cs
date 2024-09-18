using System;
using UnityEngine;
using KoboldTools;

namespace Herman
{
    public interface IHome : IItem
    {
        int Rent { get; set; }
    }

    [Serializable]
    public class Home : IHome, IEquatable<Home>
    {

        [SerializeField] private string title;
        [SerializeField] private string description;
        [SerializeField] private int id;
        [SerializeField] private int cost;

        public int Rent
        {
            get
            {
                return this.cost;
            }

            set
            {
                this.cost = value;
            }
        }
        public string Description
        {
            get {
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

        public override string ToString()
        {
            return String.Format("Home(i={0}, t={1}, r={2})", this.Id, this.Title, this.Rent);
        }
        public bool Equals(Home other)
        {
            bool IdEq = this.Id == other.Id;
            bool titleEq = this.Title == other.Title;
            bool descrEq = this.Description == other.Description;
            bool costEq = this.Rent == other.Rent;

            return IdEq && titleEq && descrEq && costEq;
        }
    }
}
