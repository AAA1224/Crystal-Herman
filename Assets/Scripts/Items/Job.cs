using System;
using UnityEngine;
using KoboldTools;

namespace Herman
{
    public interface IJob : IItem
    {
        int TimeCost { get; set; }
        int Salary { get; set; }
    }

    [Serializable]
    public class Job : IJob, IEquatable<Job>
    {
        [SerializeField] private string title;
        [SerializeField] private string description;
        [SerializeField] private int id;
        [SerializeField] private int timecost;
        [SerializeField] private int earnings;

        public int TimeCost
        {
            get
            {
                return this.timecost;
            }

            set
            {
                this.timecost = value;
            }
        }
        public int Salary
        {
            get
            {
                return this.earnings;
            }

            set
            {
                this.earnings = value;
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
            return string.Format("Job(i={0}, t={1}, s={2})", this.Id, this.Title, this.Salary);
        }

        public bool Equals(Job other)
        {
            bool idEq = this.Id == other.Id;
            bool titleEq = this.Title == other.Title;
            bool descrEq = this.Description == other.Description;
            bool costEq = this.TimeCost == other.TimeCost;
            bool earningsEq = this.Salary == other.Salary;

            return idEq && titleEq && descrEq && costEq && earningsEq;
        }
    }
}
