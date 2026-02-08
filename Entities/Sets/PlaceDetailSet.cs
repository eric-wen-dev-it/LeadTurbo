using LeadTurbo.Artemis;
using System.ComponentModel.DataAnnotations;
using GoogleMapQuery.Entities;
namespace GoogleMapQuery.Entities.Sets
{
	public class PlaceDetailSet : EntitySet
	{
		protected override Entity CreateEntity()
		{
			return new PlaceDetail();
		}
		protected override EntityIndexSet CreateEntityIndexSet()
		{
			return new IndexSet(typeof(PlaceDetail));
		}
	}
}
