using System;
using KoboldTools;

namespace Herman
{
    public interface IItem
    {
        int Id { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string LocalisedTitle { get; }
        string LocalisedDescription { get; }
    }

    [Serializable]
    public class Item : IItem, IEquatable<Item>
    {
        private string _title = "Title";
        private string _description = "Description";
        private int _id = -1;

        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
            }
        }
        public string Description {
            get
            {
                return _description;
            }

            set
            {
                _description = value;
            }
        }

        public string LocalisedTitle
        {
            get
            {
                if (Localisation.instance.hasTextId(this._title))
                {
                    return Localisation.instance.getLocalisedText(this._title);
                }
                else
                {
                    return this._title;
                }
            }
        }

        public string LocalisedDescription
        {
            get
            {
                if (Localisation.instance.hasTextId(this._description))
                {
                    return Localisation.instance.getLocalisedText(this._description);
                }
                else
                {
                    return this._description;
                }
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }
        public bool Equals(Item other)
        {
            bool IdEq = this.Id == other.Id;
            bool titleEq = this.Title == other.Title;
            bool descrEq = this.Description == other.Description;

            return IdEq && titleEq && descrEq;
        }
    }
}
