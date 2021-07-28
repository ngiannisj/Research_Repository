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
            this.Description = model.Description;

        }

        [SolrUniqueKey("id")]
        public string Id { get; set; }
        [SolrField("title")]
        public string Title { get; set; }
        [SolrField("description")]
        public string? Description { get; set; }
    }
}
