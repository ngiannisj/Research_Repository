using Research_Repository_Utility;
using SolrNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Research_Repository_Models.Solr
{
    public class ItemSolr
    {

        public ItemSolr(Item model)
        {
            this.Id = model.Id.ToString();
            if (!string.IsNullOrEmpty(model.Title))
            {
                this.Title = model.Title;
            }
            if (!string.IsNullOrEmpty(model.Abstract))
            {
                this.Abstract = model.Abstract;
            }
            if (!string.IsNullOrEmpty(model.Collaborator))
            {
                this.Collaborator = model.Collaborator;
            }
            if (model.Theme != null)
            {
                this.Theme = model.Theme.Name;
            }
            if (model.Project != null)
            {
                this.Project = model.Project.Name;

                if (model.Project.Team != null)
                {
                    this.Team = model.Project.Team.Name;
                }
            }
            if (model.ItemTags != null && model.ItemTags.Count > 0)
            {
                this.Tags = model.ItemTags.Select(i => i.Tag.Name).ToList();
            }
            if (!string.IsNullOrEmpty(model.KeyInsights))
            {
                this.KeyInsights = model.KeyInsights.Split("~~");
            }
            if (!string.IsNullOrEmpty(model.Methodology))
            {
                this.Methodology = model.Methodology;
            }
            if (!string.IsNullOrEmpty(model.Limitations))
            {
                this.Limitations = model.Limitations;
            }
            if (!string.IsNullOrEmpty(model.ApprovedUse))
            {
                this.ApprovedUse = model.ApprovedUse;
            }
            if (!string.IsNullOrEmpty(model.Sensitivity))
            {
                this.Sensitivity = model.Sensitivity;
            }
            if (!string.IsNullOrEmpty(model.Status))
            {
                this.Status = model.Status;
            }
            if (model.Uploader != null)
            {
                if (!string.IsNullOrEmpty(model.Uploader.FirstName) && !string.IsNullOrEmpty(model.Uploader.LastName))
                {
                    this.Uploader = model.Uploader.FirstName + " " + model.Uploader.LastName;
                }
            }
            this.DateRange = $"[{model.StartDate.ToString(WC.YearMonthDay)} TO {model.EndDate.ToString("yyyy-MM-dd")}]";
            this.NotifyUploader = model.NotifyUploader;
            this.LastUpdatedDate = model.LastUpdatedDate;
            this.DateCreated = model.DateCreated;
        }

        [SolrUniqueKey("id")]
        public string Id { get; set; }
        [SolrField("title_tti")]
        public string Title { get; set; }
        [SolrField("abstract_tti")]
        public string Abstract { get; set; }
        [SolrField("collaborator_tti")]
        public string Collaborator { get; set; }
        [SolrField("dateRange")]
        public string DateRange { get; set; }
        [SolrField("theme_tti")]
        public string Theme { get; set; }
        [SolrField("project_tti")]
        public string Project { get; set; }
        [SolrField("team_tti")]
        public string Team { get; set; }
        [SolrField("tags_tti")]
        public ICollection<string> Tags { get; set; }
        [SolrField("keyInsights_tti")]
        public ICollection<string> KeyInsights { get; set; }
        [SolrField("methodology_tti")]
        public string Methodology { get; set; }
        [SolrField("limitations_tti")]
        public string Limitations { get; set; }
        [SolrField("approvedUse")]
        public string ApprovedUse { get; set; }
        [SolrField("sensitivity")]
        public string Sensitivity { get; set; }
        [SolrField("status")]
        public string Status { get; set; }
        [SolrField("notifyUploader")]
        public bool NotifyUploader { get; set; }
        [SolrField("uploader_tti")]
        public string Uploader { get; set; }
        [SolrField("uploaderId")]
        public string UploaderId { get; set; }
        [SolrField("lastUpdatedDate")]
        public DateTime LastUpdatedDate { get; set; }
        [SolrField("dateCreated")]
        public DateTime DateCreated { get; set; }
    }
}
