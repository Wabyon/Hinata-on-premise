using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Hinata.Web.Mvc.DataAnnotations;

namespace Hinata.Models
{
    public class LikeViewModel
    {
        public string Id { get; set; }

        public string ItemId { get; set; }

        public string UserId { get; set; }

    }
}