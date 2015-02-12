using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
using ShawExerciseService.VideoInformation;
using System.Net;

namespace ShawExerciseService
{
    public class ShowService : IShowService
    {
        private ListSerializerWrapper<Show> _showData;
        private ListSerializerWrapper<Video> _videoData;
        private ListSerializerWrapper<Category> _categoryData;

        // CreateShow - Create a new show and load it into our data set.
        /// <inheritdoc />
        public void CreateShow(Stream JSONDataStream)
        {
            // Load required data into memory. (Alternatively, create database connection here.)
            _showData = new ListSerializerWrapper<Show>();

            try
            {
                // Retrieve JSON data string, loading it into JSONdata variable.
                StreamReader reader = new StreamReader(JSONDataStream);
                string JSONdata = reader.ReadToEnd();

                // Deserialize input JSON string, creating a new Show class.
                JavaScriptSerializer jss = new JavaScriptSerializer();
                Show newShow = jss.Deserialize<Show>(JSONdata);

                if (newShow == null)
                {
                    throw new WebFaultException<string>("Could not deserialize JSON data.", HttpStatusCode.BadRequest);
                }

                // Check if the show ID already exists.
                if (!_showData.List.Any(s => s.Id == newShow.Id))
                {
                    // Show does not yet exist - create it
                    _showData.AddAndSave(newShow);
                }
                else
                {
                    // Show ID already exists. Return an error.
                    throw new WebFaultException<string>("Show ID " + newShow.Id + " already exists.", HttpStatusCode.BadRequest);
                }

            }
            catch (Exception e)
            {
                // Some other error occurred.
                throw new WebFaultException<string>(e.Message, HttpStatusCode.BadRequest);
            }

            // Otherwise, return success!
            WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Created;
        }

        // CreateVideo - create a new Video object, if the parent Category and Show exist.
        /// <inheritdoc />
        public void CreateVideo(Stream JSONDataStream)
        {
            // Load required data into memory.
            _showData = new ListSerializerWrapper<Show>();
            _videoData = new ListSerializerWrapper<Video>();
            _categoryData = new ListSerializerWrapper<Category>();

            try
            {
                // Retrieve JSON data string, loading it into JSONdata variable.
                StreamReader reader = new StreamReader(JSONDataStream);
                string JSONdata = reader.ReadToEnd();

                // Deserialize input JSON string, creating a new Video class.
                JavaScriptSerializer jss = new JavaScriptSerializer();
                Video newVideo = jss.Deserialize<Video>(JSONdata);

                if (newVideo == null)
                {
                    throw new WebFaultException<string>("Could not deserialize JSON data.", HttpStatusCode.BadRequest);
                }

                if (!_showData.List.Any(s => s.Id == newVideo.ParentShowId))
                {
                    throw new WebFaultException<string>("Show ID " + newVideo.ParentShowId + " does not exist.", HttpStatusCode.BadRequest);
                }

                if (!_categoryData.List.Any(c => c.Id == newVideo.CategoryId))
                {
                    throw new WebFaultException<string>("Category ID " + newVideo.CategoryId + " does not exist.", HttpStatusCode.BadRequest); ;
                }

                // Check if the show ID already exists.
                if (!_videoData.List.Any(v => v.Id == newVideo.Id))
                {
                    // Video does not yet exist - create it
                    _videoData.AddAndSave(newVideo);
                }
                else 
                { 
                    // Video ID already exists
                    throw new WebFaultException<string>("Video ID " + newVideo.Id + " already exists.", HttpStatusCode.BadRequest);
                }
            }
            catch (Exception e)
            {
                // Some other error occurred.
                throw new WebFaultException<string>(e.Message, HttpStatusCode.BadRequest);
            }

            // Otherwise, return success!
            WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Created;
        }

        // CreateCategory - create a new Category object and load it into our data set.
        /// <inheritdoc />
        public void CreateCategory(Stream JSONDataStream)
        {
            // Load required data into memory.
            _categoryData = new ListSerializerWrapper<Category>();

            try
            {
                // Retrieve JSON data string, loading it into JSONdata variable.
                StreamReader reader = new StreamReader(JSONDataStream);
                string JSONdata = reader.ReadToEnd();

                // Deserialize input JSON string, creating a new Category class.
                JavaScriptSerializer jss = new JavaScriptSerializer();
                Category newCategory = jss.Deserialize<Category>(JSONdata);

                if (newCategory == null)
                {
                    throw new WebFaultException<string>("Could not deserialize JSON data.", HttpStatusCode.BadRequest);
                }

                // Check if the category ID already exists.
                if (!_categoryData.List.Any(c => c.Id == newCategory.Id))
                {
                    // Category does not yet exist - create it
                    _categoryData.AddAndSave(newCategory);
                }
                else
                {
                    // Category ID already exists
                    throw new WebFaultException<string>("Category ID " + newCategory.Id + " already exists.", HttpStatusCode.BadRequest);
                }

            }
            catch (Exception e)
            {
                // Some other error occurred.
                throw new WebFaultException<string>(e.Message, HttpStatusCode.BadRequest);
            }

            // Otherwise, return success!
            WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Created;
        }


        // GetShowListing - return the header information for all shows (i.e. for a menu)
        /// <inheritdoc />
        public List<Show> GetShowListing()
        {
            // Load data from disk...
            _showData = new ListSerializerWrapper<Show>();

            // ...and pass it back.
            return _showData.List;
        }


        // GetShowData - return a single show and its videos in a ShowDataResponse package
        /// <inheritdoc />
        public ShowDataResponse GetShowData(string showId)
        {
            ShowDataResponse resp = new ShowDataResponse();
            int showIdInt;

            if (!int.TryParse(showId, out showIdInt))
            {
                throw new WebFaultException<string>("Input was not in integer format.", HttpStatusCode.BadRequest);
            }

            // Load show data from disk and check if we can find the show.
            IEnumerable<Show> matchingShows = GetShowListing().Where(s => s.Id == showIdInt);

            if (matchingShows.Count() != 1)
            {
                throw new WebFaultException<string>("Could not find the requested show ID (or duplicate show IDs exist).", HttpStatusCode.NotFound);
            }
            else
            {
                resp.Show = matchingShows.First();
            }

            // Load video data from disk, and add it to our show response package.
            _videoData = new ListSerializerWrapper<Video>();
            IEnumerable<Video> videos = _videoData.List.Where(v => v.ParentShowId == showIdInt);

            resp.Videos = videos.ToList();

            return resp;
        }

        // DeleteShow - remove a show from our serialized data.
        /// <inheritdoc />
        public void DeleteShow(string showId)
        {
            // Load required data into memory.
            int showIdInt;

            if (!int.TryParse(showId, out showIdInt))
            {
                throw new WebFaultException<string>("Input was not in integer format.", HttpStatusCode.BadRequest);
            }

            // Load show data into memory.
            _showData = new ListSerializerWrapper<Show>();

            // Delete show and any associated videos.
            if (_showData.List.Any(s => s.Id == showIdInt))
            {
                // If show exists, also load video data into memory to delete associated videos.
                _videoData = new ListSerializerWrapper<Video>();

                _showData.RemoveAllAndSave(s => s.Id == showIdInt);
                _videoData.RemoveAllAndSave(v => v.ParentShowId == showIdInt);
            }
            else
            {
                // Could not find the show to delete.
                throw new WebFaultException<string>("Could not find the requested show ID.", HttpStatusCode.NotFound);
            }
            
            // Show successfully deleted - exit with status code 200 OK
        }
    }
}
