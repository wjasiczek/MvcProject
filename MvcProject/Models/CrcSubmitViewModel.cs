using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcProject.Models
{
    public class CrcSubmitViewModel
    {
        public CrcModel crcModel { get; set; }

        public string result { get; set; }

        public CrcSubmitViewModel()
        {
            crcModel = new CrcModel();
        }
    }
}