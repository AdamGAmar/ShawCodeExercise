using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using ShawExerciseService.VideoInformation;

namespace ShawExerciseService
{
    [ServiceContract]
    public interface IShowService
    {
        
        /// <summary>
        /// Retrieves a list of all Shows available.
        /// </summary>
        /// <returns>A List containing all created Shows.</returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "getShowListing")]
        List<Show> GetShowListing();

        /// <summary>
        /// Get show and corresponding video data.
        /// </summary>
        /// <param name="showId">The ID of the show to retrieve.</param>
        /// <returns>A ShowDataResponse package containing the requested information.</returns>
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "getShowData/{showId}")]
        ShowDataResponse GetShowData(string showId);

        /// <summary>
        /// Create a new show.
        /// </summary>
        /// <param name="JSONdataStream">JSON-serialized Show class.</param>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "createShow")]
        void CreateShow(Stream JSONdataStream);

        /// <summary>
        /// Create a new video belonging to a show.
        /// </summary>
        /// <param name="JSONdataStream">JSON-serialized Video class.</param>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "createVideo")]
        void CreateVideo(Stream JSONdataStream);

        /// <summary>
        /// Create a new video category.
        /// </summary>
        /// <param name="JSONdataStream">JSON-serialized Category class.</param>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "createCategory")]
        void CreateCategory(Stream JSONdataStream);

        /// <summary>
        /// Delete a show and corresponding videos.
        /// </summary>
        /// <param name="JSONdataStream">JSON-serialized Category class.</param>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "deleteShow/{showId}")]
        void DeleteShow(string showId);

    }

}
