using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace mentalmath
{
    /// <summary>
    /// Configs of the software
    /// </summary>
    public class ApplicationConfiguration : ConfigBase
    {
        private GeneratorConfiguration genconf;
        /// <summary>
        /// The currently used Configuration for expression generation
        /// </summary>
        public GeneratorConfiguration GeneratorConfig
        {
            get { return genconf; }
            set { genconf = value; }
        }

        public ApplicationConfiguration() : this(false)
        {
            
        }

        public ApplicationConfiguration(bool createDefaultProfiles)
        {
            GeneratorConfig = new GeneratorConfiguration();
            if(createDefaultProfiles)
            {
                GeneratorProfiles = new ObservableCollection<GeneratorConfiguration>()
                {
                    new GeneratorConfiguration()
                    {
                         Plus = false,
                         Minus = false,
                         Divide = false,
                         MaxResult = 100,
                         Name = "Small 1x1"
                    },
                    new GeneratorConfiguration()
                    {
                         Plus = false,
                         Minus = false,
                         Divide = false,
                         MaxResult = 1000,
                         Name = "Big 1x1"
                    }
                };
            }
            else
                GeneratorProfiles = new ObservableCollection<GeneratorConfiguration>();
        }

        private ObservableCollection<GeneratorConfiguration> genprofiles;
        /// <summary>
        /// Predefined Profiles for expression generation
        /// </summary>
        public ObservableCollection<GeneratorConfiguration> GeneratorProfiles
        {
            get { return genprofiles; }
            set { genprofiles = value; }
        }


        #region Save and Load stuff

        private static readonly string file = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/mentalmath/config.xml";
        private static readonly string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/mentalmath/";

        public void Save()
        {
            try
            {
                XmlSerializer s = new XmlSerializer(this.GetType());
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                File.Delete(file);
                FileStream fs = File.OpenWrite(file);
                s.Serialize(fs, this);
                fs.Close();
            }
            catch //catch possible write-protection. in this case do not save configs
            {
            }
        }

        public static ApplicationConfiguration Load()
        {
            try
            {
                if (File.Exists(file))
                {
                    FileStream fs = File.OpenRead(file);
                    XmlSerializer s = new XmlSerializer(typeof(ApplicationConfiguration));
                    ApplicationConfiguration conf = (ApplicationConfiguration)s.Deserialize(fs);
                    fs.Close();
                    return conf;
                }
            }
            catch
            {
            }
            return new ApplicationConfiguration(true); //no or invalid savefile? return defaults
        }

        #endregion
    }
}
