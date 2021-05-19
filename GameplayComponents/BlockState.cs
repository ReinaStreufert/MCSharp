using MCSharp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCSharp.GameplayComponents
{
    public class BlockState
    {
        public string NamespacedID { get; set; }
        /*public int GetNumericStateID(MCSharpServer Server, MinecraftVersion Version)
        {

        }*/
        private Dictionary<string, string> properties = new Dictionary<string, string>();
        public string this[string property]
        {
            get
            {
                string value;
                if (properties.TryGetValue(property, out value))
                {
                    return value;
                } else
                {
                    return "";
                }
            }
            set
            {
                if (value == "")
                {
                    if (properties.ContainsKey(property))
                    {
                        properties.Remove(value);
                    }
                } else
                {
                    if (properties.ContainsKey(property))
                    {
                        properties[property] = value;
                    } else
                    {
                        properties.Add(property, value);
                    }
                }
            }
        }
        public BlockState(string NamespacedID, params string[] PropertyValues)
        {
            this.NamespacedID = NamespacedID;
            if (PropertyValues.Length % 2 > 0)
            {
                Console.WriteLine("There must be an even number of property values. Arguments are given in a property, then value, then repeat pattern.");
            }
            string lastProperty = "";
            for (int i = 0; i < PropertyValues.Length; i++)
            {
                if (lastProperty == "")
                {
                    lastProperty = PropertyValues[i];
                } else
                {
                    this[lastProperty] = PropertyValues[i];
                    lastProperty = "";
                }
            }
        }
    }
}
