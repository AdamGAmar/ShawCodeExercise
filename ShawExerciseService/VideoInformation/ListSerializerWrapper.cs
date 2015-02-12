using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Xml.Serialization;

namespace ShawExerciseService.VideoInformation
{
    /// <summary>
    /// A List class that loads the stored List of its type from AppData on construction, and can save itself back to disk.
    /// Somewhat mirrors the behaviour of a DataContext.
    /// Note: This class is not even the slightest bit thread-safe.
    /// In retrospect I probably should just have put in an SQLite database.
    /// </summary>
    public class ListSerializerWrapper<T>
    {
        /// <summary>
        /// The wrapper's List object. Once loaded, is not managed and depends on the class' user to call a function that writes back to disk.
        /// </summary>
        public List<T> List { get; private set; }

        /// <summary>
        /// The path to the AppData folder, in which the serialized files are stored.
        /// </summary>
        public readonly string ProjectDirectory = HttpContext.Current.Server.MapPath("~/");
        // Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>
        /// The suffix to add to the saved files.
        /// </summary>
        public readonly string FileSuffix = "_listData.xml";

        /// <summary>
        /// The full path to the serialized file that will be created, modified and/or read when serializing the internal list.
        /// </summary>
        public string FilePath {
            get
            {
                return this.ProjectDirectory  + "App_Data\\" + this.GetType().GetGenericArguments().First().Name + this.FileSuffix;
            }
        }

        /// <summary>
        /// Constructor. Loads list from disk into memory for the lifespan of the object.
        /// </summary>
        public ListSerializerWrapper()
        {
            this._LoadList();
        }

        // Helper function: load list from disk.
        private void _LoadList()
        {
            List = new List<T>();

            if (File.Exists(FilePath))
            {
                XmlSerializer deserializer = new XmlSerializer(List.GetType());
                TextReader textReader = new StreamReader(FilePath);
                List = (List<T>)deserializer.Deserialize(textReader);
                textReader.Close();
            }
        }

        // Helper function: save list to disk.
        private void _SaveList()
        {
            try {
                bool fileExists = false;

                if (File.Exists(FilePath))
                {
                    File.Move(FilePath, FilePath + ".old");
                    fileExists = true;
                }

                XmlSerializer serializer = new XmlSerializer(List.GetType());
                TextWriter textWriter = new StreamWriter(FilePath);
                serializer.Serialize(textWriter, List);
                textWriter.Close();

                if (fileExists)
                {
                    File.Delete(FilePath + ".old");
                }
            }
            catch (Exception)
            {
                // TODO: replace the old file
            }

        }

        /// <summary>
        /// Serialize the current state of the list to disk.
        /// </summary>
        public void Save()
        {
            _SaveList();
        }

        /// <summary>
        /// Remove the serialized file from disk and clear the List.
        /// </summary>
        public void DeleteData()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }

            List = new List<T>();
        }

        public void AddAndSave(T item)
        {
            List.Add(item);
            this.Save();
        }

        public void RemoveAndSave(T item)
        {
            List.Remove(item);
            this.Save();
        }

        public void RemoveAllAndSave(Predicate<T> match)
        {
            List.RemoveAll(match);
            this.Save();
        }
    }
}