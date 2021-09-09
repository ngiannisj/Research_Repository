using SolrNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Research_Repository_Models.Solr
{
    public class ItemSolr
    {
        public ItemSolr() { }

        public ItemSolr(Item model)
        {
            this.Id = model.Id.ToString();
            this.Title = model.Title;
            this.Abstract = model.Abstract;
            this.Collaborator = model.Collaborator;
            this.DateRange = $"[{model.StartDate.ToShortDateString()} To {model.EndDate.ToShortDateString()}]";
            if (model.Theme != null)
            {
                this.Theme = model.Theme.Name;
            }
            if (model.Project != null)
            {
                this.Project = model.Project.Name;
            }
            if (model.ItemTags != null && model.ItemTags.Count() > 0)
            {
                this.Tags = model.ItemTags.Select(i => i.Tag.Name).ToList();
            }
            this.KeyInsights = model.KeyInsights.Split("~~");
            this.Methodology = model.Methodology;
            this.Limitations = model.Limitations;
            this.ApprovedUse = model.ApprovedUse;
            this.Sensitivity = model.Sensitivity;
            this.Uploader = model.Uploader.FullName;
        }

        [SolrUniqueKey("id")]
        public string Id { get; set; }
        [SolrField("title")]
        public string Title { get; set; }
        [SolrField("abstract")]
        public string Abstract { get; set; }
        [SolrField("collaborator")]
        public string Collaborator { get; set; }
        [SolrField("dateRange")]
        public string DateRange { get; set; }
        [SolrField("theme")]
        public string Theme { get; set; }
        [SolrField("project")]
        public string Project { get; set; }
        [SolrField("tags")]
        public ICollection<string> Tags { get; set; }
        [SolrField("keyInsights")]
        public ICollection<string> KeyInsights { get; set; }
        [SolrField("methodology")]
        public string Methodology { get; set; }
        [SolrField("limitations")]
        public string Limitations { get; set; }
        [SolrField("approvedUse")]
        public string ApprovedUse { get; set; }
        [SolrField("sensitivity")]
        public string Sensitivity { get; set; }
        [SolrField("uploader")]
        public string Uploader { get; set; }
    }
}
