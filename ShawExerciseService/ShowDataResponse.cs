using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using ShawExerciseService.VideoInformation;

namespace ShawExerciseService
{
    [DataContract]
    public class ShowDataResponse
    {
        [DataMember]
        public Show Show { get; set; }

        [DataMember]
        public List<Video> Videos  { get; set; }

    }
}