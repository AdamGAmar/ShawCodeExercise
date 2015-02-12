using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace ShawExerciseService.VideoInformation
{
    [DataContract]
    public class Show
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        // Uri class not used because it is not XML serializable - could write wrapper to fix
        [DataMember]
        public string BackgroundImageUri { get; set; }

        public Show()
        {
        }

        public Show(int _id, string _name, string _backgroundImageUri)
        {
            Id = _id;
            Name = _name;
            BackgroundImageUri = _backgroundImageUri;
        }
    }
}