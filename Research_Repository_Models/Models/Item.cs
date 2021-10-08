using Research_Repository_Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Research_Repository_Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        //Project information
        public string Collaborator { get; set; }

        public string SuggestedTeam { get; set; }

        public string SuggestedProject { get; set; }

        public string SuggestedTags { get; set; }

        public string SuggestedTheme { get; set; }

        [Display(Name = "Start date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = WC.YearMonthDayTagHelper, ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = WC.YearMonthDayTagHelper, ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Last updated")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = WC.YearMonthDayTagHelper, ApplyFormatInEditMode = true)]
        public DateTime LastUpdatedDate { get; set; }

        [Display(Name = "Date created")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = WC.YearMonthDayTagHelper, ApplyFormatInEditMode = true)]
        public DateTime DateCreated { get; set; }


        [Display(Name = "Theme Type")]
        public int? ThemeId { get; set; }

        [ForeignKey("ThemeId")]
        public virtual Theme Theme { get; set; }

        [Display(Name = "Team Type")]
        public int? TeamId { get; set; }

        [ForeignKey("TeamId")]
        public virtual Team Team { get; set; }

        [Display(Name = "Project Type")]
        public int? ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        public ICollection<ItemTag> ItemTags { get; set; }

        //Research information
        [Required]
        public string Title { get; set; }

        public string Abstract { get; set; }

        public string KeyInsights { get; set; }

        public string Methodology { get; set; }

        public string Limitations { get; set; }

        //Security
        public string ApprovedUse { get; set; }

        public string Sensitivity { get; set; }

        //Files
        public string Files { get; set; }

        //Status
        public string Status { get; set; }

        [Display(Name = "Reason")]
        public string Comment { get; set; }

        //User
        [Display(Name = "Uploader")]
        public string UploaderId { get; set; }

        [ForeignKey("UploaderId")]
        public virtual ApplicationUser Uploader { get; set; }

        //Notifications
        public bool NotifyUploader { get; set; }
    }
}
