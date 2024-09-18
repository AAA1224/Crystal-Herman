using System;
using UnityEngine;
using KoboldTools;

namespace Herman
{
    public interface IPerson : IItem
    {
        string Education { get; set; }
        string Hobbies { get; set; }
        int TalentId { get; set; }
    }

    [Serializable]
    public class Person : IPerson, IEquatable<Person>
    {
        [SerializeField] private string title;
        [SerializeField] private string description;
        [SerializeField] private int id;
        [SerializeField] private string education;
        [SerializeField] private string hobbies;
        [SerializeField] private int talentId;

        public void SetName(string name)
        {
            title = name;
        }

        public string Education
        {
            get
            {
                return this.education;
            }

            set
            {
                this.education = value;
            }
        }

        public string Hobbies
        {
            get
            {
                return this.hobbies;
            }

            set
            {
                this.hobbies = value;
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
            set
            {
                this.title = value;
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

        public string LocalisedEducation
        {
            get
            {
                if (Localisation.instance.hasTextId(this.education))
                {
                    return Localisation.instance.getLocalisedText(this.education);
                }
                else
                {
                    return this.education;
                }
            }
        }

        public string LocalisedHobbies
        {
            get
            {
                if (Localisation.instance.hasTextId(this.hobbies))
                {
                    return Localisation.instance.getLocalisedText(this.hobbies);
                }
                else
                {
                    return this.hobbies;
                }
            }
        }

        public int TalentId
        {
            get
            {
                return this.talentId;
            }

            set
            {
                this.talentId = value;
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
        public override string ToString()
        {
            return string.Format("Person(i={0}, t={1})", this.Id, this.Title);
        }
        public bool Equals(Person other)
        {
            return this.Id == other.Id &&
                this.Title == other.Title &&
                this.Description == other.Description &&
                this.Education == other.Education &&
                this.Hobbies == other.Hobbies &&
                this.TalentId == other.TalentId;
        }
    }
}
