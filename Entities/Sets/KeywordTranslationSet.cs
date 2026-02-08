using LeadTurbo.Artemis;
using System.ComponentModel.DataAnnotations;
using GoogleMapQuery.Entities;
namespace GoogleMapQuery.Entities.Sets
{
	public class KeywordTranslationSet : EntitySet
	{
		protected override Entity CreateEntity()
		{
			return new KeywordTranslation();
		}
		protected override EntityIndexSet CreateEntityIndexSet()
		{
			return new IndexSet(typeof(KeywordTranslation));
		}
	}
}
