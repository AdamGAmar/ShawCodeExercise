using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ShawExerciseService.VideoInformation
{
    [DataContract]
    public class Video
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int CategoryId { get; set; }

        [DataMember]
        public int ParentShowId { get; set; }

        [DataMember]
        public int Season { get; set; }

        [DataMember]
        public int Episode { get; set; }
    }
}