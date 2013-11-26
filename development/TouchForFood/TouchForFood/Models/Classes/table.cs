using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TouchForFood.Models
{
    [MetadataType(typeof(TableMetadata))]
    public partial class table
    {
        public string generateNFCHash()
        {
            Util.Security.AES aes = new Util.Security.AES();
            string toRet = aes.EncryptToString(this.id.ToString());
            return toRet;
        }
    }

    public class TableMetadata
    {
        [Required]
        public string name;
    }
}